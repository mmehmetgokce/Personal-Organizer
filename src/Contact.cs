using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace PersonalOrganizer
{
    public class Contact
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        
        private string _phone;
        public string Phone 
        { 
            get { return _phone; } 
            set { _phone = FormatPhoneNumber(value); }
        }
        
        private string _email;
        public string Email 
        { 
            get { return _email; } 
            set { _email = value; }
        }
        
        public string Address { get; set; }
        public string Company { get; set; }
        public string Notes { get; set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
        public Contact()
        {
            UserId = string.Empty;
            Name = string.Empty;
            _phone = string.Empty;
            _email = string.Empty;
            Address = string.Empty;
            Company = string.Empty;
            Notes = string.Empty;
            Category = "Genel";
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }
        
        public Contact(string userId, string name, string phone, string email)
        {
            UserId = userId;
            Name = name;
            _phone = FormatPhoneNumber(phone);
            _email = IsValidEmail(email) ? email : string.Empty;
            Address = string.Empty;
            Company = string.Empty;
            Notes = string.Empty;
            Category = "Genel";
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Id}|{UserId}|{Name}|{Phone}|{Email}|{Address}|{Company}|{Notes}|{Category}|{CreatedDate}|{ModifiedDate}";
        }

        public static Contact FromString(string data)
        {
            string[] fields = data.Split('|');
            
            if (fields.Length < 11)
            {
                throw new FormatException("Geçersiz kişi veri formatı.");
            }

            Contact contact = new Contact
            {
                Id = int.Parse(fields[0]),
                UserId = fields[1],
                Name = fields[2],
                Phone = fields[3],
                Email = fields[4],
                Address = fields[5],
                Company = fields[6],
                Notes = fields[7],
                Category = fields[8],
                CreatedDate = DateTime.Parse(fields[9]),
                ModifiedDate = DateTime.Parse(fields[10])
            };

            return contact;
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            // Sadece rakamları al
            string digits = Regex.Replace(phoneNumber, @"[^\d]", "");
            
            // 10 haneli değilse ve boş değilse format uygula
            if (digits.Length == 10)
            {
                return $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6, 4)}";
            }
            else if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return string.Empty;
            }
            
            // Zaten formatlanmış olabilir
            return phoneNumber;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }
        
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            
            // İsimde rakam olmamalı
            return name.All(c => !char.IsDigit(c));
        }
    }
} 