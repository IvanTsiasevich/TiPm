﻿using Microsoft.EntityFrameworkCore;
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
        public DbSet<Users> User { get; set; }
    }
}
