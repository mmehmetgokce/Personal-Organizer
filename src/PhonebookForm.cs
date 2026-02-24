using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace PersonalOrganizer
{
    public class PhonebookForm : Form
    {
        private TextBox txtName;
        private MaskedTextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtCompany;
        private TextBox txtAddress;
        private TextBox txtNotes;
        private ComboBox cmbCategory;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnSearch;
        private TextBox txtSearch;
        private ListBox lstContacts;
        private User currentUser;
        private List<Contact> contacts;

        public PhonebookForm(User user)
        {
            currentUser = user;
            InitializeComponents();
            LoadContacts();
        }

        private void InitializeComponents()
        {
            // Form özellikleri
            this.Text = "Telefon Rehberi - Kişisel Organizatör";
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
                Text = "Telefon Rehberi",
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
                Text = "Kişilerinizi kolayca düzenleyin,\nbildirimler alın ve her zaman\niletişimde kalın.",
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
                Text = "Kişi Bilgileri",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(0, 0),
                Size = new Size(300, 30)
            };
            formContainer.Controls.Add(lblTitle);

            // Sol bölüm (form alanları)
            Panel leftSection = new Panel
            {
                Location = new Point(0, 40),
                Size = new Size(350, 560),
                BackColor = Color.White
            };

            // İsim etiketi ve metin kutusu
            Label lblName = new Label
            {
                Text = "İsim:",
                Location = new Point(0, 10),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10)
            };
            leftSection.Controls.Add(lblName);

            txtName = new TextBox
            {
                Location = new Point(100, 10),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtName.KeyPress += TxtName_KeyPress;
            leftSection.Controls.Add(txtName);

            // Telefon etiketi ve metin kutusu
            Label lblPhone = new Label
            {
                Text = "Telefon:",
                Location = new Point(0, 50),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10)
            };
            leftSection.Controls.Add(lblPhone);

            txtPhone = new MaskedTextBox
            {
                Location = new Point(100, 50),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Mask = "(999) 000-0000",
                PromptChar = '_'
            };
            leftSection.Controls.Add(txtPhone);

            // E-posta etiketi ve metin kutusu
            Label lblEmail = new Label
            {
                Text = "E-posta:",
                Location = new Point(0, 90),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10)
            };
            leftSection.Controls.Add(lblEmail);

            txtEmail = new TextBox
            {
                Location = new Point(100, 90),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            leftSection.Controls.Add(txtEmail);

            // Şirket etiketi ve metin kutusu
            Label lblCompany = new Label
            {
                Text = "Şirket:",
                Location = new Point(0, 130),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10)
            };
            leftSection.Controls.Add(lblCompany);

            txtCompany = new TextBox
            {
                Location = new Point(100, 130),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            leftSection.Controls.Add(txtCompany);

            // Adres etiketi ve metin kutusu
            Label lblAddress = new Label
            {
                Text = "Adres:",
                Location = new Point(0, 170),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10)
            };
            leftSection.Controls.Add(lblAddress);

            txtAddress = new TextBox
            {
                Location = new Point(100, 170),
                Size = new Size(250, 80),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true
            };
            leftSection.Controls.Add(txtAddress);

            // Notlar etiketi ve metin kutusu
            Label lblNotes = new Label
            {
                Text = "Notlar:",
                Location = new Point(0, 270),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10)
            };
            leftSection.Controls.Add(lblNotes);

            txtNotes = new TextBox
            {
                Location = new Point(100, 270),
                Size = new Size(250, 80),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true
            };
            leftSection.Controls.Add(txtNotes);

            // Kategori etiketi ve combobox
            Label lblCategory = new Label
            {
                Text = "Kategori:",
                Location = new Point(0, 370),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10)
            };
            leftSection.Controls.Add(lblCategory);

            cmbCategory = new ComboBox
            {
                Location = new Point(100, 370),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.Items.AddRange(new object[] { "Genel", "Aile", "İş", "Arkadaş", "Diğer" });
            cmbCategory.SelectedIndex = 0;
            leftSection.Controls.Add(cmbCategory);

            // Butonlar
            btnAdd = new Button
            {
                Text = "Ekle",
                Location = new Point(20, 420),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;
            leftSection.Controls.Add(btnAdd);

            btnUpdate = new Button
            {
                Text = "Güncelle",
                Location = new Point(130, 420),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(60, 170, 230),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.Click += BtnUpdate_Click;
            leftSection.Controls.Add(btnUpdate);

            btnDelete = new Button
            {
                Text = "Sil",
                Location = new Point(240, 420),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;
            leftSection.Controls.Add(btnDelete);
            
            // Sağ bölüm (liste ve arama)
            Panel rightSection = new Panel
            {
                Location = new Point(380, 40),
                Size = new Size(420, 560),
                BackColor = Color.White
            };

            // Arama alanı
            Label lblSearch = new Label
            {
                Text = "Kişi Ara:",
                Location = new Point(0, 10),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10)
            };
            rightSection.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new Point(80, 10),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            rightSection.Controls.Add(txtSearch);

            btnSearch = new Button
            {
                Text = "Ara",
                Location = new Point(330, 10),
                Size = new Size(90, 25),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += BtnSearch_Click;
            rightSection.Controls.Add(btnSearch);

            // Kişiler listesi
            Label lblContacts = new Label
            {
                Text = "Kişileriniz:",
                Location = new Point(0, 50),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            rightSection.Controls.Add(lblContacts);

            lstContacts = new ListBox
            {
                Location = new Point(0, 80),
                Size = new Size(420, 480),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            lstContacts.SelectedIndexChanged += LstContacts_SelectedIndexChanged;
            rightSection.Controls.Add(lstContacts);

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

        private void TxtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void LoadContacts()
        {
            lstContacts.Items.Clear();
            contacts = DataStorage.LoadContacts();
            
            // Sadece mevcut kullanıcıya ait kişileri filtrele
            contacts = contacts.Where(c => c.UserId == currentUser.UserId).ToList();
            
            foreach (Contact contact in contacts)
            {
                lstContacts.Items.Add($"{contact.Name} - {contact.Phone}");
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("İsim alanı zorunludur!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string phoneText = txtPhone.Text;
            string phoneDigits = new string(phoneText.Where(char.IsDigit).ToArray());
            if (phoneDigits.Length < 10)
            {
                MessageBox.Show("Lütfen geçerli bir telefon numarası giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !ValidationHelper.IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Lütfen geçerli bir e-posta adresi giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Contact contact = new Contact(currentUser.UserId, txtName.Text, txtPhone.Text, txtEmail.Text);
            
            contact.Company = txtCompany.Text;
            contact.Address = txtAddress.Text;
            contact.Notes = txtNotes.Text;
            contact.Category = cmbCategory.SelectedItem.ToString();
            
            contacts.Add(contact);
            DataStorage.SaveContacts(contacts);
            
            LoadContacts();
            ClearFields();
            
            MessageBox.Show("Kişi başarıyla eklendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (lstContacts.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen güncellenecek kişiyi seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("İsim alanı zorunludur!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string phoneText = txtPhone.Text;
            string phoneDigits = new string(phoneText.Where(char.IsDigit).ToArray());
            if (phoneDigits.Length < 10)
            {
                MessageBox.Show("Lütfen geçerli bir telefon numarası giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !ValidationHelper.IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Lütfen geçerli bir e-posta adresi giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Contact selectedContact = contacts[lstContacts.SelectedIndex];
            selectedContact.Name = txtName.Text;
            selectedContact.Phone = txtPhone.Text;
            selectedContact.Email = txtEmail.Text;
            selectedContact.Company = txtCompany.Text;
            selectedContact.Address = txtAddress.Text;
            selectedContact.Notes = txtNotes.Text;
            selectedContact.Category = cmbCategory.SelectedItem.ToString();
            selectedContact.ModifiedDate = DateTime.Now;
            
            DataStorage.SaveContacts(contacts);
            
            LoadContacts();
            ClearFields();
            
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            
            MessageBox.Show("Kişi başarıyla güncellendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstContacts.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen silinecek kişiyi seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"{contacts[lstContacts.SelectedIndex].Name} kişisini silmek istediğinizden emin misiniz?",
                "Kişi Silme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                contacts.RemoveAt(lstContacts.SelectedIndex);
                DataStorage.SaveContacts(contacts);
                
                LoadContacts();
                ClearFields();
                
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
                
                MessageBox.Show("Kişi başarıyla silindi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadContacts();
                return;
            }

            string searchTerm = txtSearch.Text.ToLower();
            lstContacts.Items.Clear();
            
            var filteredContacts = contacts.Where(c => 
                c.Name.ToLower().Contains(searchTerm) || 
                c.Phone.Contains(searchTerm) || 
                c.Email.ToLower().Contains(searchTerm) || 
                c.Company.ToLower().Contains(searchTerm) ||
                c.Category.ToLower().Contains(searchTerm)).ToList();
                
            foreach (Contact contact in filteredContacts)
            {
                lstContacts.Items.Add($"{contact.Name} - {contact.Phone}");
            }
        }

        private void LstContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstContacts.SelectedIndex != -1 && lstContacts.SelectedIndex < contacts.Count)
            {
                Contact selectedContact = contacts[lstContacts.SelectedIndex];
                txtName.Text = selectedContact.Name;
                txtPhone.Text = selectedContact.Phone;
                txtEmail.Text = selectedContact.Email;
                txtCompany.Text = selectedContact.Company;
                txtAddress.Text = selectedContact.Address;
                txtNotes.Text = selectedContact.Notes;
                
                int categoryIndex = cmbCategory.Items.IndexOf(selectedContact.Category);
                if (categoryIndex != -1)
                {
                    cmbCategory.SelectedIndex = categoryIndex;
                }
                else
                {
                    cmbCategory.SelectedIndex = 0;
                }
                
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtCompany.Clear();
            txtAddress.Clear();
            txtNotes.Clear();
            cmbCategory.SelectedIndex = 0;
            lstContacts.SelectedIndex = -1;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }
    }
}