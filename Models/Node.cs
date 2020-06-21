using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Models
{
    public class Node: IHasId
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Desc { get; set; }

        public ICollection<GroupNode> GroupNodes { get; set; }
    }
}
