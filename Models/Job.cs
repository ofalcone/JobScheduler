using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Path { get; set; }

        public string Orario { get; set; }
    }
}
