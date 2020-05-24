using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Models
{
    public class Role : IdentityRole
    {
        public int MyProperty { get; set; }
    }
}
