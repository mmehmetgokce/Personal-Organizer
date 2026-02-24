using System;
using System.Drawing;
using System.Windows.Forms;

namespace PersonalOrganizer
{
    public class ResetPasswordForm : Form
    {
        private TextBox txtCode;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnResetPassword;
        private Label lblStatus;
        
        private string userEmail;
        private string expectedCode;

        public ResetPasswordForm(string email, string resetCode)
        {
            userEmail = email;
            expectedCode = resetCode;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Şifre Sıfırlama";
            this.Size = new Size(400, 320);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Form başlığı
            Label lblTitle = new Label
            {
                Text = "Yeni Şifre Belirleme",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(100, 20),
                Size = new Size(200, 30)
            };
            this.Controls.Add(lblTitle);

            // Açıklama etiketi
            Label lblInstruction = new Label
            {
                Text = $"E-posta adresinize ({userEmail}) gönderilen kodu girin\nve yeni şifrenizi belirleyin.",
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(50, 60),
                Size = new Size(300, 40)
            };
            this.Controls.Add(lblInstruction);

            // Kod etiketi
            Label lblCode = new Label
            {
                Text = "Sıfırlama Kodu:",
                Location = new Point(50, 110),
                Size = new Size(100, 20)
            };
            this.Controls.Add(lblCode);

            // Kod metin kutusu
            txtCode = new TextBox
            {
                Location = new Point(160, 110),
                Size = new Size(180, 20),
                Text = expectedCode  // Test için kodu otomatik doldur (gerçek uygulamada kaldırılabilir)
            };
            this.Controls.Add(txtCode);

            // Yeni şifre etiketi
            Label lblNewPassword = new Label
            {
                Text = "Yeni Şifre:",
                Location = new Point(50, 140),
                Size = new Size(100, 20)
            };
            this.Controls.Add(lblNewPassword);

            // Yeni şifre metin kutusu
            txtNewPassword = new TextBox
            {
                Location = new Point(160, 140),
                Size = new Size(180, 20),
                PasswordChar = '•'
            };
            this.Controls.Add(txtNewPassword);

            // Şifre tekrar etiketi
            Label lblConfirmPassword = new Label
            {
                Text = "Şifre (Tekrar):",
                Location = new Point(50, 170),
                Size = new Size(100, 20)
            };
            this.Controls.Add(lblConfirmPassword);

            // Şifre tekrar metin kutusu
            txtConfirmPassword = new TextBox
            {
                Location = new Point(160, 170),
                Size = new Size(180, 20),
                PasswordChar = '•'
            };
            this.Controls.Add(txtConfirmPassword);

            // Şifreyi Sıfırla butonu
            btnResetPassword = new Button
            {
                Text = "Şifreyi Sıfırla",
                Location = new Point(160, 210),
                Size = new Size(180, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnResetPassword.Click += BtnResetPassword_Click;
            this.Controls.Add(btnResetPassword);

            // Durum etiketi
            lblStatus = new Label
            {
                Text = "",
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(50, 250),
                Size = new Size(300, 20)
            };
            this.Controls.Add(lblStatus);
        }

        private void BtnResetPassword_Click(object sender, EventArgs e)
        {
            string code = txtCode.Text.Trim();
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            
            // Giriş kontrolü
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                lblStatus.Text = "Lütfen tüm alanları doldurunuz.";
                return;
            }
            
            // Şifre eşleşmesi kontrolü
            if (newPassword != confirmPassword)
            {
                lblStatus.Text = "Girilen şifreler eşleşmiyor.";
                return;
            }
            
            // Şifre uzunluğu kontrolü
            if (newPassword.Length < 6)
            {
                lblStatus.Text = "Şifre en az 6 karakter olmalıdır.";
                return;
            }
            
            // Kodu doğrula
            if (!DataStorage.ValidateResetCode(code, userEmail))
            {
                lblStatus.Text = "Geçersiz veya süresi dolmuş sıfırlama kodu.";
                return;
            }
            
            // Şifreyi sıfırla
            if (DataStorage.ResetPassword(code, newPassword))
            {
                MessageBox.Show("Şifreniz başarıyla değiştirildi. Yeni şifreniz ile giriş yapabilirsiniz.", 
                    "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblStatus.Text = "Şifre değiştirilirken bir hata oluştu. Lütfen tekrar deneyiniz.";
            }
        }
    }
} 