﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiRolesController : ControllerBase
    {
        private RoleManager<IdentityRole> _roleManager;
        private JobSchedulerContext _context;

        public ApiRolesController(RoleManager<IdentityRole> roleManager, JobSchedulerContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        // GET: api/<RolesController>
        [HttpGet]
        public async Task<List<IdentityRole>> Get()
        {
            var x = _roleManager.Roles.ToList();
            return x;
        }
        

        // GET api/<ApiRolesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ApiRolesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ApiRolesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ApiRolesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}