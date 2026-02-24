using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace PersonalOrganizer
{
    public class ReminderForm : Form
    {
        private ComboBox cmbType;
        private DateTimePicker dtpDateTime;
        private TextBox txtSummary;
        private TextBox txtDescription;
        private TextBox txtLocation;
        private TextBox txtAttendees;
        private ComboBox cmbPriority;
        private ComboBox cmbStatus;
        private Button btnSave;
        private Button btnCancel;
        private User currentUser;
        private Reminder currentReminder;
        private bool isEditMode;

        public ReminderForm(User user, Reminder reminder = null)
        {
            currentUser = user;
            currentReminder = reminder;
            isEditMode = reminder != null;
            InitializeComponents();
            if (isEditMode)
            {
                LoadReminderData();
            }
        }

        private void InitializeComponents()
        {
            this.Text = isEditMode ? "Hatırlatıcı Düzenle" : "Yeni Hatırlatıcı";
            this.Size = new Size(550, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Form başlığı
            Label lblTitle = new Label
            {
                Text = isEditMode ? "Hatırlatıcı Düzenle" : "Yeni Hatırlatıcı Oluştur",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(20, 20),
                Size = new Size(510, 40)
            };
            this.Controls.Add(lblTitle);

            // Hatırlatıcı tipi
            var lblType = new Label
            {
                Text = "Hatırlatıcı Tipi:",
                Location = new Point(20, 80),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(lblType);

            cmbType = new ComboBox
            {
                Location = new Point(180, 80),
                Size = new Size(330, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbType.Items.AddRange(Enum.GetNames(typeof(ReminderType)));
            cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged;
            this.Controls.Add(cmbType);

            // Tarih ve saat
            var lblDateTime = new Label
            {
                Text = "Tarih ve Saat:",
                Location = new Point(20, 120),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(lblDateTime);

            dtpDateTime = new DateTimePicker
            {
                Location = new Point(180, 120),
                Size = new Size(330, 30),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd.MM.yyyy HH:mm",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(dtpDateTime);

            // Özet
            var lblSummary = new Label
            {
                Text = "Özet:",
                Location = new Point(20, 160),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(lblSummary);

            txtSummary = new TextBox
            {
                Location = new Point(180, 160),
                Size = new Size(330, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtSummary);

            // Açıklama
            var lblDescription = new Label
            {
                Text = "Açıklama:",
                Location = new Point(20, 200),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(lblDescription);

            txtDescription = new TextBox
            {
                Location = new Point(180, 200),
                Size = new Size(330, 100),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true
            };
            this.Controls.Add(txtDescription);

            // Toplantı alanları
            var lblLocation = new Label
            {
                Text = "Konum:",
                Location = new Point(20, 320),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(lblLocation);

            txtLocation = new TextBox
            {
                Location = new Point(180, 320),
                Size = new Size(330, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtLocation);

            var lblAttendees = new Label
            {
                Text = "Katılımcılar:",
                Location = new Point(20, 360),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(lblAttendees);

            txtAttendees = new TextBox
            {
                Location = new Point(180, 360),
                Size = new Size(330, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(txtAttendees);

            // Görev alanları
            var lblPriority = new Label
            {
                Text = "Öncelik:",
                Location = new Point(20, 400),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(lblPriority);

            cmbPriority = new ComboBox
            {
                Location = new Point(180, 400),
                Size = new Size(330, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbPriority.Items.AddRange(new[] { "Düşük", "Orta", "Yüksek" });
            this.Controls.Add(cmbPriority);

            var lblStatus = new Label
            {
                Text = "Durum:",
                Location = new Point(20, 440),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(lblStatus);

            cmbStatus = new ComboBox
            {
                Location = new Point(180, 440),
                Size = new Size(330, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat
            };
            cmbStatus.Items.AddRange(new[] { "Beklemede", "Devam Ediyor", "Tamamlandı" });
            this.Controls.Add(cmbStatus);

            // Butonlar
            btnSave = new Button
            {
                Text = "Kaydet",
                Location = new Point(300, 490),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "İptal",
                Location = new Point(410, 490),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            // Varsayılan değerleri ayarla
            cmbType.SelectedIndex = 0;
            dtpDateTime.Value = DateTime.Now;
            cmbPriority.SelectedIndex = 1;
            cmbStatus.SelectedIndex = 0;

            // Başlangıçta tip alanlarını güncelle
            UpdateTypeFields();
        }

        private void CmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTypeFields();
        }

        private void UpdateTypeFields()
        {
            bool isMeeting = cmbType.SelectedItem?.ToString() == ReminderType.Meeting.ToString();

            txtLocation.Visible = isMeeting;
            txtAttendees.Visible = isMeeting;
            cmbPriority.Visible = !isMeeting;
            cmbStatus.Visible = !isMeeting;

            if (isMeeting)
            {
                txtLocation.Text = "";
                txtAttendees.Text = "";
            }
            else
            {
                cmbPriority.SelectedIndex = 1;
                cmbStatus.SelectedIndex = 0;
            }
        }

        private void LoadReminderData()
        {
            cmbType.SelectedItem = currentReminder.Type.ToString();
            dtpDateTime.Value = currentReminder.DateTime;
            txtSummary.Text = currentReminder.Summary;
            txtDescription.Text = currentReminder.Description;

            if (currentReminder.Type == ReminderType.Meeting)
            {
                txtLocation.Text = currentReminder.Location ?? "";
                txtAttendees.Text = string.Join(", ", currentReminder.Attendees ?? Array.Empty<string>());
            }
            else if (currentReminder.Type == ReminderType.Task)
            {
                cmbPriority.SelectedItem = currentReminder.Priority.ToString();
                cmbStatus.SelectedItem = currentReminder.Status;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSummary.Text))
            {
                MessageBox.Show("Lütfen özet girin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var type = (ReminderType)Enum.Parse(typeof(ReminderType), cmbType.SelectedItem.ToString());
                var factory = ReminderFactoryProvider.GetFactory(type);
                var reminder = factory.CreateReminder(int.Parse(currentUser.UserId));

                reminder.DateTime = dtpDateTime.Value;
                reminder.Summary = txtSummary.Text;
                reminder.Description = txtDescription.Text;
                reminder.IsActive = true;

                if (type == ReminderType.Meeting)
                {
                    reminder.Location = txtLocation.Text;
                    reminder.Attendees = txtAttendees.Text.Split(',').Select(s => s.Trim()).ToArray();
                }
                else
                {
                    reminder.Priority = cmbPriority.SelectedIndex;
                    reminder.Status = cmbStatus.SelectedItem.ToString();
                }

                var reminders = DataStorage.LoadReminders();
                if (isEditMode)
                {
                    var index = reminders.FindIndex(r => r.Id == currentReminder.Id);
                    if (index != -1)
                    {
                        reminders[index] = reminder;
                    }
                }
                else
                {
                    reminder.Id = reminders.Count > 0 ? reminders.Max(r => r.Id) + 1 : 1;
                    reminders.Add(reminder);
                }

                DataStorage.SaveReminders(reminders);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hatırlatıcı kaydedilirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
} 