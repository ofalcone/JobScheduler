using JobScheduler.Data;
using JobScheduler.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class GenericCRUD<TContext, TResource> 
     where TContext : JobSchedulerContext
     where TResource : class, IHasId
     //where TQuery : Query
    {
        private readonly TContext _context;
        private DbSet<TResource> _table;

        public GenericCRUD(TContext context)
        {
            _context = context;
        }

        private DbSet<TResource> Table => _table ?? (_table = _context.Set<TResource>());

        public async Task<IList<TResource>> GetAll()
        {
            //return await Table.ToQueryResponse(query);
            return await Table.ToListAsync();
        }

        //public async Task<ActionResult<TResource>> GetSingle(int id)
        //{
        //    var resource = await Table.FindAsync(id);

        //    if (resource == null)
        //    {
        //        return NotFound();
        //    }

        //    return resource;
        //}

        //public async Task<IActionResult> Update(int id, TResource resource)
        //{
        //    if (id != resource.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(resource).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!Exists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //public async Task<ActionResult<TResource>> Create(TResource resource)
        //{
        //    Table.Add(resource);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetSingle", new { id = resource.Id }, resource);
        //}

        //public async Task<ActionResult<TResource>> Delete(int id)
        //{
        //    var resource = await Table.FindAsync(id);
        //    if (resource == null)
        //    {
        //        return NotFound();
        //    }

        //    Table.Remove(resource);
        //    await _context.SaveChangesAsync();

        //    return resource;
        //}

        private bool Exists(int id)
        {
            return Table.Any(e => e.Id == id);
        }
    }
}
