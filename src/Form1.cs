using System;
using System.Windows.Forms;
using System.Drawing;

namespace PersonalOrganizer
{
    public partial class Form1 : Form
    {
        private User currentUser;

        public Form1()
        {
            InitializeComponent();
            ShowLoginForm();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            // Bu metot form yüklendiğinde çalışır
            // Form başlatılması için gereken ekstra işlemler burada yapılabilir
        }

        private void ShowLoginForm()
        {
            using (LoginForm loginForm = new LoginForm())
            {
                DialogResult result = loginForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    currentUser = loginForm.LoggedInUser;
                    this.Text = $"Kişisel Organizatör - {currentUser.Username} ({currentUser.Role})";
                    InitializeModules();
                }
                else
                {
                    // Kullanıcı giriş yapmadıysa uygulamayı kapat
                    Application.Exit();
                }
            }
        }

        private void InitializeModules()
        {
            // Ana form ayarlarını belirle
            this.Text = "Kişisel Organizatör";
            this.Size = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(800, 600);
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.FormClosing += Form1_FormClosing;

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
                Text = "Kişisel Organizatör",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 150,
                Padding = new Padding(0, 50, 0, 0)
            };
            leftPanel.Controls.Add(lblAppName);

            // Kullanıcı Bilgisi
            Label lblUser = new Label
            {
                Text = $"Hoş Geldiniz, {currentUser.Username}!",
                Font = new Font("Segoe UI Light", 16, FontStyle.Italic),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Top,
                Height = 50
            };
            leftPanel.Controls.Add(lblUser);

            // Açıklama metni
            Label lblDescription = new Label
            {
                Text = "Tüm kişisel organizasyonunuz için\ngereken modüller tek bir yerde.",
                Font = new Font("Segoe UI Light", 16, FontStyle.Regular),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 30, 20, 0)
            };
            leftPanel.Controls.Add(lblDescription);

            // Çıkış Butonu
            Button btnLogout = new Button
            {
                Text = "Çıkış Yap",
                Size = new Size(150, 40),
                Dock = DockStyle.Bottom,
                Margin = new Padding(10, 10, 10, 20),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (sender, e) => {
                DialogResult result = MessageBox.Show("Çıkış yapmak istediğinizden emin misiniz?", 
                    "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            };
            leftPanel.Controls.Add(btnLogout);

            // Sağ panel (modül butonları alanı)
            Panel rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(40)
            };

            // Modül container
            Panel moduleContainer = new Panel
            {
                Width = 650,
                Height = 600,
                Anchor = AnchorStyles.None,
                BackColor = Color.White
            };
            
            // Panel'ı ortala
            moduleContainer.Location = new Point(
                (rightPanel.ClientSize.Width - moduleContainer.Width) / 2,
                (rightPanel.ClientSize.Height - moduleContainer.Height) / 2);

            // Modül başlığı
            Label lblTitle = new Label
            {
                Text = "Modüller",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(0, 0),
                Size = new Size(650, 50)
            };
            moduleContainer.Controls.Add(lblTitle);

            // Modül butonları
            TableLayoutPanel modulePanel = new TableLayoutPanel
            {
                Location = new Point(0, 70),
                Size = new Size(650, 480),
                ColumnCount = 2,
                RowCount = 3,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            modulePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            modulePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            modulePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            modulePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
            modulePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));

            CreateModuleButton("Kullanıcı Yönetimi", modulePanel, 0, 0);
            CreateModuleButton("Telefon Rehberi", modulePanel, 0, 1);
            CreateModuleButton("Notlar", modulePanel, 1, 0);
            CreateModuleButton("Kişisel Bilgiler", modulePanel, 1, 1);
            CreateModuleButton("Hatırlatıcılar", modulePanel, 2, 0);
            CreateModuleButton("Maaş Hesaplayıcı", modulePanel, 2, 1);

            moduleContainer.Controls.Add(modulePanel);

            // Panelleri ekle
            rightPanel.Controls.Add(moduleContainer);
            this.Controls.Add(rightPanel);
            this.Controls.Add(leftPanel);
            
            // Form boyutu değiştiğinde panellerin tekrar düzenlenmesi
            this.Resize += (s, e) => {
                leftPanel.Width = this.ClientSize.Width / 3;
                moduleContainer.Location = new Point(
                    (rightPanel.ClientSize.Width - moduleContainer.Width) / 2,
                    (rightPanel.ClientSize.Height - moduleContainer.Height) / 2);
            };
        }

        private void CreateModuleButton(string text, TableLayoutPanel panel, int row, int column)
        {
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                BackColor = Color.White
            };

            Button button = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += (sender, e) => OpenModule(text);

            buttonPanel.Controls.Add(button);
            panel.Controls.Add(buttonPanel, column, row);
        }

        private void OpenModule(string moduleName)
        {
            switch (moduleName)
            {
                case "Kullanıcı Yönetimi":
                    // Admin kontrolü
                    if (currentUser.Role != UserRole.Admin)
                    {
                        MessageBox.Show("Bu modüle erişim için admin yetkisi gereklidir!", "Yetkisiz Erişim", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    var userManagementForm = new UserManagementForm(currentUser);
                    userManagementForm.ShowDialog();
                    break;
                    
                case "Telefon Rehberi":
                    var phonebookForm = new PhonebookForm(currentUser);
                    phonebookForm.ShowDialog();
                    break;
                    
                case "Notlar":
                    var notesForm = new NotesForm(currentUser);
                    notesForm.ShowDialog();
                    break;
                    
                case "Kişisel Bilgiler":
                    var personalInfoForm = new PersonalInfoForm(currentUser);
                    personalInfoForm.ShowDialog();
                    break;
                    
                case "Hatırlatıcılar":
                    var remindersForm = new RemindersForm(currentUser);
                    remindersForm.ShowDialog();
                    break;
                    
                case "Maaş Hesaplayıcı":
                    var salaryForm = new SalaryCalculatorForm(currentUser);
                    salaryForm.ShowDialog();
                    break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Uygulamadan çıkmak istediğinizden emin misiniz?", 
                    "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
