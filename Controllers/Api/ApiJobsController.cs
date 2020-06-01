using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobScheduler.Data;
using JobScheduler.Models;
using System.Net.Http;
using System.Text;
using JobScheduler.Infrastructure;

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiJobsController : ControllerBase
    {
        private readonly JobSchedulerContext _context;

        public ApiJobsController(JobSchedulerContext context)
        {
            _context = context;
        }

        // GET: api/ApiJobs
        [HttpGet]
        public async Task<IList<Job>> GetJobs()
        {
            //return await _context.Jobs.ToListAsync();
            var res = await _context.Jobs.ToListAsync();
            return res;
        }

        // GET: api/ApiJobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            if (id == default)
            {
                return null;
            }

            var job = await _context.Jobs.FindAsync(id);

            if (job == null)
            {
                return null;
            }
            return job;
        }

        // PUT: api/ApiJobs/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJob(int id, Job job)
        {
            if (id != job.Id)
            {
                return BadRequest();
            }

            _context.Entry(job).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ApiJobs
        [HttpPost, Route("[action]")]
        public async Task<ActionResult<Job>> PostJob(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            //ScheduleJob schedule = new ScheduleJob(job.Orario, TimeZoneInfo.Local, job.Id, job.Path);
            ScheduleJob schedule = new ScheduleJob(job.Orario, TimeZoneInfo.Local);

            return CreatedAtAction("GetJob", new { id = job.Id }, job);
        }

        [HttpPost, Route("[action]")]
        public async Task<ActionResult<object>> LaunchJob(LaunchJob launchJob)
        {
            return await Launch(launchJob);
        }

        //TODO: Spostare in utilityclass
        public static async Task<ActionResult<object>> Launch(LaunchJob launchJob)
        {
            string slaveURl = string.Format("https://localhost:5004/api/JobExe");
            object test = new
            {
                Id = launchJob.Id,
                Path = launchJob.Path,
                Argument = "",
                IdNodeList = new List<int> { 1, 2, 3 }

            };
            try
            {
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    StringContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(test), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync($"{slaveURl}", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(apiResponse);
                        return result;
                    }
                }
            }
            catch
            {
            }
            return default;
        }

        // DELETE: api/ApiJobs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Job>> DeleteJob(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return job;
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
    }
}
