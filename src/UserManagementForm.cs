using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing;

namespace PersonalOrganizer
{
    public class UserManagementForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtEmail;
        private ComboBox cmbRole;
        private Button btnSave;
        private ListBox lstUsers;
        private Button btnDelete;
        private Button btnChangeRole;
        private Button btnSendPassword;
        private ProgressBar progressBar;
        private User currentUser;
        private List<User> users;

        public UserManagementForm(User user)
        {
            currentUser = user;
            InitializeComponents();
            LoadUsers();
        }

        private void InitializeComponents()
        {
            this.Text = "Kullanıcı Yönetimi - Kişisel Organizatör";
            this.Size = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(800, 600);
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.FormClosing += UserManagementForm_FormClosing;

            // Sol panel (logo/banner alanı)
            Panel leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = this.Width / 3,
                BackColor = Color.FromArgb(41, 128, 185)
            };

            // Banner başlık
            Label lblAppName = new Label
            {
                Text = "Kullanıcı Yönetimi",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 150,
                Padding = new Padding(0, 100, 0, 0)
            };
            leftPanel.Controls.Add(lblAppName);

            // Açıklama metni
            Label lblDescription = new Label
            {
                Text = "Tüm sistem kullanıcılarını\nbu ekrandan yönetebilirsiniz.",
                Font = new Font("Segoe UI Light", 16, FontStyle.Regular),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 100, 20, 0)
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
                Height = 600,
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
                Text = "Sistem Kullanıcıları",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(0, 0),
                Size = new Size(800, 40)
            };
            formContainer.Controls.Add(lblTitle);

            // Sol bölüm (kullanıcı ekleme formu)
            Panel leftSection = new Panel
            {
                Location = new Point(0, 50),
                Size = new Size(350, 550),
                BackColor = Color.White
            };

            // Kullanıcı adı etiketi ve giriş alanı
            Label lblUsername = new Label
            {
                Text = "Kullanıcı Adı:",
                Location = new Point(0, 10),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            leftSection.Controls.Add(lblUsername);

            txtUsername = new TextBox
            {
                Location = new Point(0, 35),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.Black
            };
            leftSection.Controls.Add(txtUsername);

            // Şifre etiketi ve giriş alanı
            Label lblPassword = new Label
            {
                Text = "Şifre:",
                Location = new Point(0, 75),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            leftSection.Controls.Add(lblPassword);

            txtPassword = new TextBox
            {
                Location = new Point(0, 100),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '*',
                ForeColor = Color.Black
            };
            leftSection.Controls.Add(txtPassword);

            // E-posta etiketi ve giriş alanı
            Label lblEmail = new Label
            {
                Text = "E-posta Adresi:",
                Location = new Point(0, 140),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            leftSection.Controls.Add(lblEmail);

            txtEmail = new TextBox
            {
                Location = new Point(0, 165),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.Black
            };
            leftSection.Controls.Add(txtEmail);

            // Rol etiketi ve seçim alanı
            Label lblRole = new Label
            {
                Text = "Kullanıcı Rolü:",
                Location = new Point(0, 205),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            leftSection.Controls.Add(lblRole);

            cmbRole = new ComboBox
            {
                Location = new Point(0, 230),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cmbRole.Items.AddRange(Enum.GetNames(typeof(UserRole)));
            leftSection.Controls.Add(cmbRole);

            // Butonlar
            btnSave = new Button
            {
                Text = "Kullanıcı Ekle",
                Location = new Point(0, 280),
                Size = new Size(350, 40),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            leftSection.Controls.Add(btnSave);

            btnDelete = new Button
            {
                Text = "Kullanıcı Sil",
                Location = new Point(0, 330),
                Size = new Size(350, 40),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;
            leftSection.Controls.Add(btnDelete);

            btnChangeRole = new Button
            {
                Text = "Rol Değiştir",
                Location = new Point(0, 380),
                Size = new Size(350, 40),
                BackColor = Color.FromArgb(60, 170, 230),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnChangeRole.FlatAppearance.BorderSize = 0;
            btnChangeRole.Click += BtnChangeRole_Click;
            leftSection.Controls.Add(btnChangeRole);

            btnSendPassword = new Button
            {
                Text = "Şifre Gönder",
                Location = new Point(0, 430),
                Size = new Size(350, 40),
                BackColor = Color.FromArgb(55, 126, 59),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSendPassword.FlatAppearance.BorderSize = 0;
            btnSendPassword.Click += BtnSendPassword_Click;
            leftSection.Controls.Add(btnSendPassword);

            // İlerleme durumu
            Label lblProgress = new Label
            {
                Text = "İşlem Durumu:",
                Location = new Point(0, 480),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            leftSection.Controls.Add(lblProgress);

            progressBar = new ProgressBar
            {
                Location = new Point(0, 505),
                Size = new Size(350, 20),
                Visible = false
            };
            leftSection.Controls.Add(progressBar);

            // Sağ bölüm (kullanıcı listesi)
            Panel rightSection = new Panel
            {
                Location = new Point(380, 50),
                Size = new Size(420, 550),
                BackColor = Color.White
            };

            // Kullanıcı listesi etiketi
            Label lblUsersList = new Label
            {
                Text = "Kayıtlı Kullanıcılar:",
                Location = new Point(0, 10),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204)
            };
            rightSection.Controls.Add(lblUsersList);

            lstUsers = new ListBox
            {
                Location = new Point(0, 40),
                Size = new Size(420, 510),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            lstUsers.SelectedIndexChanged += LstUsers_SelectedIndexChanged;
            rightSection.Controls.Add(lstUsers);

            // Container'a ekle
            formContainer.Controls.Add(leftSection);
            formContainer.Controls.Add(rightSection);

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

        private void LstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUsers.SelectedIndex != -1 && lstUsers.SelectedIndex < users.Count)
            {
                User selectedUser = users[lstUsers.SelectedIndex];
                txtUsername.Text = selectedUser.Username;
                txtEmail.Text = selectedUser.Email;
                cmbRole.SelectedItem = selectedUser.Role.ToString();
                
                // Güvenlik için şifre gösterilmez
                txtPassword.Text = "********";
            }
        }

        private void UserManagementForm_FormClosing(object sender, FormClosingEventArgs e)
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || 
                string.IsNullOrWhiteSpace(txtPassword.Text) || 
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                cmbRole.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // E-posta formatı kontrolü
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Lütfen geçerli bir e-posta adresi girin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Seçilen kullanıcıyı güncelle veya yeni kullanıcı ekle
            if (lstUsers.SelectedIndex != -1 && lstUsers.SelectedIndex < users.Count)
            {
                // Kullanıcı güncelleme
                User selectedUser = users[lstUsers.SelectedIndex];
                
                // Admin rolündeki kullanıcının rolünü sadece admin değiştirebilir
                if (selectedUser.Role == UserRole.Admin && currentUser.Role != UserRole.Admin)
                {
                    MessageBox.Show("Admin kullanıcısının bilgilerini değiştirme yetkiniz yok!", "Uyarı",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                selectedUser.Username = txtUsername.Text;
                // Şifre değiştirilecekse (yani varsayılan şifreden farklıysa)
                if (txtPassword.Text != "********")
                {
                    selectedUser.Password = txtPassword.Text;
                }
                selectedUser.Email = txtEmail.Text;
                
                // Rol değişikliği sadece admin tarafından yapılabilir
                if (currentUser.Role == UserRole.Admin)
                {
                    UserRole newRole = (UserRole)Enum.Parse(typeof(UserRole), cmbRole.SelectedItem.ToString());
                    selectedUser.Role = newRole;
                }
                
                DataStorage.SaveUsers(users);
                MessageBox.Show("Kullanıcı başarıyla güncellendi!", "Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Yeni kullanıcı ekleme
                // Yeni kullanıcıya bir ID ata
                string userId = Guid.NewGuid().ToString();
                
                // Rol ata - eğer hiç kullanıcı yoksa ilk kullanıcı admin olsun
                UserRole role = users.Count == 0 ? 
                    UserRole.Admin : 
                    (UserRole)Enum.Parse(typeof(UserRole), cmbRole.SelectedItem.ToString());
                
                // Yeni kullanıcı oluştur
                User newUser = new User
                {
                    UserId = userId,
                    Username = txtUsername.Text,
                    Password = txtPassword.Text,
                    Email = txtEmail.Text,
                    Role = role,
                    CreatedDate = DateTime.Now,
                    LastLoginDate = DateTime.MinValue
                };

                // Kullanıcıyı listeye ekle ve kaydet
                users.Add(newUser);
                DataStorage.SaveUsers(users);
                
                MessageBox.Show("Kullanıcı başarıyla eklendi!", "Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            LoadUsers();
            ClearFields();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstUsers.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen silmek istediğiniz kullanıcıyı seçin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (lstUsers.SelectedIndex >= 0 && lstUsers.SelectedIndex < users.Count)
            {
                User selectedUser = users[lstUsers.SelectedIndex];
                
                // Admin kullanıcısını silmeyi engelle
                if (selectedUser.Role == UserRole.Admin && users.Count(u => u.Role == UserRole.Admin) <= 1)
                {
                    MessageBox.Show("Son admin kullanıcısı silinemez!", "Uyarı", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Kullanıcıyı silme yetkisi kontrolü
                if (selectedUser.Role == UserRole.Admin && currentUser.Role != UserRole.Admin)
                {
                    MessageBox.Show("Admin kullanıcısını silme yetkiniz yok!", "Uyarı", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Onay iste
                DialogResult result = MessageBox.Show(
                    $"{selectedUser.Username} kullanıcısını silmek istediğinizden emin misiniz?",
                    "Kullanıcı Silme Onayı",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                    
                if (result == DialogResult.Yes)
                {
                    // Kullanıcıyı listeden çıkar ve kaydet
                    users.RemoveAt(lstUsers.SelectedIndex);
                    DataStorage.SaveUsers(users);
                    LoadUsers();
                    ClearFields();
                }
            }
        }

        private void BtnChangeRole_Click(object sender, EventArgs e)
        {
            if (currentUser.Role != UserRole.Admin)
            {
                MessageBox.Show("Bu işlem için admin yetkisi gereklidir!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (lstUsers.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen rolünü değiştirmek istediğiniz kullanıcıyı seçin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbRole.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen yeni kullanıcı rolü seçin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (lstUsers.SelectedIndex >= 0 && lstUsers.SelectedIndex < users.Count)
            {
                User selectedUser = users[lstUsers.SelectedIndex];
                
                // Son admin kullanıcısının rolünü değiştirmeyi engelle
                if (selectedUser.Role == UserRole.Admin && 
                    users.Count(u => u.Role == UserRole.Admin) <= 1 &&
                    cmbRole.SelectedItem.ToString() != UserRole.Admin.ToString())
                {
                    MessageBox.Show("Son admin kullanıcısının rolü değiştirilemez!", "Uyarı", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                UserRole newRole = (UserRole)Enum.Parse(typeof(UserRole), cmbRole.SelectedItem.ToString());
                selectedUser.Role = newRole;
                
                // Kullanıcıyı güncelle ve tüm listeyi kaydet
                DataStorage.SaveUsers(users);
                LoadUsers();
                
                MessageBox.Show($"{selectedUser.Username} kullanıcısının rolü {newRole} olarak güncellendi!", "Bilgi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void BtnSendPassword_Click(object sender, EventArgs e)
        {
            if (currentUser.Role != UserRole.Admin)
            {
                MessageBox.Show("Bu işlem için admin yetkisi gereklidir!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (lstUsers.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen şifre göndermek istediğiniz kullanıcıyı seçin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (lstUsers.SelectedIndex >= 0 && lstUsers.SelectedIndex < users.Count)
            {
                User selectedUser = users[lstUsers.SelectedIndex];

                progressBar.Visible = true;
                progressBar.Value = 0;

                try
                {
                    await SendPasswordEmail(selectedUser);
                    MessageBox.Show("Şifre başarıyla gönderildi!", "Bilgi", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Şifre gönderilirken hata oluştu: {ex.Message}", "Hata", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    progressBar.Visible = false;
                }
            }
        }

        private async Task SendPasswordEmail(User user)
        {
            // Burada e-posta gönderme işlemi simüle ediliyor
            for (int i = 0; i <= 100; i += 10)
            {
                progressBar.Value = i;
                await Task.Delay(100);
            }

            // Gerçek e-posta gönderme kodu buraya eklenecek
            // SmtpClient kullanılarak e-posta gönderilebilir
        }

        private void LoadUsers()
        {
            lstUsers.Items.Clear();
            users = DataStorage.LoadUsers();
            foreach (User user in users)
            {
                lstUsers.Items.Add($"{user.Username} - {user.Email} - {user.Role}");
            }
        }

        private void ClearFields()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtEmail.Clear();
            cmbRole.SelectedIndex = -1;
        }
    }
}