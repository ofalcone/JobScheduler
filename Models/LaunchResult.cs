using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Models
{
    public class LaunchResult
    {
        public int Id { get; set; }
        public ICollection<NodeLaunchResult> NodeLaunchResults { get; set; }
    }
}
