using ModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsLibrary.Interfaces
{
    public interface IPerson
    {
        bool IsAdmin(string email);
        Person Login(string email, string password);
        void Register(Person person);
        Person GetPerson(string email);
        List<AdminApplication> GetApplications();
        AdminApplication GetApplication(string email);
    }
}
