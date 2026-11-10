using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    public class Member
    {
        public int MemberID { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public DateTime DateRegistered { get; set; }

        public override string ToString()
        {
            return $"[{MemberID}] {Name} | Contact: {ContactNumber} | Email: {Email}";
        }

        public Member(int memberID, string name, string contactNumber, string email, DateTime dateRegistered)
        {
            MemberID = memberID;
            Name = name;
            ContactNumber = contactNumber;
            Email = email;
            DateRegistered = dateRegistered;
        }
    }

    public class MemberManager
    {
        private readonly List<Member> _members = new List<Member>();
        private int _nextId = 1;
        public IReadOnlyList<Member> Members => _members.AsReadOnly();

        //add members
        public Member AddMember(string name, string contactNumber, string email, DateTime dateRegistered)
        {
            if (!ValidateMemberData(name, contactNumber))
                return null;

            var member = new Member(_nextId, name.Trim(), contactNumber.Trim(), email, dateRegistered);
            _members.Add(member);
            _nextId++;

            Console.WriteLine($"Member \"{member.Name}\" has been added with ID {member.MemberID}.");
            return member;
        }

        //updating member details

        public bool UpdateMember(int memberId, string newName = null, string newContact = null, string newEmail = null)
        {
            Member member = SearchById(memberId);

            if (member == null)
            {
                Console.WriteLine($"No Member Found With ID {memberId}.");
                return false;
            }

            if (newName != null)

            {
                if (string.IsNullOrEmpty(newName))
                {
                    Console.WriteLine("Name can't be empty");
                    return false;
                }
                member.Name = newName.Trim();
            }

            if (newContact != null)
            {
                if (!IsValidPhoneNumber(newContact))
                {
                    Console.WriteLine("Invalid phone number");
                    return false;
                }
                member.ContactNumber = newContact.Trim();
            }

            if (newEmail != null)
            {
                member.Email = newEmail.Trim();
            }

            Console.WriteLine($"Member ID {memberId} has been updated");
            return true;
        }

        //removing members

        public bool RemoveMember(int memberId)
        {
            Member member = SearchById(memberId);

            if (member == null)
            {
                Console.WriteLine($" ERROR member {memberId} does not exist");
                return false;
            }

            _members.Remove(member);
            Console.WriteLine($"Mmber \"{member.Name}\" (ID {memberId}) has been successfully removed");
            return true;
        }

        //View Members

        public void ViewAllMembers()
        {
            if (_members.Count == 0)
            {
                Console.WriteLine("There are no members");
                return;
            }

            Console.WriteLine("\n --Registered Members--");

            foreach (var member in _members)
            {
                Console.WriteLine(member);
            }

            Console.WriteLine($"Total members {_members.Count}\n");
        }

        public Member SearchById(int memberId)
        {
            return _members.FirstOrDefault(m => m.MemberID == memberId);
        }

        //Search ID

        public Member SearchByIdWithMessage(int memberId)
        {
            Member member = SearchById(memberId);

            if (member == null)
            {
                Console.WriteLine($"Member {memberId} does not exist");
            }
            else
            {
                Console.WriteLine("Found" + member);
            }

            return member;
        }

        // Search Name

        public List<Member> SearchByName(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("Feild can not be empty");
                return new List<Member>();
            }

            var results = _members
                .Where(m => m.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (results.Count == 0)
            {
                Console.WriteLine($"0 Memebrs found with matching name\"{searchTerm}\".");
            }
            else
            {
                Console.WriteLine($"{results.Count} matching members \"{searchTerm}\" ");

                foreach (var member in results)
                {
                    Console.WriteLine(member);
                }
            }

            return results;
        }

        //Arrange by name

        public List<Member> ArrangeByName(bool descending = false)
        {
            return descending
                ? _members.OrderByDescending(m => m.Name).ToList()
                : _members.OrderBy(m => m.Name).ToList();
        }

        //Arragnge by ID

        public List<Member> SortById(bool descending = false)
        {
            return descending
                ? _members.OrderByDescending(m => m.MemberID).ToList()
                : _members.OrderBy(m => m.MemberID).ToList();
        }

        //Member Info Validation

        private bool ValidateMemberData(string name, string contactNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Feild can't be empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(contactNumber))
            {
                Console.WriteLine("Feilf can't be empty");
                return false;
            }

            if (!IsValidPhoneNumber(contactNumber))
            {
                Console.WriteLine("Invalid Format. Enter only 10 digits");
                return false;
            }

            return true;
        }

        private bool IsValidPhoneNumber(string phone)
        {
            string cleaned = Regex.Replace(phone, @"\D", "");
            return cleaned.Length >= 10;
        }

        public static void PrintMemberList(List<Member> members)
        {
            if (members == null || members.Count == 0)
            {
                Console.WriteLine("No Members");
                return;
            }

            foreach (var member in members)
            {
                Console.WriteLine(member);
            }
        }
    }
}