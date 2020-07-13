using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace JobScheduler.Data
{
    /// <summary>
    /// Crea gli utenti di default
    /// </summary>
    public class JobSchedulerDataSeed
    {
        private readonly JobSchedulerContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;


        public JobSchedulerDataSeed
            (
            JobSchedulerContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }


        public async Task SeedAsync()
        {
            var adminRole = await CreateRoleByName(Constants.ADMIN_ROLE);
            var editorRole = await CreateRoleByName(Constants.EDITOR_ROLE);
            var adminUser = await CreateUser(Constants.ADMIN_KEY);
            var editorUser = await CreateUser(Constants.EDITOR_KEY);

            if (adminUser != null)
            {
                await CreateUserRole(adminRole, adminUser);
            }

            if (editorUser != null)
            {
                await CreateUserRole(editorRole, editorUser);
            }

            var listJobs = await CreateTestJobs();
            var listGroups = await CreateTestGroups();
            var listNodes = await CreateTestNodes();

            await CreateTestJobGroup(listJobs, listGroups);

            await CreateTestGroupNode(listGroups, listNodes);
        }

        private async Task CreateUserRole(IdentityRole role, User user)
        {
            if (role == null || user == null) return;

            await _userManager.AddToRoleAsync(user, role.Name);

            await UtilityDatabase.TryCommit<Node>(_context);
        }

        private async Task<IdentityRole> CreateRoleByName(string tipoRuolo)
        {
            var foundRole = await _roleManager.FindByNameAsync(tipoRuolo);

            if (foundRole == null)
            {
                foundRole = new IdentityRole
                {
                    Name = tipoRuolo
                };

                var result = await _roleManager.CreateAsync(foundRole);

                if (result.Succeeded == false)
                {
                    return null;
                }

                foundRole = await _roleManager.FindByNameAsync(tipoRuolo);

                foundRole = await UtilityDatabase.TryCommit<IdentityRole>(_context, foundRole);
            }

            return foundRole;
        }

        /// <summary>
        /// Find user by name, if null create a new User
        /// </summary>
        /// <param name="userKey"></param>
        /// <returns>User if created, else null </returns>
        private async Task<User> CreateUser(string userKey)
        {
            string email = _configuration[$"{userKey}Info:Email"];
            string password = _configuration[$"{userKey}Info:Password"];
            string firstName = _configuration[$"{userKey}Info:FirstName"];
            string lastname = _configuration[$"{userKey}Info:LastName"];

            User user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastname,
                };

                var result = await _userManager.CreateAsync(user, password);
                //user = await _userManager.FindByEmailAsync(userName);

                if (result != null && result.Succeeded)
                {
                    user = await UtilityDatabase.TryCommit<User>(_context, user);
                }
                else
                {
                    user = null;
                }
            }
            else
            {
                user = null;
            }

            return user;
        }

        private async Task<List<Job>> CreateTestJobs()
        {
            List<Job> listGroups = null;

            var result = _context.Jobs.Count();
            if (result < 1)
            {
                string currentProjectPath = _configuration["SlaveUrls:LocalPath"];
                string executablePath = _configuration["ExecutableInfo:Path"];
                string executableLocation = Path.Combine(currentProjectPath, executablePath);
                string defaultCron = "";

                Job job1 = new Job { Orario = defaultCron, Path = executableLocation, Description = "test master", Argomenti = "test master" };
                Job job2 = new Job { Orario = defaultCron, Path = executableLocation, Description = "test job 1", Argomenti = "test job 1" };
                Job job3 = new Job { Orario = defaultCron, Path = executableLocation, Description = "test job 2", Argomenti = "test job 2" };

                _context.Jobs.AddRange(job1, job2, job3);

                await UtilityDatabase.TryCommit<Job>(_context);

                listGroups = new List<Job>
                {
                    job1,job2,job3
                };
            }

            return listGroups;
        }

        private async Task<List<Node>> CreateTestNodes()
        {
            List<Node> listNodes = null;

            var result = _context.Nodes.Count();
            if (result < 1)
            {
                Node node1 = new Node { Desc = "Master", Tipo = Enums.NodeType.Master, IndirizzoIP = _configuration["MasterUrl:BaseHttpsUrl"] };
                Node node2 = new Node { Desc = "Node1", IndirizzoIP = _configuration["SlaveUrls:BaseHttpsUrl"] };
                Node node3 = new Node { Desc = "Node2", IndirizzoIP = _configuration["SlaveUrls:BaseHttpsUrl"] };

                _context.Nodes.AddRange(node1, node2, node3);

                await UtilityDatabase.TryCommit<Node>(_context);

                listNodes = new List<Node>
                {
                    node1,
                    node2,
                    node3
                };
            }

            return listNodes;
        }

        private async Task<List<Group>> CreateTestGroups()
        {
            List<Group> listGroups = null;

            var result = _context.Groups.Count();
            if (result < 1)
            {
                Group group1 = new Group { Desc = "Group1" };
                Group group2 = new Group { Desc = "Group2" };

                _context.Groups.AddRange(group1, group2);

                await UtilityDatabase.TryCommit<Group>(_context);

                listGroups = new List<Group>
                {
                    group1,
                    group2
                };
            }

            return listGroups;
        }

        private async Task CreateTestJobGroup(List<Job> listJobs, List<Group> listGroups)
        {
            if (listJobs == null
                || listJobs.Count < 1
                || listGroups == null
                || listGroups.Count < 1)
            {
                return;
            }

            var result = _context.JobGroupes.Count();

            if (result < 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    var job = listJobs[i];
                    var group = listGroups[i];
                    var jobGroup = new JobGroup
                    {
                        Job = job,
                        JobId = job.Id,
                        Group = group,
                        GroupId = group.Id
                    };
                    _context.JobGroupes.Add(jobGroup);
                    await UtilityDatabase.TryCommit<JobGroup>(_context);
                }
            }
        }

        private async Task CreateTestGroupNode(List<Group> listGroups, List<Node> listNodes)
        {
            if (listNodes == null
                || listNodes.Count < 1
                || listGroups == null
                || listGroups.Count < 1)
            {
                return;
            }

            var result = _context.GroupNodes.Count();

            if (result < 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    var node = listNodes[i];
                    var group = listGroups[i];
                    var groupNode = new GroupNode
                    {
                        Group = group,
                        GroupId = group.Id,
                        Node = node,
                        NodeId = node.Id
                    };
                    _context.GroupNodes.Add(groupNode);
                    await UtilityDatabase.TryCommit<GroupNode>(_context);
                }
            }
        }
    }
}
