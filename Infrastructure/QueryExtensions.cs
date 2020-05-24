using Microsoft.EntityFrameworkCore;
using JobScheduler.ViewModels;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public static class QueryExtensions
    {
        public static async Task<QueryResponse<TResource>> ToQueryResponse<TRequest, TResource>(this IQueryable<TResource> dbSet, TRequest request)
            where TRequest : Query
            where TResource : class
        {
            var itemsCount = await dbSet.CountAsync();

            var query = dbSet;

            if (request != null)
            {
                if (request.SortField != null)
                {
                    // ORDER BY {SortField} {SortOrder} -> SortOrder = asc | desc
                    query = query.OrderBy($"{request.SortField} {request.SortOrder ?? "asc"}");
                }

                query = Where(query, request);

                if (request.PageIndex > 0 && request.PageSize > 0)
                {
                    query = query.Page(request.PageIndex, request.PageSize);
                }
            }

            return new QueryResponse<TResource>
            {
                Data = await query.ToListAsync(),
                ItemsCount = itemsCount,
            };
        }

        static IQueryable<TResource> Where<TRequest, TResource>(this IQueryable<TResource> query, TRequest request)
            where TRequest : Query
            where TResource : class
        {
            var properties = typeof(TRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var prop in properties)
            {
                var name = prop.Name;
                var value = prop.GetValue(request);

                if (value == null) continue;

                switch (Type.GetTypeCode(prop.PropertyType))
                {
                    case TypeCode.String:
                        // Name.Contains(@0), @0 = "Bev"
                        query = query.Where($"{name}.Contains(@0)", value);
                        break;

                    default:
                        // Id = @0, @0 = 77
                        query = query.Where($"{name} = @0", value);
                        break;
                }
            }

            return query;
        }
    }
}
