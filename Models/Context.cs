using Microsoft.EntityFrameworkCore;
namespace YellowTail.Models
{
    public class Context : DbContext
    {
        //context for passing through dbModels to database
        public Context(DbContextOptions options) : base(options){}
        //context dpoptions include a list of users, ideas, and likes
        public DbSet<User> users {get; set;}
        public DbSet<Activity> activities {get; set;}
        public DbSet<Participant> participants {get; set;}
        }
}