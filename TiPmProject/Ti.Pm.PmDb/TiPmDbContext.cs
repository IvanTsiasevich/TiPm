using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Ti.Pm.PmDb.Model;


namespace Ti.Pm.PmDb
{
    public class TiPmDbContext : DbContext
    {
        public TiPmDbContext(DbContextOptions<TiPmDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<ApplicationError> ApplicationError { get; set; }
        public DbSet<ProjectPm> ProjectPm { get; set; }
        public DbSet<TaskTypePm> TaskTypePm { get; set; }
        public DbSet<StatusPm> StatusPm { get; set; }
        public DbSet<TaskPm> TaskPm { get; set; }
        public DbSet<User> User { get; set; }

        public User Login(string name, string pass)
        {
            var users = GetAllUsers();
            return users.SingleOrDefault(r => r.Login.ToLower() == name.ToLower() && r.Password == pass);
        }      
        public List<User> GetAllUsers()
        {
            var result = User.ToList();
            return result;
        }
    }
}
