using System;
using System.Drawing;
using System.Windows.Forms;

namespace PersonalOrganizer
{
    public partial class RegisterForm : Form
    {
        public User RegisteredUser { get; private set; }

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Kayıt Ol - Kişisel Organizatör";
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Form başlığı
            Label lblTitle = new Label
            {
                Text = "Yeni Hesap Oluştur",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(70, 20),
                Size = new Size(250, 40)
            };
            this.Controls.Add(lblTitle);

            // Kullanıcı adı etiketi
            Label lblUsername = new Label
            {
                Text = "Kullanıcı Adı:",
                Location = new Point(50, 80),
                Size = new Size(100, 20)
            };
            this.Controls.Add(lblUsername);

            // Kullanıcı adı metin kutusu
            TextBox txtUsername = new TextBox
            {
                Location = new Point(150, 80),
                Size = new Size(180, 20),
                Name = "txtUsername"
            };
            this.Controls.Add(txtUsername);

            // E-posta etiketi
            Label lblEmail = new Label
            {
                Text = "E-posta:",
                Location = new Point(50, 110),
                Size = new Size(100, 20)
            };
            this.Controls.Add(lblEmail);

            // E-posta metin kutusu
            TextBox txtEmail = new TextBox
            {
                Location = new Point(150, 110),
                Size = new Size(180, 20),
                Name = "txtEmail"
            };
            this.Controls.Add(txtEmail);

            // Şifre etiketi
            Label lblPassword = new Label
            {
                Text = "Şifre:",
                Location = new Point(50, 140),
                Size = new Size(100, 20)
            };
            this.Controls.Add(lblPassword);

            // Şifre metin kutusu
            TextBox txtPassword = new TextBox
            {
                Location = new Point(150, 140),
                Size = new Size(180, 20),
                Name = "txtPassword",
                PasswordChar = '*'
            };
            this.Controls.Add(txtPassword);

            // Şifre tekrar etiketi
            Label lblConfirmPassword = new Label
            {
                Text = "Şifre Tekrar:",
                Location = new Point(50, 170),
                Size = new Size(100, 20)
            };
            this.Controls.Add(lblConfirmPassword);

            // Şifre tekrar metin kutusu
            TextBox txtConfirmPassword = new TextBox
            {
                Location = new Point(150, 170),
                Size = new Size(180, 20),
                Name = "txtConfirmPassword",
                PasswordChar = '*'
            };
            this.Controls.Add(txtConfirmPassword);

            // Rol etiketi
            Label lblRole = new Label
            {
                Text = "Kullanıcı Rolü:",
                Location = new Point(50, 200),
                Size = new Size(100, 20)
            };
            this.Controls.Add(lblRole);

            // Rol seçim kutusu
            ComboBox cmbRole = new ComboBox
            {
                Location = new Point(150, 200),
                Size = new Size(180, 20),
                Name = "cmbRole",
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(Enum.GetNames(typeof(UserRole)));
            cmbRole.SelectedIndex = 0;
            this.Controls.Add(cmbRole);

            // Kayıt Ol butonu
            Button btnRegister = new Button
            {
                Text = "Kayıt Ol",
                Location = new Point(150, 240),
                Size = new Size(180, 30),
                Name = "btnRegister",
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.Click += (s, e) => RegisterUser();
            this.Controls.Add(btnRegister);

            // Hata mesajı etiketi
            Label lblError = new Label
            {
                Text = "",
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(50, 280),
                Size = new Size(300, 20),
                Name = "lblError"
            };
            this.Controls.Add(lblError);

            // İptal butonu
            Button btnCancel = new Button
            {
                Text = "İptal",
                Location = new Point(150, 310),
                Size = new Size(100, 30),
                Name = "btnCancel",
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);

            // Enter tuşu ile kayıt
            this.AcceptButton = btnRegister;
        }

        private void RegisterUser()
        {
            TextBox txtUsername = (TextBox)Controls.Find("txtUsername", true)[0];
            TextBox txtPassword = (TextBox)Controls.Find("txtPassword", true)[0];
            TextBox txtConfirmPassword = (TextBox)Controls.Find("txtConfirmPassword", true)[0];
            TextBox txtEmail = (TextBox)Controls.Find("txtEmail", true)[0];
            ComboBox cmbRole = (ComboBox)Controls.Find("cmbRole", true)[0];

            if (string.IsNullOrWhiteSpace(txtUsername.Text) || 
                string.IsNullOrWhiteSpace(txtPassword.Text) || 
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ShowError("Tüm alanları doldurun!");
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                ShowError("Şifreler eşleşmiyor!");
                return;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                ShowError("Geçerli bir e-posta adresi girin!");
                return;
            }

            // Kullanıcı rolünü enum'a çevir
            UserRole role = (UserRole)Enum.Parse(typeof(UserRole), cmbRole.SelectedItem.ToString());

            // Kullanıcı kaydını DataStorage üzerinden yap
            User newUser = new User(); // Varsayılan yapıcı ile oluştur
            newUser.Username = txtUsername.Text;
            newUser.Password = txtPassword.Text;
            newUser.Email = txtEmail.Text;
            newUser.Role = role;
            
            if (DataStorage.RegisterUser(newUser))
            {
                RegisteredUser = newUser;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                ShowError("Bu kullanıcı adı zaten kullanılıyor!");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ShowError(string message)
        {
            Label lblError = (Label)Controls.Find("lblError", true)[0];
            lblError.Text = message;
        }
    }
} 