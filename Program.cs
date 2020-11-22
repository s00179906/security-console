using ModelsLibrary;
using ModelsLibrary.Helpers;
using ModelsLibrary.Models;
using ModelsLibrary.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SecurityConsole
{
    class Program
    {
        static PersonRepository Repo = new PersonRepository();
        static Person loggedInUser;
        static void Main(string[] args)
        {
            int output = 0;
            bool isInt = false;
            string input = string.Empty;
            int test = 3;

            HandleInput(isInt, output, input);

            do
            {
                input = string.Empty;
                if (loggedInUser != null)
                {
                    if (!Convert.ToBoolean(loggedInUser.IsAdmin))
                    {
                        Console.WriteLine($"1) Check Your Admin Status");
                        Console.WriteLine($"2) Check your Application Status");
                    }


                    if (Convert.ToBoolean(loggedInUser.IsAdmin))
                    {
                        Console.WriteLine($"1) Retrieve List of People");
                        Console.WriteLine($"2) Approve Pending Applications");
                    }

                    if(!Convert.ToBoolean(loggedInUser.IsAdmin)) Console.WriteLine($"{test}) Apply for Admin Role");
                    Console.WriteLine("0) Exit");

                    input = Console.ReadLine();
                    isInt = int.TryParse(input, out output);
                    Console.Write("Choice: ");

                    switch (output)
                    {
                        case 1:
                            if (!Convert.ToBoolean(loggedInUser.IsAdmin)) IsUserAdmin();
                            else GetPeople();
                            break;
                        case 2:
                            if (!Convert.ToBoolean(loggedInUser.IsAdmin)) GetApplication();
                            else AcceptAdminApplications();
                            break;
                        case 3:
                            ApplyForAdmin();
                            break;
                        default:
                            if (!isInt && output == 0) GUIHelper.Print("Please enter a valid number!", ConsoleColor.Red);
                            else Environment.Exit(1);
                            break;
                    }
                }
                else HandleInput(isInt, output, input);

            } while (output != 0 || isInt == false);

        }

        private static void Register()
        {
            Console.Write("\nFirst Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Password: ");
            string password = PasswordMask.HideCharacter();

            HashSalt hashSalt = Hasher.GenerateSaltedHash(password);

            Person person = new Person()
            {
                Email = email,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                IsAdmin = 0,
                Hash = hashSalt.Hash,
                Salt = hashSalt.Salt
            };

            Repo.Register(person);
            GUIHelper.Print("Registration Successful! Please login to continue", ConsoleColor.Green);
        }

        private static void Login()
        {

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Password: ");
            string password = PasswordMask.HideCharacter();

            loggedInUser = Repo.Login(email, password);
        }

        private static void GetPeople()
        {
            Console.Clear();

            List<Person> people = Repo.GetPeople();

            people.ForEach(person => Console.WriteLine(person));
        }

        public static void ApplyForAdmin()
        {
            Console.Clear();
            if (Convert.ToBoolean(loggedInUser.IsAdmin))
            {
                GUIHelper.Print("You are already an Admin!", ConsoleColor.Red);
                return;
            }

            Repo.ApplyForAdmin(loggedInUser.Email);
        }

        public static void GetApplication() 
        { 
            Console.Clear(); 
            Repo.GetApplication(loggedInUser.Email); 
        }

        public static void IsUserAdmin() 
        { 
            Console.Clear(); 
            GUIHelper.Print($"Admin: {Repo.IsAdmin(loggedInUser.Email)}", ConsoleColor.Green); 
        }

        public static void AcceptAdminApplications()
        {
            Console.Clear();
            if (Convert.ToBoolean(loggedInUser.IsAdmin)) Repo.AcceptAdminApplication();
            else GUIHelper.Print("Only for Admins!", ConsoleColor.Red);
        }

        public static void HandleInput(bool isInt, int output, string input)
        {
            GUIHelper.Print("Please (1) Login or (2) Register first: ", ConsoleColor.Cyan);

            input = Console.ReadLine();
            isInt = int.TryParse(input, out output);

            if (output == 1) Login();
            else if (output == 2) Register();
            else GUIHelper.Print("Please enter a valid number!", ConsoleColor.Red);
        }
    }
}
