using System;
using System.Collections.Generic;

namespace PersonalOrganizer
{
    public enum UserRole
    {
        Admin,
        User,
        PartTimeUser
    }

    public class User
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastLoginDate { get; set; }

        public User()
        {
            UserId = Guid.NewGuid().ToString();
            CreatedDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{UserId}|{Username}|{Password}|{Email}|{Role}|{CreatedDate}|{LastLoginDate}";
        }
    }
} 