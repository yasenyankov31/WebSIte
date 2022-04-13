using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace WebApplication1.Models
{
    public class AppDbContext:DbContext
    {

        public DbSet<CultureEvent> CultureEvent { get; set; }
        public DbSet<SportEvent> SportEvent { get; set; }
        public DbSet<SelfTaugthEvent> SelfTaugthEvent { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<SearchForVolunteer> SearchForVolunteer { get; set; }
        public DbSet<Tourism> Tourism { get; set; }
        public DbSet<Volunteers> Volunteers { get; set; }
        public DbSet<Comment> Comment { get; set; }
    }
}