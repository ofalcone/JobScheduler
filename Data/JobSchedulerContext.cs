using JobScheduler.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Data
{
    public class JobSchedulerContext : DbContext
    {
        public JobSchedulerContext(DbContextOptions<JobSchedulerContext> options)
           : base(options)
        { }

        public DbSet<Job> Jobs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
            => optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=JobDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>().ToTable("Job");
        }
    }
}
