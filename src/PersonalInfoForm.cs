using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace PersonalOrganizer
{
    public class PersonalInfoForm : Form
    {
        private TextBox txtName;
        private TextBox txtSurname;
        private MaskedTextBox txtPhone;
        private TextBox txtAddress;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private MaskedTextBox txtBirthDate;
        private TextBox txtCity;
        private TextBox txtCountry;
        private TextBox txtEmergencyContact;
        private TextBox txtMedicalInfo;
        private PictureBox picProfile;
        private Button btnSave;
        private Button btnChangePhoto;
        private Button btnChangePassword;
        private User currentUser;
        private PersonalInfo currentInfo;
        private Stack<PersonalInfo> undoStack;
        private Stack<PersonalInfo> redoStack;
        private List<PersonalInfo> personalInfoList;
        private PersonalInfo selectedInfo;

        public PersonalInfoForm(User user)
        {
            currentUser = user;
            undoStack = new Stack<PersonalInfo>();
            redoStack = new Stack<PersonalInfo>();
            personalInfoList = new List<PersonalInfo>();
            InitializeComponents();
            
            // Form yüklendiğinde odaklanan kontrolden odağı kaldır
            this.Load += PersonalInfoForm_Load;
            
            LoadPersonalInfo();
        }

        private void PersonalInfoForm_Load(object sender, EventArgs e)
        {
            // Form yüklendiğinde hiçbir kontrolün odakta olmamasını sağla
            this.ActiveControl = null;
        }

        private void InitializeComponents()
        {
            this.Text = "Kişisel Bilgiler - Kişisel Organizatör";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1000, 700);
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.FormClosing += PersonalInfoForm_FormClosing;
            this.KeyPreview = true;
            this.KeyDown += PersonalInfoForm_KeyDown;

            // Sol panel (logo/banner alanı)
            Panel leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = this.Width / 3,
                BackColor = Color.FromArgb(41, 128, 185), // Modern mavi
                Padding = new Padding(15)
            };

            // Banner başlık
            Label lblAppName = new Label
            {
                Text = "Kişisel Bilgiler",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(0, 50, 0, 0)
            };
            leftPanel.Controls.Add(lblAppName);

            // Alt başlık
            Label lblSubtitle = new Label
            {
                Text = "Kendinizle ilgili önemli bilgileri burada güncelleyebilirsiniz.",
                Font = new Font("Segoe UI Light", 14, FontStyle.Regular),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(10, 10, 10, 30)
            };
            leftPanel.Controls.Add(lblSubtitle);

            // Profil fotoğrafı
            picProfile = new PictureBox
            {
                Size = new Size(180, 180),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point((leftPanel.Width - 180) / 2, 220)
            };
            
            // Profil resmi için yuvarlak kenar efekti
            picProfile.Paint += (s, e) => {
                System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                gp.AddEllipse(0, 0, picProfile.Width - 1, picProfile.Height - 1);
                picProfile.Region = new Region(gp);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.DrawEllipse(new Pen(Color.FromArgb(52, 152, 219), 2), 0, 0, picProfile.Width - 1, picProfile.Height - 1);
            };
            
            leftPanel.Controls.Add(picProfile);

            // Fotoğraf değiştir butonu
            btnChangePhoto = new Button
            {
                Text = "Fotoğraf Değiştir",
                Size = new Size(180, 40),
                Location = new Point((leftPanel.Width - 180) / 2, 410),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnChangePhoto.FlatAppearance.BorderSize = 0;
            btnChangePhoto.Click += BtnChangePhoto_Click;
            leftPanel.Controls.Add(btnChangePhoto);

            // Açıklama metni
            Label lblDescription = new Label
            {
                Text = "Kişisel bilgilerinizi güncelleyin ve kolayca yönetin.",
                Font = new Font("Segoe UI Light", 16, FontStyle.Regular),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Bottom,
                Height = 100,
                Padding = new Padding(20, 0, 20, 20)
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
                Width = 800,
                Height = 650,
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
                Text = "Kişisel Bilgileriniz",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(0, 0),
                Size = new Size(700, 50)
            };
            formContainer.Controls.Add(lblTitle);

            // Bilgi formu alanı
            Panel infoPanel = new Panel
            {
                Location = new Point(0, 60),
                Size = new Size(800, 500),
                BackColor = Color.White
            };

            // Sol kolon - Temel bilgiler
            int yPos = 0;
            int xPosLeft = 0;
            int labelWidth = 150;
            int controlWidth = 230;
            int controlHeight = 35;
            int margin = 20;

            // Temel bilgiler başlığı
            Label lblBasicInfo = new Label
            {
                Text = "Temel Bilgiler",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(xPosLeft, yPos),
                Size = new Size(labelWidth + controlWidth, 30)
            };
            infoPanel.Controls.Add(lblBasicInfo);
            yPos += 40;

            // İsim alanı
            Label lblName = new Label
            {
                Text = "Adınız:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(xPosLeft, yPos),
                Size = new Size(labelWidth, controlHeight)
            };
            infoPanel.Controls.Add(lblName);

            txtName = new TextBox
            {
                Location = new Point(xPosLeft + labelWidth, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Adınızı girin",
                ForeColor = Color.Gray
            };
            txtName.Enter += TextBox_Enter;
            txtName.Leave += TextBox_Leave;
            infoPanel.Controls.Add(txtName);
            yPos += controlHeight + margin;

            // Soyisim alanı
            Label lblSurname = new Label
            {
                Text = "Soyadınız:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(xPosLeft, yPos),
                Size = new Size(labelWidth, controlHeight)
            };
            infoPanel.Controls.Add(lblSurname);

            txtSurname = new TextBox
            {
                Location = new Point(xPosLeft + labelWidth, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Soyadınızı girin",
                ForeColor = Color.Gray
            };
            txtSurname.Enter += TextBox_Enter;
            txtSurname.Leave += TextBox_Leave;
            infoPanel.Controls.Add(txtSurname);
            yPos += controlHeight + margin;

            // E-posta alanı
            Label lblEmail = new Label
            {
                Text = "E-posta Adresi:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(xPosLeft, yPos),
                Size = new Size(labelWidth, controlHeight)
            };
            infoPanel.Controls.Add(lblEmail);

            txtEmail = new TextBox
            {
                Location = new Point(xPosLeft + labelWidth, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "E-posta adresinizi girin",
                ForeColor = Color.Gray
            };
            txtEmail.Enter += TextBox_Enter;
            txtEmail.Leave += TextBox_Leave;
            infoPanel.Controls.Add(txtEmail);
            yPos += controlHeight + margin;

            // Şifre alanı
            Label lblPassword = new Label
            {
                Text = "Şifre:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(xPosLeft, yPos),
                Size = new Size(labelWidth, controlHeight)
            };
            infoPanel.Controls.Add(lblPassword);

            txtPassword = new TextBox
            {
                Location = new Point(xPosLeft + labelWidth, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Şifrenizi girin",
                PasswordChar = '*',
                ForeColor = Color.Gray
            };
            txtPassword.Enter += TextBox_Enter;
            txtPassword.Leave += TextBox_Leave;
            infoPanel.Controls.Add(txtPassword);

            btnChangePassword = new Button
            {
                Text = "Değiştir",
                Location = new Point(xPosLeft + labelWidth + controlWidth + 10, yPos),
                Size = new Size(90, controlHeight),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnChangePassword.FlatAppearance.BorderSize = 0;
            btnChangePassword.Click += BtnChangePassword_Click;
            infoPanel.Controls.Add(btnChangePassword);
            yPos += controlHeight + margin;

            // Doğum tarihi alanı
            Label lblBirthDate = new Label
            {
                Text = "Doğum Tarihi:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(xPosLeft, yPos),
                Size = new Size(labelWidth, controlHeight)
            };
            infoPanel.Controls.Add(lblBirthDate);

            txtBirthDate = new MaskedTextBox
            {
                Location = new Point(xPosLeft + labelWidth, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Mask = "00/00/0000",
                PromptChar = '_',
                Text = "__/__/____",
                ForeColor = Color.Gray
            };
            txtBirthDate.Enter += TextBox_Enter;
            txtBirthDate.Leave += TextBox_Leave;
            infoPanel.Controls.Add(txtBirthDate);
            yPos += controlHeight + margin;

            // Telefon alanı
            Label lblPhone = new Label
            {
                Text = "Telefon:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(xPosLeft, yPos),
                Size = new Size(labelWidth, controlHeight)
            };
            infoPanel.Controls.Add(lblPhone);

            txtPhone = new MaskedTextBox
            {
                Location = new Point(xPosLeft + labelWidth, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Mask = "(999) 000-0000",
                PromptChar = '_',
                Text = "(___) ___-____",
                ForeColor = Color.Gray
            };
            txtPhone.Enter += TextBox_Enter;
            txtPhone.Leave += TextBox_Leave;
            infoPanel.Controls.Add(txtPhone);

            // Sağ kolon - Adres ve Acil Durum
            int xPosRight = 430;
            yPos = 0;

            // Adres bilgileri başlığı
            Label lblAddressInfo = new Label
            {
                Text = "Adres Bilgileri",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(xPosRight, yPos),
                Size = new Size(labelWidth + controlWidth, 30)
            };
            infoPanel.Controls.Add(lblAddressInfo);
            yPos += 40;

            // Şehir alanı
            Label lblCity = new Label
            {
                Text = "Şehir:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(xPosRight, yPos),
                Size = new Size(labelWidth, controlHeight)
            };
            infoPanel.Controls.Add(lblCity);

            txtCity = new TextBox
            {
                Location = new Point(xPosRight + labelWidth, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Şehrinizi girin",
                ForeColor = Color.Gray
            };
            txtCity.Enter += TextBox_Enter;
            txtCity.Leave += TextBox_Leave;
            infoPanel.Controls.Add(txtCity);
            yPos += controlHeight + margin;

            // Ülke alanı
            Label lblCountry = new Label
            {
                Text = "Ülke:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(xPosRight, yPos),
                Size = new Size(labelWidth, controlHeight)
            };
            infoPanel.Controls.Add(lblCountry);

            txtCountry = new TextBox
            {
                Location = new Point(xPosRight + labelWidth, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Ülkenizi girin",
                ForeColor = Color.Gray
            };
            txtCountry.Enter += TextBox_Enter;
            txtCountry.Leave += TextBox_Leave;
            infoPanel.Controls.Add(txtCountry);
            yPos += controlHeight + margin;

            // Adres alanı
            Label lblAddress = new Label
            {
                Text = "Adres:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(xPosRight, yPos),
                Size = new Size(labelWidth, controlHeight)
            };
            infoPanel.Controls.Add(lblAddress);

            txtAddress = new TextBox
            {
                Location = new Point(xPosRight + labelWidth, yPos),
                Size = new Size(controlWidth, controlHeight * 3 + margin * 2),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true,
                Text = "Adresinizi girin",
                ForeColor = Color.Gray
            };
            txtAddress.Enter += TextBox_Enter;
            txtAddress.Leave += TextBox_Leave;
            infoPanel.Controls.Add(txtAddress);
            yPos += (controlHeight * 3 + margin * 3);

            // Acil durum başlığı
            Label lblEmergency = new Label
            {
                Text = "Acil Durum Bilgileri",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(xPosRight, yPos),
                Size = new Size(labelWidth + controlWidth, 30)
            };
            infoPanel.Controls.Add(lblEmergency);
            yPos += 40;

            // Acil durum kontağı
            Label lblEmergencyContact = new Label
            {
                Text = "Acil Durum Kontağı:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(xPosRight, yPos),
                Size = new Size(labelWidth, controlHeight)
            };
            infoPanel.Controls.Add(lblEmergencyContact);

            txtEmergencyContact = new TextBox
            {
                Location = new Point(xPosRight + labelWidth, yPos),
                Size = new Size(controlWidth, controlHeight),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Acil durum kişisi ve telefon",
                ForeColor = Color.Gray
            };
            txtEmergencyContact.Enter += TextBox_Enter;
            txtEmergencyContact.Leave += TextBox_Leave;
            infoPanel.Controls.Add(txtEmergencyContact);
            yPos += controlHeight + margin;

            // Sağlık bilgisi
            Label lblMedicalInfo = new Label
            {
                Text = "Sağlık Bilgisi:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(xPosRight, yPos),
                Size = new Size(labelWidth, controlHeight)
            };
            infoPanel.Controls.Add(lblMedicalInfo);

            txtMedicalInfo = new TextBox
            {
                Location = new Point(xPosRight + labelWidth, yPos),
                Size = new Size(controlWidth, controlHeight * 2),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true,
                Text = "Önemli sağlık bilgilerinizi girin",
                ForeColor = Color.Gray
            };
            txtMedicalInfo.Enter += TextBox_Enter;
            txtMedicalInfo.Leave += TextBox_Leave;
            infoPanel.Controls.Add(txtMedicalInfo);

            formContainer.Controls.Add(infoPanel);

            // Kaydet butonu
            btnSave = new Button
            {
                Text = "Bilgileri Kaydet",
                Location = new Point(formContainer.Width - 200, 570),
                Size = new Size(200, 45),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            formContainer.Controls.Add(btnSave);

            // Panelleri ekle
            rightPanel.Controls.Add(formContainer);
            this.Controls.Add(rightPanel);
            this.Controls.Add(leftPanel);
            
            // Form boyutu değiştiğinde panellerin tekrar düzenlenmesi
            this.Resize += (s, e) => {
                leftPanel.Width = this.ClientSize.Width / 3;
                // Profil resmini yeniden yerleştir
                picProfile.Location = new Point((leftPanel.Width - 180) / 2, 220);
                btnChangePhoto.Location = new Point((leftPanel.Width - 180) / 2, 410);
                
                formContainer.Location = new Point(
                    (rightPanel.ClientSize.Width - formContainer.Width) / 2,
                    (rightPanel.ClientSize.Height - formContainer.Height) / 2);
            };
        }

        private void PersonalInfoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Çıkmak istediğinizden emin misiniz?", 
                    "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void PersonalInfoForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                Undo();
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                Redo();
            }
        }

        private void BtnChangePhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Image image = Image.FromFile(openFileDialog.FileName);
                        
                        // Resmi boyutlandır - büyük resimler Base64 dönüşümünde sorun çıkarabilir
                        int maxWidth = 800;
                        int maxHeight = 800;
                        
                        if (image.Width > maxWidth || image.Height > maxHeight)
                        {
                            double ratioX = (double)maxWidth / image.Width;
                            double ratioY = (double)maxHeight / image.Height;
                            double ratio = Math.Min(ratioX, ratioY);
                            
                            int newWidth = (int)(image.Width * ratio);
                            int newHeight = (int)(image.Height * ratio);
                            
                            Bitmap resizedImage = new Bitmap(newWidth, newHeight);
                            using (Graphics g = Graphics.FromImage(resizedImage))
                            {
                                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                g.DrawImage(image, 0, 0, newWidth, newHeight);
                            }
                            
                            image.Dispose();
                            image = resizedImage;
                        }
                        
                        picProfile.Image = image;
                        SaveToUndoStack();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Resim yüklenirken hata oluştu: {ex.Message}", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            using (PasswordChangeForm passwordForm = new PasswordChangeForm())
            {
                if (passwordForm.ShowDialog() == DialogResult.OK)
                {
                    txtPassword.Text = passwordForm.NewPassword;
                    SaveToUndoStack();
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Varsayılan/placeholder metinler kaydedilmeyecek
                bool hasName = txtName.ForeColor != System.Drawing.Color.Gray && !string.IsNullOrWhiteSpace(txtName.Text);
                bool hasSurname = txtSurname.ForeColor != System.Drawing.Color.Gray && !string.IsNullOrWhiteSpace(txtSurname.Text);
                
                // Ad ve soyad kontrolü
                if (!hasName || !hasSurname)
                {
                    MessageBox.Show("Lütfen ad ve soyad alanlarını doldurunuz.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ad ve soyad sadece harf kontrolü
                if (!txtName.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)) || 
                    !txtSurname.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    MessageBox.Show("Ad ve soyad sadece harflerden oluşmalıdır.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Telefon numarası kontrolü
                string phoneText = txtPhone.ForeColor != System.Drawing.Color.Gray ? txtPhone.Text : "";
                string phoneDigits = new string(phoneText.Where(char.IsDigit).ToArray());
                if (!string.IsNullOrWhiteSpace(phoneText) && phoneDigits.Length < 10)
                {
                    MessageBox.Show("Lütfen geçerli bir telefon numarası giriniz.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // E-posta kontrolü
                string emailText = txtEmail.ForeColor != System.Drawing.Color.Gray ? txtEmail.Text : "";
                if (!string.IsNullOrWhiteSpace(emailText) && !ValidationHelper.IsValidEmail(emailText))
                {
                    MessageBox.Show("Lütfen geçerli bir e-posta adresi giriniz.", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Doğum tarihi kontrolü
                string birthDateText = txtBirthDate.ForeColor != System.Drawing.Color.Gray ? txtBirthDate.Text : "";
                DateTime birthDate = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(birthDateText) && !DateTime.TryParse(birthDateText, out birthDate))
                {
                    MessageBox.Show("Lütfen geçerli bir doğum tarihi giriniz (GG.AA.YYYY).", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var info = new PersonalInfo
                {
                    Id = currentInfo?.Id ?? 0,
                    UserId = currentUser.UserId,
                    Username = currentUser.Username,
                    Name = hasName ? txtName.Text : "",
                    Surname = hasSurname ? txtSurname.Text : "",
                    Phone = txtPhone.ForeColor != System.Drawing.Color.Gray ? txtPhone.Text : "",
                    PhoneNumber = txtPhone.ForeColor != System.Drawing.Color.Gray ? txtPhone.Text : "",
                    Address = txtAddress.ForeColor != System.Drawing.Color.Gray ? txtAddress.Text : "",
                    Email = txtEmail.ForeColor != System.Drawing.Color.Gray ? txtEmail.Text : "",
                    BirthDate = txtBirthDate.ForeColor != System.Drawing.Color.Gray ? birthDate : DateTime.Now,
                    City = txtCity.ForeColor != System.Drawing.Color.Gray ? txtCity.Text : "",
                    Country = txtCountry.ForeColor != System.Drawing.Color.Gray ? txtCountry.Text : "",
                    EmergencyContact = txtEmergencyContact.ForeColor != System.Drawing.Color.Gray ? txtEmergencyContact.Text : "",
                    MedicalInfo = txtMedicalInfo.ForeColor != System.Drawing.Color.Gray ? txtMedicalInfo.Text : "",
                    ProfilePhotoBase64 = picProfile.Image != null ? PersonalInfo.ImageToBase64(picProfile.Image) : currentInfo?.ProfilePhotoBase64 ?? "",
                    CreatedDate = currentInfo?.CreatedDate ?? DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                var personalInfoList = DataStorage.LoadPersonalInfo();
                var existingInfo = personalInfoList.FirstOrDefault(p => p.UserId == currentUser.UserId);

                if (existingInfo != null)
                {
                    personalInfoList.Remove(existingInfo);
                }

                personalInfoList.Add(info);
                DataStorage.SavePersonalInfo(personalInfoList);
                currentInfo = info;

                MessageBox.Show("Kişisel bilgileriniz başarıyla kaydedildi.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kişisel bilgiler kaydedilirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPersonalInfo()
        {
            var personalInfoList = DataStorage.LoadPersonalInfo();
            currentInfo = personalInfoList.FirstOrDefault(p => p.UserId == currentUser.UserId);

            if (currentInfo == null)
            {
                currentInfo = new PersonalInfo
                {
                    Id = 0,
                    UserId = currentUser.UserId,
                    Username = currentUser.Username,
                    Name = "",
                    Surname = "",
                    Phone = "",
                    PhoneNumber = "",
                    Address = "",
                    Email = "",
                    BirthDate = DateTime.Now,
                    City = "",
                    Country = "",
                    EmergencyContact = "",
                    MedicalInfo = "",
                    ProfilePhotoBase64 = ""
                };
                
                // Yeni kayıt için placeholder metinleri göster
                ShowPlaceholders();
            }
            else
            {
                // Mevcut kayıt için gerçek verileri göster
                ShowRealData();
            }

            // Profil fotoğrafını yükle
            if (!string.IsNullOrEmpty(currentInfo.ProfilePhotoBase64))
            {
                try
                {
                    picProfile.Image = PersonalInfo.Base64ToImage(currentInfo.ProfilePhotoBase64);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Profil fotoğrafı yüklenirken hata oluştu: {ex.Message}", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void ShowPlaceholders()
        {
            // TextBox'lar için placeholder metinleri göster
            txtName.Text = "Adınızı girin";
            txtName.ForeColor = System.Drawing.Color.Gray;
            
            txtSurname.Text = "Soyadınızı girin";
            txtSurname.ForeColor = System.Drawing.Color.Gray;
            
            txtPhone.Text = "(5__) ___-____";
            txtPhone.ForeColor = System.Drawing.Color.Gray;
            
            txtAddress.Text = "Adresinizi girin";
            txtAddress.ForeColor = System.Drawing.Color.Gray;
            
            txtEmail.Text = "E-posta adresinizi girin";
            txtEmail.ForeColor = System.Drawing.Color.Gray;
            
            txtPassword.Text = "Şifrenizi girin";
            txtPassword.PasswordChar = '\0';
            txtPassword.ForeColor = System.Drawing.Color.Gray;
            
            txtBirthDate.Text = "__.__.____";
            txtBirthDate.ForeColor = System.Drawing.Color.Gray;
            
            txtCity.Text = "Şehir giriniz";
            txtCity.ForeColor = System.Drawing.Color.Gray;
            
            txtCountry.Text = "Ülke giriniz";
            txtCountry.ForeColor = System.Drawing.Color.Gray;
            
            txtEmergencyContact.Text = "Ad Soyad - Telefon";
            txtEmergencyContact.ForeColor = System.Drawing.Color.Gray;
            
            txtMedicalInfo.Text = "Kan grubu, alerji, vs.";
            txtMedicalInfo.ForeColor = System.Drawing.Color.Gray;
        }
        
        private void ShowRealData()
        {
            // Gerçek form verilerini göster ve renkleri normal yap
            if (!string.IsNullOrEmpty(currentInfo.Name))
            {
                txtName.Text = currentInfo.Name;
                txtName.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                txtName.Text = "Adınızı girin";
                txtName.ForeColor = System.Drawing.Color.Gray;
            }
            
            if (!string.IsNullOrEmpty(currentInfo.Surname))
            {
                txtSurname.Text = currentInfo.Surname;
                txtSurname.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                txtSurname.Text = "Soyadınızı girin";
                txtSurname.ForeColor = System.Drawing.Color.Gray;
            }
            
            if (!string.IsNullOrEmpty(currentInfo.Phone))
            {
                txtPhone.Text = currentInfo.Phone;
                txtPhone.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                txtPhone.Text = "(5__) ___-____";
                txtPhone.ForeColor = System.Drawing.Color.Gray;
            }
            
            if (!string.IsNullOrEmpty(currentInfo.Address))
            {
                txtAddress.Text = currentInfo.Address;
                txtAddress.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                txtAddress.Text = "Adresinizi girin";
                txtAddress.ForeColor = System.Drawing.Color.Gray;
            }
            
            if (!string.IsNullOrEmpty(currentInfo.Email))
            {
                txtEmail.Text = currentInfo.Email;
                txtEmail.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                txtEmail.Text = "E-posta adresinizi girin";
                txtEmail.ForeColor = System.Drawing.Color.Gray;
            }
            
            if (!string.IsNullOrEmpty(currentInfo.Password))
            {
                txtPassword.Text = currentInfo.Password;
                txtPassword.PasswordChar = '*';
                txtPassword.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                txtPassword.Text = "Şifrenizi girin";
                txtPassword.PasswordChar = '\0';
                txtPassword.ForeColor = System.Drawing.Color.Gray;
            }
            
            if (currentInfo.BirthDate != DateTime.MinValue)
            {
                txtBirthDate.Text = currentInfo.BirthDate.ToString("dd.MM.yyyy");
                txtBirthDate.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                txtBirthDate.Text = "__.__.____";
                txtBirthDate.ForeColor = System.Drawing.Color.Gray;
            }
            
            if (!string.IsNullOrEmpty(currentInfo.City))
            {
                txtCity.Text = currentInfo.City;
                txtCity.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                txtCity.Text = "Şehir giriniz";
                txtCity.ForeColor = System.Drawing.Color.Gray;
            }
            
            if (!string.IsNullOrEmpty(currentInfo.Country))
            {
                txtCountry.Text = currentInfo.Country;
                txtCountry.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                txtCountry.Text = "Ülke giriniz";
                txtCountry.ForeColor = System.Drawing.Color.Gray;
            }
            
            if (!string.IsNullOrEmpty(currentInfo.EmergencyContact))
            {
                txtEmergencyContact.Text = currentInfo.EmergencyContact;
                txtEmergencyContact.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                txtEmergencyContact.Text = "Ad Soyad - Telefon";
                txtEmergencyContact.ForeColor = System.Drawing.Color.Gray;
            }
            
            if (!string.IsNullOrEmpty(currentInfo.MedicalInfo))
            {
                txtMedicalInfo.Text = currentInfo.MedicalInfo;
                txtMedicalInfo.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                txtMedicalInfo.Text = "Kan grubu, alerji, vs.";
                txtMedicalInfo.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void SaveToUndoStack()
        {
            PersonalInfo copy = new PersonalInfo(
                currentInfo.Id,
                currentInfo.UserId.ToString(),
                currentInfo.Username)
            {
                Name = currentInfo.Name,
                Surname = currentInfo.Surname,
                PhoneNumber = currentInfo.PhoneNumber,
                Address = currentInfo.Address,
                Email = currentInfo.Email,
                Password = currentInfo.Password,
                ProfilePhotoBase64 = currentInfo.ProfilePhotoBase64
            };
            undoStack.Push(copy);
            redoStack.Clear();
        }

        private void Undo()
        {
            if (undoStack.Count > 0)
            {
                PersonalInfo previousState = undoStack.Pop();
                redoStack.Push(new PersonalInfo(
                    currentInfo.Id,
                    currentInfo.UserId.ToString(),
                    currentInfo.Username)
                {
                    Name = currentInfo.Name,
                    Surname = currentInfo.Surname,
                    PhoneNumber = currentInfo.PhoneNumber,
                    Address = currentInfo.Address,
                    Email = currentInfo.Email,
                    Password = currentInfo.Password,
                    ProfilePhotoBase64 = currentInfo.ProfilePhotoBase64
                });

                currentInfo = previousState;
                UpdateUI();
            }
        }

        private void Redo()
        {
            if (redoStack.Count > 0)
            {
                PersonalInfo nextState = redoStack.Pop();
                undoStack.Push(new PersonalInfo(
                    currentInfo.Id,
                    currentInfo.UserId.ToString(),
                    currentInfo.Username)
                {
                    Name = currentInfo.Name,
                    Surname = currentInfo.Surname,
                    PhoneNumber = currentInfo.PhoneNumber,
                    Address = currentInfo.Address,
                    Email = currentInfo.Email,
                    Password = currentInfo.Password,
                    ProfilePhotoBase64 = currentInfo.ProfilePhotoBase64
                });

                currentInfo = nextState;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            // Geri alma ve ileri alma işlemleri için UI'ı güncelle
            if (currentInfo != null)
            {
                ShowRealData();
                
                // Profil fotoğrafını güncelle
                if (!string.IsNullOrEmpty(currentInfo.ProfilePhotoBase64))
                {
                    try
                    {
                        picProfile.Image = PersonalInfo.Base64ToImage(currentInfo.ProfilePhotoBase64);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Profil fotoğrafını yüklerken hata: {ex.Message}");
                    }
                }
                else
                {
                    picProfile.Image = null;
                }
            }
        }

        // Placeholder textbox'a tıklandığında içeriği silmek için
        private void TextBox_Enter(object sender, EventArgs e)
        {
            // Sadece gri renk olan placeholder metinleri temizle
            if (sender is TextBox textBox && textBox.ForeColor == System.Drawing.Color.Gray)
            {
                textBox.Text = "";
                textBox.ForeColor = System.Drawing.Color.Black;
                
   
            }
            else if (sender is MaskedTextBox maskedTextBox && maskedTextBox.ForeColor == System.Drawing.Color.Gray)
            {
                maskedTextBox.Clear();
                maskedTextBox.ForeColor = System.Drawing.Color.Black;
            }
        }

        // Placeholder textbox'tan çıkıldığında ve içerik boşsa placeholder metni göstermek için
        private void TextBox_Leave(object sender, EventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
            {
                SetPlaceholderForTextBox(textBox);
            }
            else if (sender is MaskedTextBox maskedTextBox && !maskedTextBox.MaskCompleted && string.IsNullOrWhiteSpace(new string(maskedTextBox.Text.Where(c => !char.IsWhiteSpace(c) && c != '_' && c != '(' && c != ')' && c != '-' && c != '.').ToArray())))
            {
                SetPlaceholderForMaskedTextBox(maskedTextBox);
            }
        }
        
        private void SetPlaceholderForTextBox(TextBox textBox)
        {
            textBox.ForeColor = System.Drawing.Color.Gray;
            
            if (textBox.Name == txtName.Name)
            {
                textBox.Text = "Adınızı girin";
            }
            else if (textBox.Name == txtSurname.Name)
            {
                textBox.Text = "Soyadınızı girin";
            }
            else if (textBox.Name == txtAddress.Name)
            {
                textBox.Text = "Adresinizi girin";
            }
            else if (textBox.Name == txtEmail.Name)
            {
                textBox.Text = "E-posta adresinizi girin";
            }
            else if (textBox.Name == txtPassword.Name)
            {
                textBox.PasswordChar = '\0'; // Şifre karakterini kapat
                textBox.Text = "Şifrenizi girin";
            }
            else if (textBox.Name == txtCity.Name)
            {
                textBox.Text = "Şehir giriniz";
            }
            else if (textBox.Name == txtCountry.Name)
            {
                textBox.Text = "Ülke giriniz";
            }
            else if (textBox.Name == txtEmergencyContact.Name)
            {
                textBox.Text = "Ad Soyad - Telefon";
            }
            else if (textBox.Name == txtMedicalInfo.Name)
            {
                textBox.Text = "Kan grubu, alerji, vs.";
            }
        }
        
        private void SetPlaceholderForMaskedTextBox(MaskedTextBox maskedTextBox)
        {
            maskedTextBox.ForeColor = System.Drawing.Color.Gray;
            
            if (maskedTextBox.Name == txtPhone.Name)
            {
                maskedTextBox.Text = "(5__) ___-____";
            }
            else if (maskedTextBox.Name == txtBirthDate.Name)
            {
                maskedTextBox.Text = "__.__.____";
            }
        }

        // Gri renkli placeholder metinleri temizle
        private void ClearPlaceholderTexts()
        {
            ClearPlaceholderTextsRecursive(this.Controls);
        }
        
        private void ClearPlaceholderTextsRecursive(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is TextBox textBox && textBox.ForeColor == System.Drawing.Color.Gray)
                {
                    textBox.Text = "";
                }
                else if (control is MaskedTextBox maskedTextBox && maskedTextBox.ForeColor == System.Drawing.Color.Gray)
                {
                    maskedTextBox.Text = "";
                }
                
                // Alt kontrolleri de kontrol et
                if (control.HasChildren)
                {
                    ClearPlaceholderTextsRecursive(control.Controls);
                }
            }
        }
    }

    public class PasswordChangeForm : Form
    {
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnOK;
        private Button btnCancel;

        public string NewPassword { get; private set; }

        public PasswordChangeForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Şifre Değiştir";
            this.Size = new System.Drawing.Size(400, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 10);
            
            // Form başlığı
            Label lblTitle = new Label
            {
                Text = "Şifre Değiştirme",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(20, 20),
                Size = new Size(360, 35)
            };
            
            // Yeni şifre etiketi
            Label lblNewPassword = new Label
            {
                Text = "Yeni Şifre:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 70),
                Size = new Size(150, 25)
            };
            
            // Yeni şifre alanı
            txtNewPassword = new TextBox
            {
                Location = new Point(20, 95),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 11),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // Şifre onay etiketi
            Label lblConfirmPassword = new Label
            {
                Text = "Şifre Onay:",
                Font = new Font("Segoe UI", 11),
                Location = new Point(20, 135),
                Size = new Size(150, 25)
            };
            
            // Şifre onay alanı
            txtConfirmPassword = new TextBox
            {
                Location = new Point(20, 160),
                Size = new Size(360, 30),
                Font = new Font("Segoe UI", 11),
                PasswordChar = '*',
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // Butonlar
            btnOK = new Button
            {
                Text = "Tamam",
                Location = new Point(180, 200),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK,
                Cursor = Cursors.Hand
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += BtnOK_Click;
            
            btnCancel = new Button
            {
                Text = "İptal",
                Location = new Point(290, 200),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(189, 195, 199),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            
            this.Controls.AddRange(new Control[] { 
                lblTitle, 
                lblNewPassword, txtNewPassword, 
                lblConfirmPassword, txtConfirmPassword, 
                btnOK, btnCancel 
            });
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                MessageBox.Show("Lütfen yeni şifreyi girin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Şifreler eşleşmiyor!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            NewPassword = txtNewPassword.Text;
        }
    }
} 