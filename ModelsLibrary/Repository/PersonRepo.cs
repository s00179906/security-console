using Dapper;
using ModelsLibrary.Helpers;
using ModelsLibrary.Interfaces;
using ModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace ModelsLibrary.Repository
{
    public class PersonRepository : IPerson
    {
        private static string Cnn = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

        public void Register(Person person)
        {
            using (IDbConnection cnn = new SQLiteConnection(Cnn))
            {
                cnn.Execute("insert into People (Email, Password, FirstName, LastName, IsAdmin, Hash, Salt) values (@Email, @Password, @FirstName, @LastName, @IsAdmin, @Hash, @Salt)", person);
            }
        }

        public List<Person> GetPeople()
        {
            using (IDbConnection cnn = new SQLiteConnection(Cnn))
            {
                return cnn.Query<Person>("select * from People", new DynamicParameters()).ToList();
            }
        }

        public bool IsAdmin(string email)
        {
            using (IDbConnection cnn = new SQLiteConnection(Cnn))
            {
                Person person = cnn.QueryFirst<Person>("select * from People where email=@email", new { email });
                return person.IsAdmin == 0 ? false : true;
            }
        }

        public Person Login(string email, string password)
        {
            using (IDbConnection cnn = new SQLiteConnection(Cnn))
            {
                Person person = GetPerson(email);

                if (person == null)
                {
                    GUIHelper.Print("User not Found!", ConsoleColor.Red);
                    return null;
                }

                bool isPasswordMatched = Hasher.VerifyPassword(password, person.Hash, person.Salt);

                if (!isPasswordMatched)
                {
                    GUIHelper.Print("Incorrect Email or Password!", ConsoleColor.Red);
                    return null;
                }

                Console.Clear();
                GUIHelper.Print($"Welcome {person.FullName}", ConsoleColor.Green);
                return person;
            }
        }

        public void ApplyForAdmin(string email)
        {
            Person person = GetPerson(email);

            if (person != null)
            {
                AdminApplication existingApplication = GetApplication(person.Email);

                if (existingApplication == null)
                {
                    AdminApplication application = new AdminApplication()
                    {
                        PersonId = person.Id,
                        Email = person.Email
                    };

                    using (IDbConnection cnn = new SQLiteConnection(Cnn))
                    {
                        cnn.Execute("insert into AdminApplications (PersonId, Email) values (@PersonId, @Email)", application);
                        GUIHelper.Print("Application Sent", ConsoleColor.Green);
                    }
                }
            }

        }

        public Person GetPerson(string email)
        {
            using (IDbConnection cnn = new SQLiteConnection(Cnn))
            {
                return cnn.QueryFirstOrDefault<Person>("select * from People where email=@email", new { email });
            }
        }

        public List<AdminApplication> GetApplications()
        {
            using (IDbConnection cnn = new SQLiteConnection(Cnn))
            {
                return cnn.Query<AdminApplication>("select * from AdminApplications").ToList();
            }
        }

        public AdminApplication GetApplication(string email)
        {
            using (IDbConnection cnn = new SQLiteConnection(Cnn))
            {
                Person person = GetPerson(email);

                AdminApplication application = null;

                if (person != null)
                {
                    application = cnn.QueryFirstOrDefault<AdminApplication>("select * from AdminApplications where Email=@email", new { email });

                    if (application == null)
                    {
                        GUIHelper.Print("You have no applications pending!", ConsoleColor.Red);
                        return null;
                    }
                }

                GUIHelper.Print("You have 1 pending application!", ConsoleColor.Green);
                return application;
            }
        }

        public void AcceptAdminApplication()
        {
            bool isInt;
            int output;

            List<AdminApplication> applications = GetApplications();

            if (applications == null || applications.Count == 0)
            {
                GUIHelper.Print("No applications found!", ConsoleColor.Red);
            }
            else
            {
                GUIHelper.Print("Please pick from the following applications for approval", ConsoleColor.Cyan);

                applications.ForEach(application => Console.WriteLine($"{application.Id}: {application.Email}"));

                Console.Write("Choice: ");

                string input = Console.ReadLine();
                isInt = int.TryParse(input, out output);

                if (isInt)
                {
                    AdminApplication chosenApplication = applications.FirstOrDefault(a => a.Id == output);

                    Person person = GetPerson(chosenApplication.Email);

                    using (IDbConnection cnn = new SQLiteConnection(Cnn))
                    {
                        cnn.Query<AdminApplication>("update People set IsAdmin = @value where email = @email", new { value = true, person.Email });
                        cnn.Query<AdminApplication>("delete from AdminApplications where email = @email", new { person.Email });
                    }

                    GUIHelper.Print("Application Approved!", ConsoleColor.Green);
                }
                else
                {
                    GUIHelper.Print("Please enter a valid number!", ConsoleColor.Red);
                }

            }
        }
    }
}
