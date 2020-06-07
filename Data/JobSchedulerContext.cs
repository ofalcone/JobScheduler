using JobScheduler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Data
{
    public class JobSchedulerContext : IdentityDbContext<User>
    {
        public JobSchedulerContext(DbContextOptions<JobSchedulerContext> options)
           : base(options)
        { }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Node> Nodes { get; set; }
        //public DbSet<JobNode> JobNodes { get; set; }
        public DbSet<JobGroup> JobGroupes { get; set; }
        public DbSet<GroupNode> GroupNodes { get; set; }
        public DbSet<Group> Groups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=JobDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //nome nel DB
            modelBuilder.Entity<Job>().ToTable("Job");
            modelBuilder.Entity<Node>().ToTable("Node");
            //modelBuilder.Entity<JobNode>().ToTable("JobNode");
            modelBuilder.Entity<JobGroup>().ToTable("JobGroup");
            modelBuilder.Entity<GroupNode>().ToTable("GroupNode");
            modelBuilder.Entity<Group>().ToTable("Group");



            //chiave primaria nm
            //modelBuilder.Entity<JobNode>().HasKey(jobNode => new { jobNode.JobId, jobNode.NodeId });

            //collegamento job to NM
            //modelBuilder.Entity<JobNode>().HasOne(jobNode => jobNode.Job).WithMany(jobNode => jobNode.JobNodes);
            modelBuilder.Entity<JobGroup>().HasKey(jobGroup => new { jobGroup.JobId, jobGroup.GroupId });
            modelBuilder.Entity<JobGroup>().HasOne(jobGroup => jobGroup.Job).WithMany(jobGroup => jobGroup.JobGroupes);
            modelBuilder.Entity<JobGroup>().HasOne(jobGroup => jobGroup.Group).WithMany(jobGroup => jobGroup.JobGroupes);

            //modelBuilder.Entity<JobNode>().HasOne(jobNode => jobNode.Node).WithMany(jobNode => jobNode.JobNodes);

            modelBuilder.Entity<GroupNode>().HasKey(groupNode => new { groupNode.GroupId, groupNode.NodeId });
            modelBuilder.Entity<GroupNode>().HasOne(groupNode => groupNode.Node).WithMany(groupNode => groupNode.GroupNodes);
            modelBuilder.Entity<GroupNode>().HasOne(groupNode => groupNode.Group).WithMany(groupNode => groupNode.GroupNodes);

            //per poter aggiungere la migration Identity
            base.OnModelCreating(modelBuilder);
        }

    }
}
