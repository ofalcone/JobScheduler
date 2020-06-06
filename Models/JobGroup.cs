using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Models
{
    public class JobGroup
    {
        public int JobId { get; set; }
        public int GroupId { get; set; }
        public Job Job { get; set; }
        public Group Group { get; set; }
    }
}
