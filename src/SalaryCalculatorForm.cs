using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace PersonalOrganizer
{
    public class SalaryCalculatorForm : Form
    {
        private ComboBox cmbFamilyStatus;
        private ComboBox cmbExperience;
        private ComboBox cmbLocation;
        private ComboBox cmbEducation;
        private ComboBox cmbLanguage;
        private ComboBox cmbPosition;
        private Label lblBaseSalary;
        private Label lblTotalSalary;
        private Button btnCalculate;
        private User currentUser;

        // Sabitleri ve katsayıları tanımla
        private const decimal BASE_SALARY = 70000M; // Taban maaş

        // Katsayı tabloları
        private Dictionary<string, decimal> familyFactors = new Dictionary<string, decimal>
        {
            { "Evli ve eşi çalışmıyor", 0.20M },
            { "0-6 yaş arası çocuk", 0.20M },
            { "7-18 yaş arası çocuk", 0.30M },
            { "18 yaş üstü çocuk", 0.40M },
            { "Üniversite lisans/ön lisans öğrencisi olmak koşuluyla", 0.40M }
        };

        private Dictionary<string, decimal> experienceFactors = new Dictionary<string, decimal>
        {
            { "2-4 yıl", 0.60M },
            { "5-9 yıl", 1.00M },
            { "10-14 yıl", 1.20M },
            { "15-20 yıl", 1.35M },
            { "20 yıl üstü", 1.50M }
        };

        private Dictionary<string, decimal> locationFactors = new Dictionary<string, decimal>
        {
            { "TR10: İstanbul", 0.30M },
            { "TR51: Ankara", 0.20M },
            { "TR31: İzmir", 0.20M },
            { "TR42: Kocaeli, Sakarya, Düzce, Bolu, Yalova", 0.10M },
            { "TR21: Edirne, Kırklareli, Tekirdağ", 0.10M },
            { "TR90: Trabzon, Ordu, Giresun, Rize, Artvin, Gümüşhane", 0.05M },
            { "TR41: Bursa, Eskişehir, Bilecik", 0.05M },
            { "TR32: Aydın, Denizli, Muğla", 0.05M },
            { "TR62: Adana, Mersin", 0.05M },
            { "TR22: Balıkesir, Çanakkale", 0.05M },
            { "TR61: Antalya, Isparta, Burdur", 0.05M },
            { "Diğer İller", 0.00M }
        };

        private Dictionary<string, decimal> educationFactors = new Dictionary<string, decimal>
        {
            { "Meslek alanı ile ilgili yüksek lisans", 0.10M },
            { "Meslek alanı ile ilgili doktora", 0.30M },
            { "Meslek alanı ile ilgili doçentlik", 0.35M },
            { "Meslek alanı ile ilgili olmayan yüksek lisans", 0.05M },
            { "Meslek alanı ile ilgili olmayan doktora/doçentlik", 0.15M }
        };

        private Dictionary<string, decimal> languageFactors = new Dictionary<string, decimal>
        {
            { "Belgelendirilmiş İngilizce bilgisi", 0.20M },
            { "İngilizce eğitim veren okul mezuniyeti", 0.20M },
            { "Belgelendirilmiş diğer yabancı dil bilgisi (her dil için)", 0.05M }
        };

        private Dictionary<string, decimal> positionFactors = new Dictionary<string, decimal>
        {
            { "Takım Lideri/Grup Yöneticisi/Teknik Yönetici/Yazılım Mimarı", 0.50M },
            { "Proje Yöneticisi", 0.75M },
            { "Direktör/Projeler Yöneticisi", 0.85M },
            { "CTO/Genel Müdür", 1.00M },
            { "Bilgi İşlem Sorumlusu/Müdürü (Bilgi işlem biriminde en çok 5 bilişim personeli varsa)", 0.40M },
            { "Bilgi İşlem Sorumlusu/Müdürü (Bilgi işlem biriminde 5'ten çok bilişim personeli varsa)", 0.60M }
        };

        public SalaryCalculatorForm(User user)
        {
            currentUser = user;
            InitializeComponents();
            LoadUserData();
        }

        private void InitializeComponents()
        {
            this.Text = "Maaş Hesaplama - Kişisel Organizatör";
            this.Size = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(800, 600);
            this.BackColor = Color.FromArgb(245, 245, 245);

            // Sol panel (logo/banner alanı)
            Panel leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = this.Width / 3,
                BackColor = Color.FromArgb(0, 102, 204)
            };

            // Banner başlık
            Label lblAppName = new Label
            {
                Text = "Maaş Hesaplama",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(0, 50, 0, 0)
            };
            leftPanel.Controls.Add(lblAppName);

            // Açıklama metni
            Label lblDescription = new Label
            {
                Text = "Farklı faktörlere göre\nmaaş hesaplamanızı yapın\nve kariyerinizi planlayın.",
                Font = new Font("Segoe UI Light", 16, FontStyle.Regular),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 50, 20, 0)
            };
            leftPanel.Controls.Add(lblDescription);

            // Sağ panel (form alanı)
            Panel rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(40)
            };

            // Form container
            Panel formContainer = new Panel
            {
                Width = 650,
                Height = 600,
                Anchor = AnchorStyles.None,
                BackColor = Color.White,
                AutoScroll = true
            };
            
            // Panel'ı ortala
            formContainer.Location = new Point(
                (rightPanel.ClientSize.Width - formContainer.Width) / 2,
                (rightPanel.ClientSize.Height - formContainer.Height) / 2);

            // Form başlığı
            Label lblTitle = new Label
            {
                Text = "Maaş Hesaplama Kriterleri",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(0, 0),
                Size = new Size(650, 40)
            };
            formContainer.Controls.Add(lblTitle);

            int yPos = 60;
            const int X_LABEL = 20;
            const int X_CONTROL = 250;
            const int LABEL_WIDTH = 220;
            const int CONTROL_WIDTH = 350;
            const int HEIGHT = 30;
            const int MARGIN = 30;

            // Aile Durumu
            var lblFamily = new Label
            {
                Text = "Aile Durumu:",
                Location = new Point(X_LABEL, yPos),
                Size = new Size(LABEL_WIDTH, HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10)
            };
            formContainer.Controls.Add(lblFamily);

            cmbFamilyStatus = new ComboBox
            {
                Location = new Point(X_CONTROL, yPos),
                Size = new Size(CONTROL_WIDTH, HEIGHT),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbFamilyStatus.Items.Add("Seçiniz");
            foreach (var item in familyFactors.Keys)
            {
                cmbFamilyStatus.Items.Add(item);
            }
            cmbFamilyStatus.SelectedIndex = 0;
            formContainer.Controls.Add(cmbFamilyStatus);

            yPos += HEIGHT + MARGIN;

            // Deneyim Süresi
            var lblExp = new Label
            {
                Text = "Deneyim Süresi (Yıl):",
                Location = new Point(X_LABEL, yPos),
                Size = new Size(LABEL_WIDTH, HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10)
            };
            formContainer.Controls.Add(lblExp);

            cmbExperience = new ComboBox
            {
                Location = new Point(X_CONTROL, yPos),
                Size = new Size(CONTROL_WIDTH, HEIGHT),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbExperience.Items.Add("Seçiniz");
            foreach (var item in experienceFactors.Keys)
            {
                cmbExperience.Items.Add(item);
            }
            cmbExperience.SelectedIndex = 0;
            formContainer.Controls.Add(cmbExperience);

            yPos += HEIGHT + MARGIN;

            // Yaşanılan İl (TÜİK)
            var lblLoc = new Label
            {
                Text = "Yaşanılan İl Grubu:",
                Location = new Point(X_LABEL, yPos),
                Size = new Size(LABEL_WIDTH, HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10)
            };
            formContainer.Controls.Add(lblLoc);

            cmbLocation = new ComboBox
            {
                Location = new Point(X_CONTROL, yPos),
                Size = new Size(CONTROL_WIDTH, HEIGHT),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbLocation.Items.Add("Seçiniz");
            foreach (var item in locationFactors.Keys)
            {
                cmbLocation.Items.Add(item);
            }
            cmbLocation.SelectedIndex = 0;
            formContainer.Controls.Add(cmbLocation);

            yPos += HEIGHT + MARGIN;

            // Akademik Derece
            var lblEdu = new Label
            {
                Text = "Alınan Akademik Derece:",
                Location = new Point(X_LABEL, yPos),
                Size = new Size(LABEL_WIDTH, HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10)
            };
            formContainer.Controls.Add(lblEdu);

            cmbEducation = new ComboBox
            {
                Location = new Point(X_CONTROL, yPos),
                Size = new Size(CONTROL_WIDTH, HEIGHT),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbEducation.Items.Add("Seçiniz");
            foreach (var item in educationFactors.Keys)
            {
                cmbEducation.Items.Add(item);
            }
            cmbEducation.SelectedIndex = 0;
            formContainer.Controls.Add(cmbEducation);

            yPos += HEIGHT + MARGIN;

            // Yabancı Dil Bilgisi
            var lblLang = new Label
            {
                Text = "Yabancı Dil Bilgisi:",
                Location = new Point(X_LABEL, yPos),
                Size = new Size(LABEL_WIDTH, HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10)
            };
            formContainer.Controls.Add(lblLang);

            cmbLanguage = new ComboBox
            {
                Location = new Point(X_CONTROL, yPos),
                Size = new Size(CONTROL_WIDTH, HEIGHT),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbLanguage.Items.Add("Seçiniz");
            foreach (var item in languageFactors.Keys)
            {
                cmbLanguage.Items.Add(item);
            }
            cmbLanguage.SelectedIndex = 0;
            formContainer.Controls.Add(cmbLanguage);

            yPos += HEIGHT + MARGIN;

            // Yöneticilik Görevi
            var lblPos = new Label
            {
                Text = "Yöneticilik Görevi:",
                Location = new Point(X_LABEL, yPos),
                Size = new Size(LABEL_WIDTH, HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10)
            };
            formContainer.Controls.Add(lblPos);

            cmbPosition = new ComboBox
            {
                Location = new Point(X_CONTROL, yPos),
                Size = new Size(CONTROL_WIDTH, HEIGHT),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbPosition.Items.Add("Seçiniz");
            foreach (var item in positionFactors.Keys)
            {
                cmbPosition.Items.Add(item);
            }
            cmbPosition.SelectedIndex = 0;
            formContainer.Controls.Add(cmbPosition);

            yPos += HEIGHT + MARGIN;

            // Taban maaş bilgi alanı
            var lblBaseInfo = new Label
            {
                Text = "Taban Maaş:",
                Location = new Point(X_LABEL, yPos),
                Size = new Size(LABEL_WIDTH, HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10)
            };
            formContainer.Controls.Add(lblBaseInfo);

            lblBaseSalary = new Label
            {
                Text = BASE_SALARY.ToString("C2"),
                Location = new Point(X_CONTROL, yPos),
                Size = new Size(CONTROL_WIDTH, HEIGHT),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204)
            };
            formContainer.Controls.Add(lblBaseSalary);

            yPos += HEIGHT + MARGIN;

            // Hesaplama butonu
            btnCalculate = new Button
            {
                Text = "Maaşı Hesapla",
                Location = new Point(X_CONTROL, yPos),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCalculate.FlatAppearance.BorderSize = 0;
            btnCalculate.Click += BtnCalculate_Click;
            formContainer.Controls.Add(btnCalculate);

            yPos += 60;

            // Sonuç alanı
            Panel resultPanel = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(610, 100),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 249, 255)
            };

            var lblTotalInfo = new Label
            {
                Text = "Toplam Maaş:",
                Location = new Point(20, 40),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };
            resultPanel.Controls.Add(lblTotalInfo);

            lblTotalSalary = new Label
            {
                Text = "0,00 ₺",
                Location = new Point(230, 40),
                Size = new Size(350, 30),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 212)
            };
            resultPanel.Controls.Add(lblTotalSalary);

            formContainer.Controls.Add(resultPanel);

            // Panelleri ekle
            rightPanel.Controls.Add(formContainer);
            this.Controls.Add(rightPanel);
            this.Controls.Add(leftPanel);
            
            // Form boyutu değiştiğinde panellerin tekrar düzenlenmesi
            this.Resize += (s, e) => {
                leftPanel.Width = this.ClientSize.Width / 3;
                formContainer.Location = new Point(
                    (rightPanel.ClientSize.Width - formContainer.Width) / 2,
                    (rightPanel.ClientSize.Height - formContainer.Height) / 2);
            };
        }

        private void LoadUserData()
        {
            // Kullanıcının mevcut maaş bilgilerini yükle
            var personalInfo = DataStorage.LoadPersonalInfo()
                .FirstOrDefault(p => p.UserId == currentUser.UserId);

            if (personalInfo != null)
            {
                // EducationLevel, ResponsibilityLevel gibi alanlar artık string olduğundan
                // bunlara karşılık gelen ComboBox seçeneklerini seçelim
                if (!string.IsNullOrEmpty(personalInfo.EducationLevel))
                {
                    int index = cmbEducation.Items.IndexOf(personalInfo.EducationLevel);
                    if (index >= 0)
                        cmbEducation.SelectedIndex = index;
                }

                if (!string.IsNullOrEmpty(personalInfo.ResponsibilityLevel))
                {
                    int index = cmbPosition.Items.IndexOf(personalInfo.ResponsibilityLevel);
                    if (index >= 0)
                        cmbPosition.SelectedIndex = index;
                }

                // Diğer ComboBox'lar için de benzer şekilde yükleme yapılabilir
                // Bu örnekte sadece EducationLevel ve ResponsibilityLevel yüklendi
            }
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            decimal totalFactor = 0;
            
            // Aile durumu faktörü
            if (cmbFamilyStatus.SelectedIndex > 0)
            {
                string selectedFamily = cmbFamilyStatus.SelectedItem.ToString();
                totalFactor += familyFactors[selectedFamily];
            }
            
            // Deneyim faktörü
            if (cmbExperience.SelectedIndex > 0)
            {
                string selectedExperience = cmbExperience.SelectedItem.ToString();
                totalFactor += experienceFactors[selectedExperience];
            }
            
            // Lokasyon faktörü
            if (cmbLocation.SelectedIndex > 0)
            {
                string selectedLocation = cmbLocation.SelectedItem.ToString();
                totalFactor += locationFactors[selectedLocation];
            }
            
            // Eğitim faktörü
            if (cmbEducation.SelectedIndex > 0)
            {
                string selectedEducation = cmbEducation.SelectedItem.ToString();
                totalFactor += educationFactors[selectedEducation];
            }
            
            // Dil faktörü
            if (cmbLanguage.SelectedIndex > 0)
            {
                string selectedLanguage = cmbLanguage.SelectedItem.ToString();
                totalFactor += languageFactors[selectedLanguage];
            }
            
            // Pozisyon faktörü
            if (cmbPosition.SelectedIndex > 0)
            {
                string selectedPosition = cmbPosition.SelectedItem.ToString();
                totalFactor += positionFactors[selectedPosition];
            }

            // Toplam faktör ile maaşı hesapla
            decimal totalSalary = BASE_SALARY * (1 + totalFactor);

            lblBaseSalary.Text = $"Taban Maaş: {BASE_SALARY:N0} TL";
            lblTotalSalary.Text = $"Toplam Maaş: {totalSalary:N0} TL (Katsayı: {totalFactor:F2})";

            // Maaş bilgilerini kaydet
            var personalInfo = DataStorage.LoadPersonalInfo()
                .FirstOrDefault(p => p.UserId == currentUser.UserId);

            if (personalInfo != null)
            {
                // Seçimleri kaydet
                if (cmbEducation.SelectedIndex > 0)
                    personalInfo.EducationLevel = cmbEducation.SelectedItem.ToString();

                if (cmbPosition.SelectedIndex > 0)
                    personalInfo.ResponsibilityLevel = cmbPosition.SelectedItem.ToString();

                // Diğer bilgileri de saklamak istiyorsanız benzer şekilde kaydedin
                
                // Maaş bilgisini kaydet
                personalInfo.Salary = totalSalary;

                var personalInfoList = DataStorage.LoadPersonalInfo();
                var index = personalInfoList.FindIndex(p => p.UserId == currentUser.UserId);
                if (index != -1)
                {
                    personalInfoList[index] = personalInfo;
                    DataStorage.SavePersonalInfo(personalInfoList);
                }
            }
        }
    }
}