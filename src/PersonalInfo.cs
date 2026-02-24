using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace PersonalOrganizer
{
    public class PersonalInfo
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string EmergencyContact { get; set; }
        public string MedicalInfo { get; set; }
        public string ProfilePhotoBase64 { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ExperienceYears { get; set; }
        public string EducationLevel { get; set; }
        public string ResponsibilityLevel { get; set; }
        public string PerformanceLevel { get; set; }
        public decimal Salary { get; set; }

        public PersonalInfo()
        {
            CustomFields = new Dictionary<string, string>();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            ProfilePhotoBase64 = string.Empty;
        }

        public PersonalInfo(string userId, string name, string surname)
        {
            UserId = userId;
            Name = name;
            Surname = surname;
            BirthDate = DateTime.MinValue;
            Address = string.Empty;
            City = string.Empty;
            Country = string.Empty;
            Phone = string.Empty;
            PhoneNumber = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            EmergencyContact = string.Empty;
            MedicalInfo = string.Empty;
            ProfilePhotoBase64 = string.Empty;
            CustomFields = new Dictionary<string, string>();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        public PersonalInfo(int id, string userId, string username)
        {
            Id = id;
            UserId = userId;
            Username = username;
            Name = string.Empty;
            Surname = string.Empty;
            BirthDate = DateTime.MinValue;
            Address = string.Empty;
            City = string.Empty;
            Country = string.Empty;
            Phone = string.Empty;
            PhoneNumber = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            EmergencyContact = string.Empty;
            MedicalInfo = string.Empty;
            ProfilePhotoBase64 = string.Empty;
            CustomFields = new Dictionary<string, string>();
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        public override string ToString()
        {
            string customFieldsStr = string.Empty;
            foreach (var field in CustomFields)
            {
                customFieldsStr += $"{field.Key}:{field.Value};";
            }

            return $"{Id}|{UserId}|{Username}|{Name}|{Surname}|{Phone}|{Address}|{Email}|{BirthDate}|{City}|{Country}|{EmergencyContact}|{MedicalInfo}|{ProfilePhotoBase64}|{CreatedDate}|{ModifiedDate}|{ExperienceYears}|{EducationLevel}|{ResponsibilityLevel}|{PerformanceLevel}|{Salary}";
        }

        public static PersonalInfo FromString(string data)
        {
            string[] fields = data.Split('|');
            
            if (fields.Length < 3)
            {
                throw new FormatException("Geçersiz kişisel bilgi veri formatı.");
            }

            PersonalInfo info = new PersonalInfo
            {
                Id = int.TryParse(fields[0], out int id) ? id : 0,
                UserId = fields[1],
                Username = fields.Length > 2 ? fields[2] : string.Empty,
                Name = fields.Length > 3 ? fields[3] : string.Empty,
                Surname = fields.Length > 4 ? fields[4] : string.Empty,
                Phone = fields.Length > 5 ? fields[5] : string.Empty,
                PhoneNumber = fields.Length > 5 ? fields[5] : string.Empty,
                Address = fields.Length > 6 ? fields[6] : string.Empty,
                Email = fields.Length > 7 ? fields[7] : string.Empty,
                BirthDate = fields.Length > 8 && DateTime.TryParse(fields[8], out DateTime birthDate) ? birthDate : DateTime.MinValue,
                City = fields.Length > 9 ? fields[9] : string.Empty,
                Country = fields.Length > 10 ? fields[10] : string.Empty,
                EmergencyContact = fields.Length > 11 ? fields[11] : string.Empty,
                MedicalInfo = fields.Length > 12 ? fields[12] : string.Empty,
                ProfilePhotoBase64 = fields.Length > 13 ? fields[13] : string.Empty,
                CreatedDate = fields.Length > 14 && DateTime.TryParse(fields[14], out DateTime createdDate) ? createdDate : DateTime.Now,
                ModifiedDate = fields.Length > 15 && DateTime.TryParse(fields[15], out DateTime modifiedDate) ? modifiedDate : DateTime.Now,
                ExperienceYears = fields.Length > 16 && int.TryParse(fields[16], out int expYears) ? expYears : 0,
                EducationLevel = fields.Length > 17 ? fields[17] : string.Empty,
                ResponsibilityLevel = fields.Length > 18 ? fields[18] : string.Empty,
                PerformanceLevel = fields.Length > 19 ? fields[19] : string.Empty,
                Salary = fields.Length > 20 && decimal.TryParse(fields[20], out decimal salary) ? salary : 0
            };

            return info;
        }

        private static Dictionary<string, string> ParseCustomFields(string data)
        {
            Dictionary<string, string> fields = new Dictionary<string, string>();
            
            if (string.IsNullOrEmpty(data))
                return fields;
                
            string[] pairs = data.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                string[] keyValue = pair.Split(':');
                if (keyValue.Length == 2)
                {
                    fields[keyValue[0]] = keyValue[1];
                }
            }
            
            return fields;
        }

        public static string ImageToBase64(Image image)
        {
            if (image == null) return string.Empty;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Jpeg);
                    byte[] imageBytes = ms.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Resim Base64'e dönüştürülürken hata: {ex.Message}");
                return string.Empty;
            }
        }

        public static Image Base64ToImage(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return null;

            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Base64 resme dönüştürülürken hata: {ex.Message}");
                return null;
            }
        }
    }
}