﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Models
{
    public interface IHasId
    {
        int Id { get; set; }
    }
}
