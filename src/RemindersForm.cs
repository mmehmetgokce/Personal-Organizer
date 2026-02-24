using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace PersonalOrganizer
{
    public class RemindersForm : Form
    {
        private ListView listViewReminders;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private User currentUser;
        private ReminderSubject reminderSubject;
        private Timer reminderCheckTimer;

        public RemindersForm(User user)
        {
            currentUser = user;
            InitializeComponents();
            LoadReminders();
            SetupReminderCheck();
        }

        private void InitializeComponents()
        {
            this.Text = "Hatırlatıcılar - Kişisel Organizatör";
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
                Text = "Hatırlatıcılar",
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
                Text = "Önemli tarihleri ve görevleri\nhiçbir zaman unutmayın,\ntüm hatırlatıcılarınızı düzenleyin.",
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
                Text = "Hatırlatıcı Listeniz",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(0, 0),
                Size = new Size(800, 40)
            };
            formContainer.Controls.Add(lblTitle);

            // ListView
            listViewReminders = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(0, 50),
                Size = new Size(800, 480),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            listViewReminders.Columns.AddRange(new ColumnHeader[]
            {
                new ColumnHeader { Text = "ID", Width = 50 },
                new ColumnHeader { Text = "Tür", Width = 120 },
                new ColumnHeader { Text = "Özet", Width = 250 },
                new ColumnHeader { Text = "Tarih", Width = 150 },
                new ColumnHeader { Text = "Saat", Width = 100 },
                new ColumnHeader { Text = "Durum", Width = 130 }
            });
            formContainer.Controls.Add(listViewReminders);

            // Butonlar paneli
            Panel buttonPanel = new Panel
            {
                Location = new Point(0, 540),
                Size = new Size(800, 60),
                BackColor = Color.White
            };

            // Butonlar
            btnAdd = new Button
            {
                Text = "Yeni Hatırlatıcı",
                Location = new Point(0, 10),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;
            buttonPanel.Controls.Add(btnAdd);

            btnEdit = new Button
            {
                Text = "Düzenle",
                Location = new Point(160, 10),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(60, 170, 230),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += BtnEdit_Click;
            buttonPanel.Controls.Add(btnEdit);

            btnDelete = new Button
            {
                Text = "Sil",
                Location = new Point(320, 10),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += BtnDelete_Click;
            buttonPanel.Controls.Add(btnDelete);

            formContainer.Controls.Add(buttonPanel);

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

            // Observer pattern için subject oluştur
            reminderSubject = new ReminderSubject(this);
            var observer = new ReminderObserver(this);
            reminderSubject.Attach(observer);
        }

        private void SetupReminderCheck()
        {
            reminderCheckTimer = new Timer();
            reminderCheckTimer.Interval = 60000; // Her dakika kontrol et
            reminderCheckTimer.Tick += ReminderCheckTimer_Tick;
            reminderCheckTimer.Start();
        }

        private void ReminderCheckTimer_Tick(object sender, EventArgs e)
        {
            var reminders = DataStorage.LoadReminders()
                .Where(r => r.UserId.ToString() == currentUser.UserId && r.IsActive)
                .ToList();

            foreach (var reminder in reminders)
            {
                if (reminder.DateTime <= DateTime.Now)
                {
                    reminderSubject.Notify(reminder.Summary);
                    reminder.IsActive = false;
                }
            }

            // Değişiklikleri kaydet
            if (reminders.Any(r => !r.IsActive))
            {
                DataStorage.SaveReminders(reminders);
                LoadReminders(); // Listeyi güncelle
            }
        }

        private void LoadReminders()
        {
            listViewReminders.Items.Clear();
            var reminders = DataStorage.LoadReminders()
                .Where(r => r.UserId.ToString() == currentUser.UserId)
                .OrderBy(r => r.DateTime);
                
            foreach (var reminder in reminders)
            {
                var item = new ListViewItem(new[]
                {
                    reminder.Id.ToString(),
                    reminder.Type.ToString(),
                    reminder.Summary,
                    reminder.DateTime.ToShortDateString(),
                    reminder.DateTime.ToShortTimeString(),
                    reminder.IsActive ? "Aktif" : "Tamamlandı"
                });
                item.Tag = reminder;
                listViewReminders.Items.Add(item);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new ReminderForm(currentUser))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadReminders();
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (listViewReminders.SelectedItems.Count == 0)
            {
                MessageBox.Show("Lütfen düzenlenecek hatırlatıcıyı seçin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var reminder = (Reminder)listViewReminders.SelectedItems[0].Tag;
            using (var form = new ReminderForm(currentUser, reminder))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadReminders();
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (listViewReminders.SelectedItems.Count == 0)
            {
                MessageBox.Show("Lütfen silinecek hatırlatıcıyı seçin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Seçili hatırlatıcıyı silmek istediğinizden emin misiniz?",
                "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var reminder = (Reminder)listViewReminders.SelectedItems[0].Tag;
                var reminders = DataStorage.LoadReminders();
                reminders.RemoveAll(r => r.Id == reminder.Id);
                DataStorage.SaveReminders(reminders);
                LoadReminders();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            reminderCheckTimer.Stop();
        }
    }
}