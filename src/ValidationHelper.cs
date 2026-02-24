using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace PersonalOrganizer
{
    public static class ValidationHelper
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email);
        }

        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Sadece rakamları al
            string digits = new string(phone.Where(char.IsDigit).ToArray());
            return digits.Length == 10;
        }
    }
} 