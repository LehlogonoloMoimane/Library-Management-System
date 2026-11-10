using System;
using System.Collections.Generic;
using System.Linq;
    namespace LibraryManagementSystem
{
    // ============================================================
    // 1. BOOK CLASS
    // ============================================================
    /// <summary>
    /// Represents a single book in the library catalogue.
    /// A simple data model / entity — all mutation happens through
    /// properties so other modules (Member Management, Borrowing/Returning,
    /// Main Menu) can safely reference and display Book objects.
    /// </summary>
    public class Book
    {
        // Auto-assigned by BookManager — never set manually.
        public int BookID { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string ISBN { get; set; }

        // Tracks whether the book is currently available to borrow.
        // The Borrowing/Returning module should toggle this on checkout/return.
        public bool IsAvailable { get; set; }

        public DateTime DateAdded { get; set; }

        public Book(int bookId, string title, string author, string genre = "N/A", string isbn = "N/A")
        {
            BookID = bookId;
            Title = title;
            Author = author;
            Genre = genre;
            ISBN = isbn;
            IsAvailable = true; // New books are available by default.
            DateAdded = DateTime.Now;
        }

        /// <summary>
        /// Console-friendly summary of the book, used by View/Search/Sort output.
        /// </summary>
        public override string ToString()
        {
            string status = IsAvailable ? "Available" : "Borrowed";
            return $"[{BookID,4}] {Title,-30} | {Author,-20} | {Genre,-12} | ISBN: {ISBN,-15} | {status}";
        }
    }

    // ============================================================
    // 2. BOOKMANAGER CLASS
    // ============================================================
    /// <summary>
    /// Handles all book-related operations: adding, updating, removing,
    /// viewing, searching, and sorting. Owns the in-memory List<Book>
    /// and exposes a clean public API for other modules to call into.
    /// </summary>
    public class BookManager
    {
        private readonly List<Book> _books = new List<Book>();

        // Auto-increments so callers never have to supply an ID.
        private int _nextId = 1;

        /// <summary>
        /// Read-only view of the catalogue for other modules to consume
        /// without being able to mutate the list directly.
        /// </summary>
        public IReadOnlyList<Book> Books => _books.AsReadOnly();

        // --------------------------------------------------------
        // ADD BOOK
        // --------------------------------------------------------
        public Book AddBook(string title, string author, string genre = "N/A", string isbn = "N/A")
        {
            if (!ValidateTitleAndAuthor(title, author))
                return null;

            var book = new Book(_nextId, title.Trim(), author.Trim(), genre?.Trim(), isbn?.Trim());
            _books.Add(book);
            _nextId++;

            Console.WriteLine($"[SUCCESS] Book \"{book.Title}\" added with ID {book.BookID}.");
            return book;
        }

        // --------------------------------------------------------
        // UPDATE BOOK
        // --------------------------------------------------------
        public bool UpdateBook(int bookId, string newTitle = null, string newAuthor = null,
                                string newGenre = null, string newIsbn = null, bool? newAvailability = null)
        {
            Book book = SearchById(bookId);

            if (book == null)
            {
                Console.WriteLine($"[ERROR] Update failed. No book found with ID {bookId}.");
                return false;
            }

            if (newTitle != null)
            {
                if (string.IsNullOrWhiteSpace(newTitle))
                {
                    Console.WriteLine("[ERROR] Update failed. Title cannot be empty.");
                    return false;
                }

                book.Title = newTitle.Trim();
            }

            if (newAuthor != null)
            {
                if (string.IsNullOrWhiteSpace(newAuthor))
                {
                    Console.WriteLine("[ERROR] Update failed. Author cannot be empty.");
                    return false;
                }

                book.Author = newAuthor.Trim();
            }

            if (newGenre != null) book.Genre = newGenre.Trim();
            if (newIsbn != null) book.ISBN = newIsbn.Trim();
            if (newAvailability.HasValue) book.IsAvailable = newAvailability.Value;

            Console.WriteLine($"[SUCCESS] Book ID {bookId} updated.");
            return true;
        }

        // --------------------------------------------------------
        // REMOVE BOOK
        // --------------------------------------------------------
        public bool RemoveBook(int bookId)
        {
            Book book = SearchById(bookId);

            if (book == null)
            {
                Console.WriteLine($"[ERROR] Removal failed. No book found with ID {bookId}.");
                return false;
            }

            _books.Remove(book);

            Console.WriteLine($"[SUCCESS] Book \"{book.Title}\" (ID {bookId}) removed.");
            return true;
        }

        // --------------------------------------------------------
        // VIEW ALL BOOKS
        // --------------------------------------------------------
        public void ViewAllBooks()
        {
            if (_books.Count == 0)
            {
                Console.WriteLine("[INFO] No books currently in the catalogue.");
                return;
            }

            Console.WriteLine("\n--- Library Catalogue ---");

            foreach (var book in _books)
                Console.WriteLine(book);

            Console.WriteLine($"Total books: {_books.Count}\n");
        }

        // --------------------------------------------------------
        // SEARCH BY ID
        // --------------------------------------------------------
        public Book SearchById(int bookId)
        {
            return _books.FirstOrDefault(b => b.BookID == bookId);
        }

        public Book SearchByIdWithMessage(int bookId)
        {
            Book book = SearchById(bookId);

            if (book == null)
                Console.WriteLine($"[ERROR] No book found with ID {bookId}.");
            else
                Console.WriteLine("[FOUND] " + book);

            return book;
        }

        // --------------------------------------------------------
        // SEARCH BY TITLE
        // --------------------------------------------------------
        public List<Book> SearchByTitle(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("[ERROR] Search term cannot be empty.");
                return new List<Book>();
            }

            var results = _books
                .Where(b => b.Title.ToLower().Contains(searchTerm.ToLower()))
                .ToList();

            if (results.Count == 0)
                Console.WriteLine($"[INFO] No books found matching title \"{searchTerm}\".");
            else
            {
                Console.WriteLine($"[FOUND] {results.Count} book(s) matching \"{searchTerm}\":");

                foreach (var book in results)
                    Console.WriteLine(book);
            }

            return results;
        }

        // --------------------------------------------------------
        // SORT BY TITLE
        // --------------------------------------------------------
        public List<Book> SortByTitle(bool descending = false)
        {
            return descending
                ? _books.OrderByDescending(b => b.Title).ToList()
                : _books.OrderBy(b => b.Title).ToList();
        }

        // --------------------------------------------------------
        // SORT BY AVAILABILITY
        // --------------------------------------------------------
        public List<Book> SortByAvailability(bool availableFirst = true)
        {
            return availableFirst
                ? _books.OrderByDescending(b => b.IsAvailable).ThenBy(b => b.Title).ToList()
                : _books.OrderBy(b => b.IsAvailable).ThenBy(b => b.Title).ToList();
        }

        // --------------------------------------------------------
        // VALIDATION HELPERS
        // --------------------------------------------------------
        private bool ValidateTitleAndAuthor(string title, string author)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("[ERROR] Cannot add book. Title cannot be empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(author))
            {
                Console.WriteLine("[ERROR] Cannot add book. Author cannot be empty.");
                return false;
            }

            return true;
        }

        public static void PrintBookList(List<Book> books)
        {
            if (books == null || books.Count == 0)
            {
                Console.WriteLine("[INFO] No books to display.");
                return;
            }

            foreach (var book in books)
                Console.WriteLine(book);
        }
    }
}