using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text;

namespace PersonalOrganizer
{
    public static class DataStorage
    {
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string DataDirectory = Path.Combine(BaseDirectory, "Data");
        private static readonly string UsersFile = Path.Combine(DataDirectory, "users.txt");
        private static readonly string PhonebookFile = Path.Combine(DataDirectory, "phonebook.txt");
        private static readonly string NotesFile = Path.Combine(DataDirectory, "notes.txt");
        private static readonly string RemindersFile = Path.Combine(DataDirectory, "reminders.txt");
        private static readonly string PersonalInfoFile = Path.Combine(DataDirectory, "personalinfo.txt");
        private static readonly string SalaryDataFile = Path.Combine(DataDirectory, "salarydata.txt");

        // Uygulama ilk çalıştığında gerekli dizinleri ve dosyaları oluştur
        static DataStorage()
        {
            EnsureDirectoryExists();
        }

        private static void EnsureDirectoryExists()
        {
            if (!Directory.Exists(DataDirectory))
            {
                Directory.CreateDirectory(DataDirectory);
            }

            // Dosyaların varlığını kontrol et ve gerekirse oluştur
            if (!File.Exists(UsersFile))
            {
                // Varsayılan bir admin kullanıcısı oluştur
                User admin = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    Username = "admin",
                    Password = "admin123",
                    Email = "admin@example.com",
                    Role = UserRole.Admin,
                    CreatedDate = DateTime.Now,
                    LastLoginDate = DateTime.MinValue
                };
                
                List<User> users = new List<User> { admin };
                SaveUsers(users);
            }

            EnsureFileExists(PhonebookFile);
            EnsureFileExists(NotesFile);
            EnsureFileExists(RemindersFile);
            EnsureFileExists(PersonalInfoFile);
            EnsureFileExists(SalaryDataFile);
        }

        private static void EnsureFileExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        #region Kullanıcı İşlemleri

        public static List<User> LoadUsers()
        {
            List<User> users = new List<User>();

            if (File.Exists(UsersFile))
            {
                try
                {
                    string[] lines = File.ReadAllLines(UsersFile);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] parts = line.Split('|');
                            if (parts.Length >= 6)
                            {
                                User user = new User
                                {
                                    UserId = parts[0],
                                    Username = parts[1],
                                    Password = parts[2],
                                    Email = parts[3],
                                    Role = (UserRole)Enum.Parse(typeof(UserRole), parts[4]),
                                    CreatedDate = DateTime.Parse(parts[5]),
                                    LastLoginDate = parts.Length > 6 && !string.IsNullOrEmpty(parts[6]) ? 
                                        DateTime.Parse(parts[6]) : DateTime.MinValue
                                };
                                users.Add(user);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Kullanıcı verileri yüklenirken hata oluştu: {ex.Message}");
                }
            }

            return users;
        }

        public static void SaveUsers(List<User> users)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (User user in users)
                {
                    string line = $"{user.UserId}|{user.Username}|{user.Password}|{user.Email}|{user.Role}|{user.CreatedDate}|{user.LastLoginDate}";
                    lines.Add(line);
                }
                File.WriteAllLines(UsersFile, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Kullanıcı verileri kaydedilirken hata oluştu: {ex.Message}");
            }
        }

        public static User AuthenticateUser(string username, string password)
        {
            List<User> users = LoadUsers();
            User user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            
            if (user != null)
            {
                // Kullanıcı giriş tarihini güncelle
                user.LastLoginDate = DateTime.Now;
                SaveUsers(users);
            }
            
            return user;
        }

        public static bool RegisterUser(User newUser)
        {
            List<User> users = LoadUsers();
            
            // Kullanıcı adı kontrolü
            if (users.Any(u => u.Username.Equals(newUser.Username, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            
            users.Add(newUser);
            SaveUsers(users);
            return true;
        }

        #endregion

        #region Kişi İşlemleri

        public static List<Contact> LoadContacts()
        {
            List<Contact> contacts = new List<Contact>();
            string csvFilePath = Path.Combine(DataDirectory, "contacts.csv");

            // CSV dosyası yoksa, eski dosyadan yüklemeyi dene
            if (!File.Exists(csvFilePath) && File.Exists(PhonebookFile))
            {
                try
                {
                    // Eski formattan yükle ve CSV olarak kaydet
                    string[] lines = File.ReadAllLines(PhonebookFile);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            try
                            {
                                Contact contact = Contact.FromString(line);
                                contacts.Add(contact);
                            }
                            catch (FormatException ex)
                            {
                                Console.WriteLine($"Kişi verisi ayrıştırılırken hata oluştu: {ex.Message}");
                                
                                // Temel verileri çıkarmak için alternatif yöntem
                                string[] parts = line.Split('|');
                                if (parts.Length >= 4)
                                {
                                    Contact contact = new Contact(parts[1], parts[2], parts[3], parts.Length > 4 ? parts[4] : "");
                                    if (int.TryParse(parts[0], out int id))
                                    {
                                        contact.Id = id;
                                    }
                                    contacts.Add(contact);
                                }
                            }
                        }
                    }
                    
                    // CSV'ye dönüştür ve kaydet
                    SaveContactsAsCsv(contacts, csvFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Rehber verileri yüklenirken hata oluştu: {ex.Message}");
                }
            }
            else if (File.Exists(csvFilePath))
            {
                try
                {
                    // CSV dosyasından yükle
                    string[] lines = File.ReadAllLines(csvFilePath);
                    
                    // İlk satır başlık satırı olduğu için atla
                    if (lines.Length > 1)
                    {
                        for (int i = 1; i < lines.Length; i++)
                        {
                            string line = lines[i];
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                try
                                {
                                    Contact contact = ContactFromCsvLine(line);
                                    contacts.Add(contact);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"CSV satırı ayrıştırılırken hata oluştu: {ex.Message}, Satır: {line}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CSV verileri yüklenirken hata oluştu: {ex.Message}");
                    MessageBox.Show($"Kişi verileri yüklenirken hata oluştu: {ex.Message}", "Hata", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return contacts;
        }

        private static Contact ContactFromCsvLine(string csvLine)
        {
            // CSV satırını virgülle ayır, tırnak içindeki virgülleri koru
            List<string> values = new List<string>();
            bool inQuotes = false;
            StringBuilder field = new StringBuilder();
            
            foreach (char c in csvLine)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(field.ToString());
                    field.Clear();
                }
                else
                {
                    field.Append(c);
                }
            }
            
            // Son alanı ekle
            values.Add(field.ToString());
            
            // Tırnak işaretlerini temizle
            for (int i = 0; i < values.Count; i++)
            {
                values[i] = values[i].Trim('"');
            }
            
            Contact contact = new Contact();
            
            // CSV alanlarını Contact nesnesine eşle
            if (values.Count >= 11)
            {
                contact.Id = int.Parse(values[0]);
                contact.UserId = values[1];
                contact.Name = values[2];
                contact.Phone = values[3];
                contact.Email = values[4];
                contact.Address = values[5];
                contact.Company = values[6];
                contact.Notes = values[7];
                contact.Category = values[8];
                contact.CreatedDate = DateTime.Parse(values[9]);
                contact.ModifiedDate = DateTime.Parse(values[10]);
            }
            
            return contact;
        }

        public static void SaveContacts(List<Contact> contacts)
        {
            string csvFilePath = Path.Combine(DataDirectory, "contacts.csv");
            
            try
            {
                // CSV dosyasına kaydet
                SaveContactsAsCsv(contacts, csvFilePath);
                
                // Geriye uyumluluk için eski formatta da kaydet
                List<string> lines = new List<string>();
                foreach (Contact contact in contacts)
                {
                    string line = contact.ToString();
                    lines.Add(line);
                }
                File.WriteAllLines(PhonebookFile, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Rehber verileri kaydedilirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"Rehber verileri kaydedilirken hata oluştu: {ex.Message}");
            }
        }
        
        private static void SaveContactsAsCsv(List<Contact> contacts, string filePath)
        {
            List<string> csvLines = new List<string>();
            
            // CSV başlık satırı
            csvLines.Add("Id,UserId,Name,Phone,Email,Address,Company,Notes,Category,CreatedDate,ModifiedDate");
            
            // Kişileri CSV satırlarına dönüştür
            foreach (Contact contact in contacts)
            {
                string csvLine = $"{contact.Id}," +
                                $"\"{EscapeCsvField(contact.UserId)}\"," +
                                $"\"{EscapeCsvField(contact.Name)}\"," +
                                $"\"{EscapeCsvField(contact.Phone)}\"," +
                                $"\"{EscapeCsvField(contact.Email)}\"," +
                                $"\"{EscapeCsvField(contact.Address)}\"," +
                                $"\"{EscapeCsvField(contact.Company)}\"," +
                                $"\"{EscapeCsvField(contact.Notes)}\"," +
                                $"\"{EscapeCsvField(contact.Category)}\"," +
                                $"\"{contact.CreatedDate}\"," +
                                $"\"{contact.ModifiedDate}\"";
                csvLines.Add(csvLine);
            }
            
            File.WriteAllLines(filePath, csvLines);
        }
        
        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;
                
            // Tırnak işaretlerini çift tırnak olarak kaçır
            return field.Replace("\"", "\"\"");
        }

        #endregion

        #region Not İşlemleri

        public static List<Note> LoadNotes()
        {
            List<Note> notes = new List<Note>();

            if (File.Exists(NotesFile))
            {
                try
                {
                    string[] lines = File.ReadAllLines(NotesFile);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] parts = line.Split('|');
                            if (parts.Length >= 7)
                            {
                                int id = int.Parse(parts[0]);
                                int userId = int.Parse(parts[1]);
                                string title = parts[2];
                                string content = parts[3];
                                string category = parts[4];
                                string[] tags = parts[5].Split(',');
                                DateTime createdDate = DateTime.Parse(parts[6]);
                                
                                Note note = new Note(id, userId, title, content, category, tags);
                                note.CreatedDate = createdDate;
                                
                                if (parts.Length > 7 && DateTime.TryParse(parts[7], out DateTime modifiedDate))
                                {
                                    note.ModifiedDate = modifiedDate;
                                }
                                
                                if (parts.Length > 8)
                                {
                                    bool isArchived;
                                    if (bool.TryParse(parts[8], out isArchived))
                                    {
                                        note.IsArchived = isArchived;
                                    }
                                }
                                
                                if (parts.Length > 9)
                                {
                                    note.Username = parts[9];
                                }
                                
                                notes.Add(note);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Not verileri yüklenirken hata oluştu: {ex.Message}");
                }
            }

            return notes;
        }

        public static void SaveNotes(List<Note> notes)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Note note in notes)
                {
                    string line = $"{note.Id}|{note.UserId}|{note.Title}|{note.Content}|{note.Category}|{string.Join(",", note.Tags)}|{note.CreatedDate}|{note.ModifiedDate}|{note.IsArchived}|{note.Username}";
                    lines.Add(line);
                }
                File.WriteAllLines(NotesFile, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Not verileri kaydedilirken hata oluştu: {ex.Message}");
            }
        }

        public static void AddNote(Note note)
        {
            List<Note> notes = LoadNotes();
            
            // Yeni not için ID ataması yapılmadıysa yap
            if (note.Id == 0 && notes.Count > 0)
            {
                note.Id = notes.Max(n => n.Id) + 1;
            }
            else if (note.Id == 0)
            {
                note.Id = 1;
            }
            
            notes.Add(note);
            SaveNotes(notes);
        }

        #endregion

        #region Kişisel Bilgi İşlemleri

        public static List<PersonalInfo> LoadPersonalInfo()
        {
            List<PersonalInfo> personalInfoList = new List<PersonalInfo>();

            if (File.Exists(PersonalInfoFile))
            {
                try
                {
                    string[] lines = File.ReadAllLines(PersonalInfoFile);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            try
                            {
                                // Sıralamanın PersonalInfo.ToString() metodu ile aynı olduğundan emin oluyoruz
                                PersonalInfo personalInfo = PersonalInfo.FromString(line);
                                if (personalInfo != null)
                                {
                                    personalInfoList.Add(personalInfo);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Kişisel bilgi verisi ayrıştırılırken hata oluştu: {ex.Message}");
                                
                                // Manuel ayrıştırma yöntemi (geri uyumluluk için)
                                string[] parts = line.Split('|');
                                if (parts.Length >= 3)
                                {
                                    PersonalInfo personalInfo = new PersonalInfo
                                    {
                                        Id = int.TryParse(parts[0], out int id) ? id : 0,
                                        UserId = parts[1],
                                        Username = parts.Length > 2 ? parts[2] : string.Empty,
                                        Name = parts.Length > 3 ? parts[3] : string.Empty,
                                        Surname = parts.Length > 4 ? parts[4] : string.Empty,
                                        Phone = parts.Length > 5 ? parts[5] : string.Empty,
                                        Address = parts.Length > 6 ? parts[6] : string.Empty,
                                        Email = parts.Length > 7 ? parts[7] : string.Empty,
                                        BirthDate = parts.Length > 8 && DateTime.TryParse(parts[8], out DateTime birthDate) ? birthDate : DateTime.MinValue,
                                        City = parts.Length > 9 ? parts[9] : string.Empty,
                                        Country = parts.Length > 10 ? parts[10] : string.Empty,
                                        EmergencyContact = parts.Length > 11 ? parts[11] : string.Empty,
                                        MedicalInfo = parts.Length > 12 ? parts[12] : string.Empty,
                                        ProfilePhotoBase64 = parts.Length > 13 ? parts[13] : string.Empty
                                    };
                                    personalInfoList.Add(personalInfo);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Kişisel bilgi verileri yüklenirken hata oluştu: {ex.Message}");
                }
            }

            return personalInfoList;
        }

        public static void SavePersonalInfo(List<PersonalInfo> personalInfoList)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (PersonalInfo info in personalInfoList)
                {
                    string line = $"{info.Id}|{info.UserId}|{info.Username}|{info.Name}|{info.Surname}|{info.Phone}|{info.Address}|{info.Email}|{info.BirthDate}|{info.City}|{info.Country}|{info.EmergencyContact}|{info.MedicalInfo}|{info.ProfilePhotoBase64}|{info.CreatedDate}|{info.ModifiedDate}|{info.ExperienceYears}|{info.EducationLevel}|{info.ResponsibilityLevel}|{info.PerformanceLevel}|{info.Salary}";
                    lines.Add(line);
                }
                File.WriteAllLines(PersonalInfoFile, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kişisel bilgiler kaydedilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void AddPersonalInfo(PersonalInfo info)
        {
            List<PersonalInfo> infoList = LoadPersonalInfo();
            
            // Yeni kişisel bilgi için ID ataması yapılmadıysa yap
            if (info.Id == 0 && infoList.Count > 0)
            {
                info.Id = infoList.Max(p => p.Id) + 1;
            }
            else if (info.Id == 0)
            {
                info.Id = 1;
            }
            
            infoList.Add(info);
            SavePersonalInfo(infoList);
        }

        #endregion

        #region Hatırlatıcı İşlemleri

        public static List<Reminder> LoadReminders()
        {
            List<Reminder> reminders = new List<Reminder>();

            if (File.Exists(RemindersFile))
            {
                try
                {
                    string[] lines = File.ReadAllLines(RemindersFile);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] parts = line.Split('|');
                            if (parts.Length >= 7)
                            {
                                int id = int.Parse(parts[0]);
                                int userId = int.Parse(parts[1]);
                                string summary = parts[2];
                                string description = parts[3];
                                DateTime date = DateTime.Parse(parts[4]);
                                TimeSpan time = TimeSpan.Parse(parts[5]);
                                bool isActive = bool.Parse(parts[6]);
                                
                                Reminder reminder = new Reminder
                                {
                                    Id = id,
                                    UserId = userId,
                                    Summary = summary,
                                    Description = description,
                                    DateTime = date.Add(time),
                                    IsActive = isActive
                                };
                                
                                if (parts.Length > 7 && Enum.TryParse(parts[7], out ReminderType type))
                                {
                                    reminder.Type = type;
                                }
                                
                                if (parts.Length > 8 && int.TryParse(parts[8], out int priority))
                                {
                                    reminder.Priority = priority;
                                }
                                
                                if (parts.Length > 9)
                                {
                                    reminder.Recurrence = parts[9];
                                }
                                
                                if (parts.Length > 10 && bool.TryParse(parts[10], out bool isCompleted))
                                {
                                    reminder.IsCompleted = isCompleted;
                                }
                                
                                reminders.Add(reminder);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Hatırlatıcı verileri yüklenirken hata oluştu: {ex.Message}");
                }
            }

            return reminders;
        }

        public static void SaveReminders(List<Reminder> reminders)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Reminder reminder in reminders)
                {
                    string line = $"{reminder.Id}|{reminder.UserId}|{reminder.Summary}|{reminder.Description}|{reminder.DateTime.Date}|{reminder.DateTime.TimeOfDay}|{reminder.IsActive}|{reminder.Type}|{reminder.Priority}|{reminder.Recurrence}|{reminder.IsCompleted}";
                    lines.Add(line);
                }
                File.WriteAllLines(RemindersFile, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hatırlatıcı verileri kaydedilirken hata oluştu: {ex.Message}");
            }
        }

        #endregion

        #region Maaş Hesaplama İşlemleri

        public static void SaveSalaryData(string userId, decimal salary, Dictionary<string, decimal> components)
        {
            try
            {
                string componentString = string.Join(",", components.Select(kv => $"{kv.Key}:{kv.Value}"));
                string line = $"{userId}|{salary}|{componentString}|{DateTime.Now}";
                
                List<string> lines = new List<string>();
                if (File.Exists(SalaryDataFile))
                {
                    // Eski verileri filtrele (aynı kullanıcıya ait olanları çıkar)
                    lines = File.ReadAllLines(SalaryDataFile)
                        .Where(l => !l.StartsWith($"{userId}|"))
                        .ToList();
                }
                
                lines.Add(line);
                File.WriteAllLines(SalaryDataFile, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Maaş verileri kaydedilirken hata oluştu: {ex.Message}");
            }
        }

        public static Tuple<decimal, Dictionary<string, decimal>, DateTime> LoadSalaryData(string userId)
        {
            if (File.Exists(SalaryDataFile))
            {
                try
                {
                    string[] lines = File.ReadAllLines(SalaryDataFile);
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line) && line.StartsWith($"{userId}|"))
                        {
                            string[] parts = line.Split('|');
                            if (parts.Length >= 4)
                            {
                                decimal salary = decimal.Parse(parts[1]);
                                Dictionary<string, decimal> components = new Dictionary<string, decimal>();
                                
                                string[] componentParts = parts[2].Split(',');
                                foreach (string componentPart in componentParts)
                                {
                                    string[] kvp = componentPart.Split(':');
                                    if (kvp.Length == 2)
                                    {
                                        components.Add(kvp[0], decimal.Parse(kvp[1]));
                                    }
                                }
                                
                                DateTime calculationDate = DateTime.Parse(parts[3]);
                                return new Tuple<decimal, Dictionary<string, decimal>, DateTime>(
                                    salary, components, calculationDate);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Maaş verileri yüklenirken hata oluştu: {ex.Message}");
                }
            }

            return null;
        }

        #endregion

        #region Şifre Sıfırlama İşlemleri

        // Aktif şifre sıfırlama kodlarını saklayacak koleksiyon
        private static Dictionary<string, PasswordResetInfo> passwordResetCodes = new Dictionary<string, PasswordResetInfo>();

        // Şifre sıfırlama bilgilerini tutan sınıf
        private class PasswordResetInfo
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public string ResetCode { get; set; }
            public DateTime ExpirationTime { get; set; }
        }

        /// <summary>
        /// E-posta adresine göre kullanıcıyı bulur
        /// </summary>
        public static User FindUserByEmail(string email)
        {
            List<User> users = LoadUsers();
            return users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Yeni bir şifre sıfırlama kodu oluşturur ve kaydeder
        /// </summary>
        public static string GeneratePasswordResetCode(User user)
        {
            // Önceki kodları temizle
            CleanupExpiredResetCodes();

            // 6 haneli rastgele bir kod oluştur
            Random random = new Random();
            string resetCode = random.Next(100000, 999999).ToString();
            
            // Kod bilgilerini sakla (30 dakika geçerli)
            passwordResetCodes[resetCode] = new PasswordResetInfo
            {
                UserId = user.UserId,
                Email = user.Email,
                ResetCode = resetCode,
                ExpirationTime = DateTime.Now.AddMinutes(30)
            };
            
            return resetCode;
        }

        /// <summary>
        /// Şifre sıfırlama kodunun geçerli olup olmadığını kontrol eder
        /// </summary>
        public static bool ValidateResetCode(string resetCode, string email)
        {
            CleanupExpiredResetCodes();
            
            if (passwordResetCodes.TryGetValue(resetCode, out PasswordResetInfo info))
            {
                // Kod geçerli mi ve doğru e-posta adresi mi?
                return info.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && 
                       info.ExpirationTime > DateTime.Now;
            }
            
            return false;
        }

        /// <summary>
        /// Şifreyi sıfırlama koduna göre günceller
        /// </summary>
        public static bool ResetPassword(string resetCode, string newPassword)
        {
            CleanupExpiredResetCodes();
            
            if (passwordResetCodes.TryGetValue(resetCode, out PasswordResetInfo info))
            {
                List<User> users = LoadUsers();
                User user = users.FirstOrDefault(u => u.UserId == info.UserId);
                
                if (user != null)
                {
                    user.Password = newPassword;
                    SaveUsers(users);
                    
                    // Kodu kullanıldı olarak işaretle
                    passwordResetCodes.Remove(resetCode);
                    
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Süresi dolmuş şifre sıfırlama kodlarını temizler
        /// </summary>
        private static void CleanupExpiredResetCodes()
        {
            DateTime now = DateTime.Now;
            List<string> expiredCodes = passwordResetCodes
                .Where(kvp => kvp.Value.ExpirationTime <= now)
                .Select(kvp => kvp.Key)
                .ToList();
                
            foreach (string code in expiredCodes)
            {
                passwordResetCodes.Remove(code);
            }
        }

        #endregion
    }
} 