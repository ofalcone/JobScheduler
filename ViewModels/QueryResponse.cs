using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.ViewModels
{
    public class QueryResponse<TResource>
    {
        public IEnumerable<TResource> Data { get; set; }

        public int ItemsCount { get; set; }
    }
}
