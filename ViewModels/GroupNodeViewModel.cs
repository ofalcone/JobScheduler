using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.ViewModels
{
    public class GroupNodeViewModel
    {
        public int OldNodeId { get; set; }
        public int OldGroupId { get; set; }

        public int NewNodeId { get; set; }
        public int NewGroupId { get; set; }
    }
}
