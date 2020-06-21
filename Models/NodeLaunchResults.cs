using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Models
{
    public class NodeLaunchResults
    {
        public int NodeId { get; set; }
        public int LaunchResultId { get; set; }
        public Node Node { get; set; }
        public LaunchResult LaunchResult { get; set; }
    }
}
