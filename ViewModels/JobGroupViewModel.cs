using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.ViewModels
{
    public class JobGroupViewModel
    {
        public int OldJobId { get; set; }
        public int OldGroupId { get; set; }

        public int NewJobId { get; set; }
        public int NewGroupId { get; set; }

    }
}
