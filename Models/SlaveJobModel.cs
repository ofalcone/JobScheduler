using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Models
{
    public class SlaveJobModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Argomenti { get; set; }
        public int NodeId { get; set; }
    }
}
