using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.ViewModels
{
    public abstract class Query
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string SortField { get; set; }

        public string SortOrder { get; set; }
    }
}
