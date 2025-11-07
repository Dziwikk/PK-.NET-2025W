using System;

namespace LibraryApp.Domain
{
    public class EBook : Book
    {
        public string FileFormat { get; protected set; } // PDF/EPUB/MOBI

        public EBook(int id, string title, string author, string isbn, string fileFormat)
            : base(id, title, author, isbn)
        {
            FileFormat = string.IsNullOrWhiteSpace(fileFormat) ? throw new ArgumentException("File format required.", nameof(fileFormat)) : fileFormat;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[EBook] #{Id} \"{Title}\" — {Author} | ISBN: {Isbn} | Format: {FileFormat} | {(IsAvailable ? "Available" : "Reserved")}");
        }
    }
}