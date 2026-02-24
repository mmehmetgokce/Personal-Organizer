using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;

namespace PersonalOrganizer
{
    public class NotesForm : Form
    {
        private TextBox txtTitle;
        private TextBox txtContent;
        private TextBox txtTags;
        private ComboBox cmbCategory;
        private Button btnSave;
        private Button btnDelete;
        private Button btnUpdate;
        private ListBox lstNotes;
        private User currentUser;
        private List<Note> notes;

        public NotesForm(User user)
        {
            currentUser = user;
            InitializeComponents();
            LoadNotes();
        }

        private void InitializeComponents()
        {
            this.Text = "Notlar - Kişisel Organizatör";
            this.Size = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(800, 600);
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.FormClosing += NotesForm_FormClosing;

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
                Text = "Notlar",
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
                Text = "Önemli bilgileri kaydedin,\ndüzenleyin ve kolayca\nerişin.",
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
                Text = "Not Defteriniz",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(0, 0),
                Size = new Size(800, 40)
            };
            formContainer.Controls.Add(lblTitle);

            // Sol bölüm (not oluşturma formu)
            Panel leftSection = new Panel
            {
                Location = new Point(0, 50),
                Size = new Size(350, 550),
                BackColor = Color.White
            };

            // Başlık alanı
            Label lblNoteTitle = new Label
            {
                Text = "Not Başlığı:",
                Location = new Point(0, 10),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            leftSection.Controls.Add(lblNoteTitle);

            txtTitle = new TextBox
            {
                Location = new Point(0, 35),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Notunuzun başlığını girin",
                ForeColor = Color.Gray
            };
            txtTitle.Enter += TextBox_Enter;
            txtTitle.Leave += TextBox_Leave;
            leftSection.Controls.Add(txtTitle);

            // Kategori alanı
            Label lblCategory = new Label
            {
                Text = "Kategori:",
                Location = new Point(0, 75),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            leftSection.Controls.Add(lblCategory);

            cmbCategory = new ComboBox
            {
                Location = new Point(0, 100),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cmbCategory.Items.AddRange(new object[] { "Kişisel", "İş", "Okul", "Diğer" });
            cmbCategory.SelectedIndex = 0;
            leftSection.Controls.Add(cmbCategory);

            // Etiketler alanı
            Label lblTags = new Label
            {
                Text = "Etiketler (virgülle ayırın):",
                Location = new Point(0, 140),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            leftSection.Controls.Add(lblTags);

            txtTags = new TextBox
            {
                Location = new Point(0, 165),
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "örn: önemli, toplantı, proje",
                ForeColor = Color.Gray
            };
            txtTags.Enter += TextBox_Enter;
            txtTags.Leave += TextBox_Leave;
            leftSection.Controls.Add(txtTags);

            // İçerik alanı
            Label lblContent = new Label
            {
                Text = "Not İçeriği:",
                Location = new Point(0, 205),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            leftSection.Controls.Add(lblContent);

            txtContent = new TextBox
            {
                Location = new Point(0, 230),
                Size = new Size(350, 200),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true,
                Text = "Notunuzun içeriğini buraya yazınız...",
                ForeColor = Color.Gray
            };
            txtContent.Enter += TextBox_Enter;
            txtContent.Leave += TextBox_Leave;
            leftSection.Controls.Add(txtContent);

            // Butonlar
            btnSave = new Button
            {
                Text = "Notu Kaydet",
                Location = new Point(0, 450),
                Size = new Size(110, 40),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            leftSection.Controls.Add(btnSave);

            btnUpdate = new Button
            {
                Text = "Güncelle",
                Location = new Point(120, 450),
                Size = new Size(110, 40),
                BackColor = Color.FromArgb(60, 170, 230),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.Click += BtnUpdate_Click;
            leftSection.Controls.Add(btnUpdate);

            btnDelete = new Button
            {
                Text = "Sil",
                Location = new Point(240, 450),
                Size = new Size(110, 40),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;
            leftSection.Controls.Add(btnDelete);

            // Sağ bölüm (not listesi)
            Panel rightSection = new Panel
            {
                Location = new Point(380, 50),
                Size = new Size(420, 550),
                BackColor = Color.White
            };

            // Not listesi etiketi
            Label lblNotesList = new Label
            {
                Text = "Kayıtlı Notlarınız:",
                Location = new Point(0, 10),
                Size = new Size(420, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204)
            };
            rightSection.Controls.Add(lblNotesList);

            lstNotes = new ListBox
            {
                Location = new Point(0, 40),
                Size = new Size(420, 510),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            lstNotes.SelectedIndexChanged += LstNotes_SelectedIndexChanged;
            rightSection.Controls.Add(lstNotes);

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

        private void NotesForm_FormClosing(object sender, FormClosingEventArgs e)
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
            if (string.IsNullOrWhiteSpace(txtContent.Text))
            {
                MessageBox.Show("Lütfen not içeriğini girin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            notes = DataStorage.LoadNotes();
            var note = new Note(
                id: notes.Count + 1,
                userId: currentUser.Id,
                title: txtTitle.Text,
                content: txtContent.Text,
                category: cmbCategory.SelectedItem?.ToString(),
                tags: txtTags.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            );

            DataStorage.AddNote(note);
            LoadNotes();
            ClearFields();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (lstNotes.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz notu seçin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtContent.Text))
            {
                MessageBox.Show("Lütfen not içeriğini girin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var notes = DataStorage.LoadNotes();
            var selectedNote = notes[lstNotes.SelectedIndex];

            if (selectedNote.UserId != currentUser.Id)
            {
                MessageBox.Show("Sadece kendi notlarınızı güncelleyebilirsiniz!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            selectedNote.Title = txtTitle.Text;
            selectedNote.Content = txtContent.Text;
            selectedNote.Category = cmbCategory.SelectedItem?.ToString();
            selectedNote.Tags = txtTags.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            selectedNote.ModifiedDate = DateTime.Now;

            DataStorage.SaveNotes(notes);
            LoadNotes();
            ClearFields();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lstNotes.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen silmek istediğiniz notu seçin!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var notes = DataStorage.LoadNotes();
            var selectedNote = notes[lstNotes.SelectedIndex];

            if (selectedNote.UserId != currentUser.Id)
            {
                MessageBox.Show("Sadece kendi notlarınızı silebilirsiniz!", "Uyarı", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            notes.RemoveAt(lstNotes.SelectedIndex);
            DataStorage.SaveNotes(notes);
            LoadNotes();
            ClearFields();
        }

        private void LstNotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstNotes.SelectedIndex != -1)
            {
                var selectedNote = notes.Where(n => n.UserId == currentUser.Id).ElementAt(lstNotes.SelectedIndex);
                txtTitle.Text = selectedNote.Title;
                txtContent.Text = selectedNote.Content;
                txtTags.Text = string.Join(",", selectedNote.Tags);
                cmbCategory.SelectedItem = selectedNote.Category;
            }
        }

        private void LoadNotes()
        {
            notes = DataStorage.LoadNotes();
            lstNotes.Items.Clear();
            foreach (var note in notes.Where(n => n.UserId == currentUser.Id))
            {
                lstNotes.Items.Add($"{note.Title} - {note.Category} ({note.CreatedDate:dd.MM.yyyy})");
            }
        }

        private void ClearFields()
        {
            txtTitle.Clear();
            txtContent.Clear();
            txtTags.Clear();
            cmbCategory.SelectedIndex = 0;
        }

        // Placeholder textbox'a tıklandığında içeriği silmek için
        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.ForeColor == System.Drawing.Color.Gray)
            {
                textBox.Text = "";
                textBox.ForeColor = System.Drawing.Color.Black;
            }
        }

        // Placeholder textbox'tan çıkıldığında ve içerik boşsa placeholder metni göstermek için
        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox == txtTitle)
                {
                    textBox.Text = "Notunuzun başlığını girin";
                }
                else if (textBox == txtTags)
                {
                    textBox.Text = "örn: önemli, toplantı, proje";
                }
                else if (textBox == txtContent)
                {
                    textBox.Text = "Notunuzun içeriğini buraya yazınız...";
                }
                
                textBox.ForeColor = System.Drawing.Color.Gray;
            }
        }
    }
}