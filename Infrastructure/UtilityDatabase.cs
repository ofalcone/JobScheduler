using JobScheduler.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
