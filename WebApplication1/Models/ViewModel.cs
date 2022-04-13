using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ViewModel
    {
        public CultureEvent CultureEvent { get; set; }
        public SportEvent SportEvent { get; set; }
        public SelfTaugthEvent SelfTaugthEvent { get; set; }
        public SearchForVolunteer SearchForVolunteer { get; set; }
        public Tourism Tourism { get; set; }
        public Volunteers Volunteers { get; set; }
        public IEnumerable<Volunteers> VolunteersList { get; set; }
        public IEnumerable<Comment> Comment { get; set; }
    }
}