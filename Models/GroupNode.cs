using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Models
{
    public class GroupNode
    {
        public int GroupId { get; set; }
        public int NodeId { get; set; }
        public Group Group { get; set; }
        public Node Node { get; set; }
    }
}
