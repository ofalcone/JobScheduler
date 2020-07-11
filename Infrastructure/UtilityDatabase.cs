using JobScheduler.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class UtilityDatabase
    {
        internal static async Task<T> TryCommit<T>(JobSchedulerContext _context, T obj = default)
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                obj = default;
            }

            return obj;
        }


        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }
    }
}
