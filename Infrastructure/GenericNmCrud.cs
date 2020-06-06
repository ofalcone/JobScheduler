using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class GenericNmCrud<TContext, TResource>
   where TContext : DbContext
   where TResource : class
        //where TQuery : Query
    {
        private readonly TContext _context;
        private DbSet<TResource> _table;
        public GenericNmCrud(TContext context)
        {
            _context = context;
        }

        private DbSet<TResource> Table => _table ?? (_table = _context.Set<TResource>());

        public async Task<IList<TResource>> GetAll()
        {
            return await Table.ToListAsync();
        }

        public async Task<TResource> GetSingle(int firstId, int secondId)
        {
            //var x = await Table.Tol(_firstId, _secondId);
            //return x;
            var resource = await Table.FindAsync(firstId, secondId);

            if (resource == null)
            {
                return null;
            }
            return resource;

            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var jobNode = await _context.JobGroupes
            //    .Include(j => j.Job)
            //    .Include(j => j.Group)
            //    .FirstOrDefaultAsync(m => m.JobId == id);
            //if (jobNode == null)
            //{
            //    return NotFound();
            //}

            //return View(jobNode);
        }

        //public async Task<bool> Update(int id, TResource resource)
        //{
        //    bool result = false;
        //    if (id != resource.Id)
        //    {
        //        return result;
        //    }

        //    _context.Entry(resource).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //        result = true;
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!Exists(id))
        //        {
        //            result = false;
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return result;
        //}

        //public async Task<bool> Create(TResource resource)
        //{
        //    Table.Add(resource);
        //    var result = await _context.SaveChangesAsync();
        //    return result > 0;
        //}

        //public async Task<bool> Delete(int id)
        //{
        //    var resource = await Table.FindAsync(id);
        //    if (resource == null)
        //    {
        //        return false;
        //    }

        //    Table.Remove(resource);
        //    var res = await _context.SaveChangesAsync();
        //    return res > 0;
        //}

        //private bool Exists(int id)
        //{
        //    return Table.Any(e => e.Id == id);
        //}
    }
}
