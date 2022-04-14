
namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public  class SearchForVolunteer
    {
        public int Id { get; set; }
        [Required]
        public string EventName { get; set; }
        public string ImagePath { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime Date { get; set; }

        public string Creator { get; set; }


    }
}
