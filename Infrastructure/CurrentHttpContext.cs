using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class CurrentHttpContext
    {
        private static IHttpContextAccessor currentHttpContextAccessor;

        public static HttpContext Current => currentHttpContextAccessor.HttpContext;

        public static string AppBaseUrl => $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";

        internal static void Configure(IHttpContextAccessor contextAccessor)
        {
            currentHttpContextAccessor = contextAccessor;
        }
    }
}
