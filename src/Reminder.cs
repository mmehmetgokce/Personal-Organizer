using System;

namespace PersonalOrganizer
{
    public enum ReminderType
    {
        Meeting,
        Task
    }

    public class Reminder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime Date { get => DateTime.Date; set => DateTime = value.Date.Add(Time); }
        public TimeSpan Time { get => DateTime.TimeOfDay; set => DateTime = Date.Add(value); }
        public bool IsActive { get; set; }
        public string Recurrence { get; set; }
        public bool IsCompleted { get; set; }
        public int Priority { get; set; }
        public ReminderType Type { get; set; }
        public string Location { get; set; }
        public string[] Attendees { get; set; }
        public string Status { get; set; }

        public Reminder()
        {
            IsActive = true;
            IsCompleted = false;
            Recurrence = "";
            Priority = 0;
            Attendees = new string[0];
        }

        public Reminder(int id, int userId, DateTime dateTime, string summary, string description)
        {
            Id = id;
            UserId = userId;
            DateTime = dateTime;
            Summary = summary;
            Description = description;
            IsActive = true;
            IsCompleted = false;
            Recurrence = "";
            Priority = 0;
            Attendees = new string[0];
        }

        public override string ToString()
        {
            return $"{Summary} - {DateTime:dd.MM.yyyy HH:mm}";
        }
    }
} 