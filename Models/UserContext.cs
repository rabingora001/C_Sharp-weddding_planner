using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }
        public DbSet<User> UsersTable {get;set;}

        public DbSet<WeddingSchedule> WeddingTable {get;set;}
        public DbSet<Guest> Guest {get;set;}

    }
}