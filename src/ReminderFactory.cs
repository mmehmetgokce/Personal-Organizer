using System;

namespace PersonalOrganizer
{
    public abstract class ReminderFactory
    {
        public abstract Reminder CreateReminder(int userId);
    }

    public class MeetingReminderFactory : ReminderFactory
    {
        public override Reminder CreateReminder(int userId)
        {
            var reminder = new Reminder
            {
                UserId = userId,
                IsActive = true,
                Type = ReminderType.Meeting
            };
            return reminder;
        }
    }

    public class TaskReminderFactory : ReminderFactory
    {
        public override Reminder CreateReminder(int userId)
        {
            var reminder = new Reminder
            {
                UserId = userId,
                IsActive = true,
                Type = ReminderType.Task
            };
            return reminder;
        }
    }

    public static class ReminderFactoryProvider
    {
        public static ReminderFactory GetFactory(ReminderType type)
        {
            switch (type)
            {
                case ReminderType.Meeting:
                    return new MeetingReminderFactory();
                case ReminderType.Task:
                    return new TaskReminderFactory();
                default:
                    throw new ArgumentException("Geçersiz hatırlatıcı tipi");
            }
        }
    }
} 