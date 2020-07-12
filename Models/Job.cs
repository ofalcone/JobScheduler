using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Models
{
    public class Job: IHasId
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Orario { get; set; }
        public string Description { get; set; }
        public string Argomenti { get; set; }
        public ICollection<JobGroup> JobGroupes { get; set; }

    }
}
