using System;
using System.Drawing;
using System.Windows.Forms;

namespace PersonalOrganizer
{
    public class ForgotPasswordForm : Form
    {
        private TextBox txtEmail;
        private Button btnSendCode;
        private Label lblStatus;
        
        // Şifre sıfırlama işlemindeki kullanıcı
        public User FoundUser { get; private set; }
        
        // Oluşturulan sıfırlama kodu
        public string ResetCode { get; private set; }

        public ForgotPasswordForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Şifremi Unuttum - Kişisel Organizatör";
            this.Size = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
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
                Text = "Şifre Sıfırlama",
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
                Text = "Şifrenizi mi unuttunuz?\n\nEndişelenmeyin, hesabınıza bağlı\ne-posta adresinize bir sıfırlama\nkodu göndereceğiz.",
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
                Padding = new Padding(60)
            };

            // Form container
            Panel formContainer = new Panel
            {
                Width = 450,
                Height = 400,
                Anchor = AnchorStyles.None,
                BackColor = Color.White
            };
            
            // Panel'ı ortala
            formContainer.Location = new Point(
                (rightPanel.ClientSize.Width - formContainer.Width) / 2,
                (rightPanel.ClientSize.Height - formContainer.Height) / 2);

            // Form başlığı
            Label lblTitle = new Label
            {
                Text = "Hesabınızı Doğrulayın",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(0, 0),
                Size = new Size(450, 40)
            };
            formContainer.Controls.Add(lblTitle);

            // Açıklama etiketi
            Label lblInstruction = new Label
            {
                Text = "Lütfen hesabınıza ait e-posta adresini girin.\nŞifre sıfırlama kodu bu adrese gönderilecektir.",
                Font = new Font("Segoe UI", 11),
                Location = new Point(0, 50),
                Size = new Size(450, 50)
            };
            formContainer.Controls.Add(lblInstruction);

            // E-posta etiketi
            Label lblEmail = new Label
            {
                Text = "E-posta Adresi",
                Font = new Font("Segoe UI", 12),
                Location = new Point(0, 120),
                Size = new Size(450, 30)
            };
            formContainer.Controls.Add(lblEmail);

            // E-posta metin kutusu
            txtEmail = new TextBox
            {
                Location = new Point(0, 150),
                Size = new Size(450, 40),
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8)
            };
            formContainer.Controls.Add(txtEmail);

            // Kod Gönder butonu
            btnSendCode = new Button
            {
                Text = "Kod Gönder",
                Location = new Point(0, 210),
                Size = new Size(450, 45),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSendCode.FlatAppearance.BorderSize = 0;
            btnSendCode.Click += BtnSendCode_Click;
            formContainer.Controls.Add(btnSendCode);

            // Giriş sayfasına geri dön butonu
            Button btnBack = new Button
            {
                Text = "Giriş Sayfasına Dön",
                Location = new Point(0, 270),
                Size = new Size(450, 40),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(0, 102, 204),
                Font = new Font("Segoe UI", 11),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderColor = Color.FromArgb(0, 102, 204);
            btnBack.Click += (s, e) => this.Close();
            formContainer.Controls.Add(btnBack);

            // Durum etiketi
            lblStatus = new Label
            {
                Text = "",
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 330),
                Size = new Size(450, 30)
            };
            formContainer.Controls.Add(lblStatus);

            // Panel'ları ekle
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

        private void BtnSendCode_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(email))
            {
                lblStatus.Text = "Lütfen e-posta adresinizi giriniz.";
                return;
            }

            if (!ValidationHelper.IsValidEmail(email))
            {
                lblStatus.Text = "Lütfen geçerli bir e-posta adresi giriniz.";
                return;
            }

            btnSendCode.Enabled = false;
            btnSendCode.Text = "Gönderiliyor...";
            Application.DoEvents();

            // E-posta ile kullanıcıyı bul
            FoundUser = DataStorage.FindUserByEmail(email);
            if (FoundUser == null)
            {
                lblStatus.Text = "Bu e-posta adresi ile kayıtlı kullanıcı bulunamadı.";
                btnSendCode.Enabled = true;
                btnSendCode.Text = "Kod Gönder";
                return;
            }

            // Şifre sıfırlama kodu oluştur
            ResetCode = DataStorage.GeneratePasswordResetCode(FoundUser);
            
            // E-posta gönderimi (gerçek uygulamada)
            // bool success = EmailService.SendPasswordResetEmail(email, ResetCode);
            
            // Geliştirme aşamasında simülasyon kullan
            bool success = EmailService.SendPasswordResetEmailSimulation(email, ResetCode);
            
            btnSendCode.Enabled = true;
            btnSendCode.Text = "Kod Gönder";
            
            if (success)
            {
                MessageBox.Show("Şifre sıfırlama kodu e-posta adresinize gönderildi.", 
                    "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Şifre değiştirme formunu aç
                ResetPasswordForm resetForm = new ResetPasswordForm(email, ResetCode);
                if (resetForm.ShowDialog() == DialogResult.OK)
                {
                    // Şifre başarıyla değiştirildi
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            else
            {
                lblStatus.Text = "E-posta gönderilirken bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";
            }
        }
    }
} 