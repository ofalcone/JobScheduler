using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Models
{
    public class StopJob
    {
        public int JobId { get; set; }
        public int Pid { get; set; }
    }
}
