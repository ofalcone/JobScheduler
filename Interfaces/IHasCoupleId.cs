using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Interfaces
{
    interface IHasCoupleId
    {
        int IdN { get; set; }
        int IdM { get; set; }
    }
}
