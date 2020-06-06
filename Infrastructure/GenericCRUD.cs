using JobScheduler.Data;
using JobScheduler.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class GenericCrud<TContext, TResource> 
     where TContext : DbContext
     where TResource : class, IHasId
     //where TQuery : Query
    {
        private readonly TContext _context;
        private DbSet<TResource> _table;

        public GenericCrud(TContext context)
        {
            _context = context;
        }

        private DbSet<TResource> Table => _table ?? (_table = _context.Set<TResource>());

        public async Task<IList<TResource>> GetAll()
        {
            //return await Table.ToQueryResponse(query);
            return await Table.ToListAsync();
        }

        public async Task<TResource> GetSingle(int id)
        {
            var resource = await Table.FindAsync(id);

            if (resource == null)
            {
                return null;
            }
            return resource;
        }

        public async Task<bool> Update(int id, TResource resource)
        {
            bool result = false;
            if (id != resource.Id)
            {
                return result;
            }

            _context.Entry(resource).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                result= true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    result = false;
                }
                else
                {
                    throw;
                }
            }

            return result;
        }

        public async Task<bool> Create(TResource resource)
        {
            Table.Add(resource);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var resource = await Table.FindAsync(id);
            if (resource == null)
            {
                return false;
            }

            Table.Remove(resource);
            var res = await _context.SaveChangesAsync();
            return res > 0;
        }

        private bool Exists(int id)
        {
            return Table.Any(e => e.Id == id);
        }
    }
}
