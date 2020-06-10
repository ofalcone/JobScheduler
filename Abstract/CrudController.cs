using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Abstract
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class CrudController<TContext, TResource> : ControllerBase
        where TContext : DbContext
        where TResource : class, IHasId
    {

        private readonly GenericCrud<TContext, TResource> _genericCrud;

        public CrudController(TContext context)
        {
            _genericCrud = new GenericCrud<TContext, TResource>(context);
        }

        [HttpGet]
        public async Task<IList<TResource>> GetAll()
        {
            //return await Table.ToQueryResponse(query);
            return await _genericCrud.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<TResource> GetSingle(int id)
        {
            return await _genericCrud.GetSingle(id);
        }

        [HttpPut("{id}")]
        public async Task<bool> Update(int id, TResource resource)
        {
            return await _genericCrud.Update(id, resource);
        }

        [HttpPost]
        public async Task<bool> Create(TResource resource)
        {
            return await _genericCrud.Create(resource);
        }

        // DELETE: api/<Resource>/5
        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id)
        {
            return await _genericCrud.Delete(id);
        }

    }
}
