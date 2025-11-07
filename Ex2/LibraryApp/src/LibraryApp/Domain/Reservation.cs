using System;

namespace LibraryApp.Domain
{
    public class Reservation
    {
        public int Id { get; }
        public LibraryItem Item { get; }
        public string UserEmail { get; }
        public DateTime From { get; }
        public DateTime To { get; }
        public bool IsActive { get; private set; } = true;

        public Reservation(int id, LibraryItem item, string userEmail, DateTime from, DateTime to)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            UserEmail = string.IsNullOrWhiteSpace(userEmail) ? throw new ArgumentException("Email required.", nameof(userEmail)) : userEmail;
            if (from >= to) throw new ArgumentException("From must be earlier than To.");
            Id = id; From = from; To = to;
        }

        public void Cancel() => IsActive = false;

        public bool ConflictsWith(DateTime from, DateTime to)
            => IsActive && From < to && from < To; 

        public override string ToString()
            => $"Reservation #{Id}: {Item.Title} for {UserEmail} ({From:yyyy-MM-dd} → {To:yyyy-MM-dd}) {(IsActive ? "[Active]" : "[Cancelled]")}";
    }
}