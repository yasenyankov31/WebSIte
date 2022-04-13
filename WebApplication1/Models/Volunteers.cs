using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Volunteers
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string  Username { get; set; }

        public string PhoneNumber { get; set; }
    }
}