using System;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    // Stores information about each borrowing transaction
    public class BorrowRecord
    {
        public int MemberID { get; set; }
        public int BookID { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool Returned { get; set; }
    }

    // Handles all borrowing and returning operations
    public class BorrowManager
    {
        // List used to store all borrowing records
        private readonly List<BorrowRecord> records = new List<BorrowRecord>();

        // Read-only access to transaction records
        public IReadOnlyList<BorrowRecord> Records => records.AsReadOnly();

        // Borrow a book
        public void BorrowBook(int memberID, int bookID)
        {
            // Create a new borrowing record
            BorrowRecord record = new BorrowRecord
            {
                MemberID = memberID,
                BookID = bookID,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7), // Due after 7 days
                Returned = false
            };

            // Save the record
            records.Add(record);

            Console.WriteLine("Book Borrowed");
            Console.WriteLine("Borrow Date: " +
                record.BorrowDate.ToShortDateString());
            Console.WriteLine("Due Date: " +
                record.DueDate.ToShortDateString());
        }

        // Return a borrowed book
        public void ReturnBook(int memberID, int bookID)
        {
            // Search for the matching borrow record
            BorrowRecord record =
                records.Find(r =>
                    r.MemberID == memberID &&
                    r.BookID == bookID &&
                    !r.Returned);

            // Display error if no record exists
            if (record == null)
            {
                Console.WriteLine("No borrow record found");
                return;
            }

            // Update return details
            record.ReturnDate = DateTime.Now;
            record.Returned = true;

            // Calculate any fine owed
            double fine = CalculateFine(record);

            Console.WriteLine();
            Console.WriteLine("Book Returned");
            Console.WriteLine("Return Date: " +
                record.ReturnDate.ToShortDateString());
            Console.WriteLine("Fine: R" + fine);
        }

        // Calculates overdue fine
        public double CalculateFine(BorrowRecord record)
        {
            int daysLate = (record.ReturnDate - record.DueDate).Days;

            // R5 charged per overdue day
            if (daysLate > 0)
            {
                return daysLate * 5;
            }

            // No fine if returned on time
            return 0;
        }

        // Display all borrowing transactions
        public void ViewTransactions()
        {
            Console.WriteLine();
            Console.WriteLine("Transaction History");
            Console.WriteLine("-------------------");

            // Loop through each record in the list
            foreach (BorrowRecord record in records)
            {
                Console.WriteLine("Member ID: " + record.MemberID);
                Console.WriteLine("Book ID: " + record.BookID);
                Console.WriteLine("Borrow Date: " +
                    record.BorrowDate.ToShortDateString());
                Console.WriteLine("Due Date: " +
                    record.DueDate.ToShortDateString());

                // Only show return date if book was returned
                if (record.Returned)
                {
                    Console.WriteLine("Return Date: " +
                        record.ReturnDate.ToShortDateString());
                }

                Console.WriteLine();
            }
        }
    }
}