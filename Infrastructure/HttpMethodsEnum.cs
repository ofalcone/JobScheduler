using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public enum HttpMethodsEnum
    {
        GET,
        GET_BY_ID,
        DELETE,
        POST, 
        PUT,
        DELETE_CONFIRMED
    }
}
