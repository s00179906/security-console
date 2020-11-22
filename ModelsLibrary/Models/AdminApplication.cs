using System;
using System.Collections.Generic;
using System.Text;

namespace ModelsLibrary.Models
{
    public class AdminApplication
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Email { get; set; }
    }
}
