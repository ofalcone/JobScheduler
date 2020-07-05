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
        public DbSet<JobGroup> JobGroupes { get; set; }
        public DbSet<GroupNode> GroupNodes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<NodeLaunchResult> NodesLaunchResults { get; set; }
        public DbSet<LaunchResult> LaunchResult { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=JobDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>().ToTable("Job");
            modelBuilder.Entity<Node>().ToTable("Node");
            modelBuilder.Entity<JobGroup>().ToTable("JobGroup");
            modelBuilder.Entity<GroupNode>().ToTable("GroupNode");
            modelBuilder.Entity<Group>().ToTable("Group");
            modelBuilder.Entity<NodeLaunchResult>().ToTable("NodeLaunchResult");
            modelBuilder.Entity<LaunchResult>().ToTable("LaunchResult");

            modelBuilder.Entity<JobGroup>().HasKey(jobGroup => new { jobGroup.JobId, jobGroup.GroupId });
            modelBuilder.Entity<JobGroup>().HasOne(jobGroup => jobGroup.Job).WithMany(jobGroup => jobGroup.JobGroupes);
            modelBuilder.Entity<JobGroup>().HasOne(jobGroup => jobGroup.Group).WithMany(jobGroup => jobGroup.JobGroupes);

            modelBuilder.Entity<GroupNode>().HasKey(groupNode => new { groupNode.GroupId, groupNode.NodeId });
            modelBuilder.Entity<GroupNode>().HasOne(groupNode => groupNode.Node).WithMany(groupNode => groupNode.GroupNodes);
            modelBuilder.Entity<GroupNode>().HasOne(groupNode => groupNode.Group).WithMany(groupNode => groupNode.GroupNodes);

            modelBuilder.Entity<NodeLaunchResult>().HasKey(nodeLaunchResult => new { nodeLaunchResult.NodeId, nodeLaunchResult.LaunchResultId});
            modelBuilder.Entity<NodeLaunchResult>().HasOne(nodeLaunchResult => nodeLaunchResult.LaunchResult).WithMany(nodeLaunchResult => nodeLaunchResult.NodeLaunchResults);
            modelBuilder.Entity<NodeLaunchResult>().HasOne(nodeLaunchResult => nodeLaunchResult.Node).WithMany(nodeLaunchResult => nodeLaunchResult.NodeLaunchResults);

            base.OnModelCreating(modelBuilder);
        }

    }
}
