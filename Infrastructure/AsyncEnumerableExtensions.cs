using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public static class AsyncEnumerableExtensions
    {
        public static async Task<IList<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
        {
            var result = new List<T>();
            await foreach (var e in asyncEnumerable)
            {
                result.Add(e);
            }

            return result;
        }
    }
}
