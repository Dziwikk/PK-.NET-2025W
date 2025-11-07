using System;

namespace LibraryApp.Domain
{
    public class Book : LibraryItem
    {
        public string Author { get; protected set; }
        public string Isbn { get; protected set; }

        public Book(int id, string title, string author, string isbn) : base(id, title)
        {
            Author = string.IsNullOrWhiteSpace(author) ? throw new ArgumentException("Author required.", nameof(author)) : author;
            Isbn  = string.IsNullOrWhiteSpace(isbn)    ? throw new ArgumentException("ISBN required.", nameof(isbn))     : isbn;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Book]  #{Id} \"{Title}\" — {Author} | ISBN: {Isbn} | {(IsAvailable ? "Available" : "Reserved")}");
        }
    }
}