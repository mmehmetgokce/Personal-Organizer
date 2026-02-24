using System;

namespace PersonalOrganizer
{
    public class Note
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public string[] Tags { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsArchived { get; set; }
        public string Username { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public Note(int id, int userId, string title, string content, string category, string[] tags)
        {
            Id = id;
            UserId = userId;
            Title = title;
            Content = content;
            Category = category;
            Tags = tags;
            CreatedDate = DateTime.Now;
            IsArchived = false;
        }

        public override string ToString()
        {
            return $"{Title} - {Category}";
        }

        public static Note FromString(string noteData)
        {
            string[] parts = noteData.Split(',');
            if (parts.Length >= 4)
            {
                Note note = new Note(int.Parse(parts[0]), int.Parse(parts[1]), parts[2], parts[3], parts[4], parts[5].Split(';'));
                note.CreatedDate = DateTime.Parse(parts[6]);
                if (!string.IsNullOrEmpty(parts[7]))
                {
                    note.ModifiedDate = DateTime.Parse(parts[7]);
                }
                return note;
            }
            return null;
        }
    }
} 