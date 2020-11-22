using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsLibrary.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName{ get; set; }
        public string LastName { get; set; }
        public string Email  { get; set; }
        public string Password { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }       
        public int IsAdmin { get; set; }

        public string  FullName 
        {
            get { return $"{FirstName} {LastName}"; }
        }

        public override string ToString()
        {
            return String.Format($"{FullName} - {Email}");
        }
    }

}
