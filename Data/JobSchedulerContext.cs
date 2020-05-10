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
        public DbSet<Node> Nodes { get; set; }
        public DbSet<JobNode> JobNodes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=JobDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //nome nel DB
            modelBuilder.Entity<Job>().ToTable("Job");
            modelBuilder.Entity<Node>().ToTable("Node");
            modelBuilder.Entity<JobNode>().ToTable("JobNode");

            
            //chiave primaria nm
            modelBuilder.Entity<JobNode>().HasKey(jobNode => new { jobNode.JobId, jobNode.NodeId });

            //collegamento job to NM
            modelBuilder.Entity<JobNode>().HasOne(jobNode => jobNode.Job).WithMany(jobNode => jobNode.JobNodes);

            //collegamento node to NM
            modelBuilder.Entity<JobNode>().HasOne(jobNode => jobNode.Node).WithMany(jobNode => jobNode.JobNodes);

        }
    }
}
