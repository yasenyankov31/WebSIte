
namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    
    public  class SearchForVolunteer
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public string Creator { get; set; }


    }
}
