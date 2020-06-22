using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Models
{
    public class NodeLaunchResult
    {
        public int NodeId { get; set; }
        public int LaunchResultId { get; set; }
        public Node Node { get; set; }
        public LaunchResult LaunchResult { get; set; }

        public int Pid { get; set; }
        public int ExitCode { get; set; }
        public int IdNode { get; set; }
        public string StandardOutput { get; set; }
        public int JobId { get; set; }
    }
}
