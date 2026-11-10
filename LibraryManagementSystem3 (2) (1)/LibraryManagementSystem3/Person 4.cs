using System;

namespace LibraryManagementSystem
{
    class Program
    {
        static BookManager bookManager = new BookManager();
        static MemberManager memberManager = new MemberManager();
        static BorrowManager borrowManager = new BorrowManager();

        static void Main(string[] args)
        {
            LoadSampleData();

            bool exit = false;

            while (!exit)
            {
                Console.Clear();

                Console.WriteLine("===== LIBRARY MANAGEMENT SYSTEM =====");
                Console.WriteLine("1. View Books");
                Console.WriteLine("2. View Members");
                Console.WriteLine("3. Borrow Book");
                Console.WriteLine("4. Return Book");
                Console.WriteLine("5. View Transactions");
                Console.WriteLine("0. Exit");

                Console.Write("\nEnter choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        bookManager.ViewAllBooks();
                        break;

                    case "2":
                        memberManager.ViewAllMembers();
                        break;

                    case "3":
                        BorrowBook();
                        break;

                    case "4":
                        ReturnBook();
                        break;

                    case "5":
                        borrowManager.ViewTransactions();
                        break;

                    case "0":
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid option");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void LoadSampleData()
        {
            bookManager.AddBook("The Hobbit", "J.R.R Tolkien");
            bookManager.AddBook("1984", "George Orwell");

            memberManager.AddMember(
                "Jason Miller",
                "0829877611",
                "jason@gmail.com",
                DateTime.Now);

            memberManager.AddMember(
                "Noe Raphela",
                "0627198231",
                "noe@gmail.com",
                DateTime.Now);
        }

        static void BorrowBook()
        {
            Console.Write("Member ID: ");
            int memberId = int.Parse(Console.ReadLine());

            Console.Write("Book ID: ");
            int bookId = int.Parse(Console.ReadLine());

            borrowManager.BorrowBook(memberId, bookId);
        }

        static void ReturnBook()
        {
            Console.Write("Member ID: ");
            int memberId = int.Parse(Console.ReadLine());

            Console.Write("Book ID: ");
            int bookId = int.Parse(Console.ReadLine());

            borrowManager.ReturnBook(memberId, bookId);
        }
    }
}