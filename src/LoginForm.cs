using System;
using System.Drawing;
using System.Windows.Forms;

namespace PersonalOrganizer
{
    public partial class LoginForm : Form
    {
        public User LoggedInUser { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Kişisel Organizatör - Giriş";
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
                Width = this.Width / 2,
                BackColor = Color.FromArgb(0, 102, 204)
            };

            // Banner başlık
            Label lblAppName = new Label
            {
                Text = "Kişisel Organizatör",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(0, 50, 0, 0)
            };
            leftPanel.Controls.Add(lblAppName);

            // Slogan/Açıklama
            Label lblSlogan = new Label
            {
                Text = "Hayatınızı düzenleyin,\nzamanınızı verimli kullanın.",
                Font = new Font("Segoe UI Light", 20, FontStyle.Regular),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 30, 20, 0)
            };
            leftPanel.Controls.Add(lblSlogan);

            // Sağ panel (giriş formu)
            Panel rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(60)
            };

            // Giriş formu container
            Panel loginContainer = new Panel
            {
                Width = 400,
                Height = 500,
                Anchor = AnchorStyles.None,
                BackColor = Color.White
            };
            
            // Panel'ı ortala
            loginContainer.Location = new Point(
                (rightPanel.ClientSize.Width - loginContainer.Width) / 2,
                (rightPanel.ClientSize.Height - loginContainer.Height) / 2);

            // Giriş formu başlığı
            Label lblLoginTitle = new Label
            {
                Text = "Hesabınıza Giriş Yapın",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(0, 0),
                Size = new Size(400, 50)
            };
            loginContainer.Controls.Add(lblLoginTitle);

            // Kullanıcı adı etiketi
            Label lblUsername = new Label
            {
                Text = "Kullanıcı Adı",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Location = new Point(0, 80),
                Size = new Size(400, 30)
            };
            loginContainer.Controls.Add(lblUsername);

            // Kullanıcı adı metin kutusu
            TextBox txtUsername = new TextBox
            {
                Location = new Point(0, 110),
                Size = new Size(400, 40),
                Name = "txtUsername",
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            loginContainer.Controls.Add(txtUsername);

            // Şifre etiketi
            Label lblPassword = new Label
            {
                Text = "Şifre",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Location = new Point(0, 160),
                Size = new Size(400, 30)
            };
            loginContainer.Controls.Add(lblPassword);

            // Şifre metin kutusu
            TextBox txtPassword = new TextBox
            {
                Location = new Point(0, 190),
                Size = new Size(400, 40),
                Name = "txtPassword",
                PasswordChar = '•',
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            loginContainer.Controls.Add(txtPassword);

            // Şifremi Unuttum bağlantısı
            LinkLabel lnkForgotPassword = new LinkLabel
            {
                Text = "Şifremi Unuttum",
                Font = new Font("Segoe UI", 10),
                LinkColor = Color.FromArgb(0, 102, 204),
                ActiveLinkColor = Color.FromArgb(0, 150, 250),
                Location = new Point(0, 240),
                Size = new Size(400, 20),
                TextAlign = ContentAlignment.MiddleRight
            };
            lnkForgotPassword.LinkClicked += (s, e) => ShowForgotPasswordForm();
            loginContainer.Controls.Add(lnkForgotPassword);

            // Giriş Yap butonu
            Button btnLogin = new Button
            {
                Text = "Giriş Yap",
                Location = new Point(0, 280),
                Size = new Size(400, 45),
                Name = "btnLogin",
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += (s, e) => LoginUser(txtUsername.Text, txtPassword.Text);
            loginContainer.Controls.Add(btnLogin);

            // Çizgi
            Panel separatorLine = new Panel
            {
                Location = new Point(0, 350),
                Size = new Size(400, 1),
                BackColor = Color.FromArgb(230, 230, 230)
            };
            loginContainer.Controls.Add(separatorLine);

            // Kayıt Ol etiketi
            Label lblRegister = new Label
            {
                Text = "Hesabınız yok mu?",
                Font = new Font("Segoe UI", 10),
                Location = new Point(50, 370),
                Size = new Size(200, 30),
                TextAlign = ContentAlignment.MiddleRight
            };
            loginContainer.Controls.Add(lblRegister);

            // Kayıt Ol bağlantısı
            LinkLabel lnkRegister = new LinkLabel
            {
                Text = "Hemen Kayıt Olun",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                LinkColor = Color.FromArgb(0, 102, 204),
                ActiveLinkColor = Color.FromArgb(0, 150, 250),
                Location = new Point(250, 370),
                Size = new Size(150, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            lnkRegister.LinkClicked += (s, e) => ShowRegisterForm();
            loginContainer.Controls.Add(lnkRegister);

            // Hata mesajı etiketi
            Label lblError = new Label
            {
                Text = "",
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 9.75f),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 420),
                Size = new Size(400, 30),
                Name = "lblError"
            };
            loginContainer.Controls.Add(lblError);

            // Panel'ları ekle
            rightPanel.Controls.Add(loginContainer);
            this.Controls.Add(rightPanel);
            this.Controls.Add(leftPanel);

            // Enter tuşu ile giriş
            this.AcceptButton = btnLogin;
            
            // Form boyutu değiştiğinde panellerin tekrar düzenlenmesi
            this.Resize += (s, e) => {
                leftPanel.Width = this.ClientSize.Width / 2;
                loginContainer.Location = new Point(
                    (rightPanel.ClientSize.Width - loginContainer.Width) / 2,
                    (rightPanel.ClientSize.Height - loginContainer.Height) / 2);
            };
        }

        private void LoginUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Kullanıcı adı ve şifre boş olamaz!");
                return;
            }

            User user = DataStorage.AuthenticateUser(username, password);
            if (user != null)
            {
                LoggedInUser = user;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                ShowError("Kullanıcı adı veya şifre hatalı!");
            }
        }

        private void ShowError(string message)
        {
            Label lblError = (Label)Controls.Find("lblError", true)[0];
            lblError.Text = message;
        }

        private void ShowRegisterForm()
        {
            RegisterForm registerForm = new RegisterForm();
            if (registerForm.ShowDialog() == DialogResult.OK)
            {
                // Başarılı kayıt olunduğunda kullanıcıyı otomatik olarak giriş yap
                LoggedInUser = registerForm.RegisteredUser;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void ShowForgotPasswordForm()
        {
            ForgotPasswordForm forgotPasswordForm = new ForgotPasswordForm();
            if (forgotPasswordForm.ShowDialog() == DialogResult.OK)
            {
                // Şifre başarıyla sıfırlandı, kullanıcıya bilgi mesajı göster
                ShowError("Şifreniz başarıyla sıfırlandı. Yeni şifreniz ile giriş yapabilirsiniz.");
            }
        }
    }
} 
