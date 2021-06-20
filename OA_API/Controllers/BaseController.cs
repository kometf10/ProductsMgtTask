using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OA.DataAccess;
using OA.Domin;
using OA.Domin.DataFilter;
using OA.Domin.Paging;
using OA.Domin.Reflection;
using OA.Domin.RequestFeatures;
using OA.Domin.Requests;
using OA.Domin.Resources;
using OA_API.ActionFilters;
using OA_API.LocalServices.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Resources;
using System.Threading.Tasks;

namespace OA_API.Controllers
{
    public class BaseController<T> : ControllerBase where T : class, new()
    {
        protected readonly DbSet<T> DbSet;
        protected readonly AppDbContext Context;

        public BaseController(AppDbContext context)
        {          
            this.Context = context;
            this.DbSet = Context.Set<T>();
        }

        [HttpGet]
        public virtual async Task<IActionResult> Get([FromQuery] RequestParameters pagingParameters)
        {
            //throw new Exception("TEST EXCEPTION");

            List<T> list = await DbSet.ToListAsync();

            var listFilter = (pagingParameters.FilterParams != null) ? EntityFilter<T>.FilteredData(pagingParameters.FilterParams, list, pagingParameters.Gather) : list;

            listFilter = listFilter.OrderByDescending(i => i.GetType().GetProperty("CreatedAt").GetValue(i));

            var resultList = PagedList<T>.ToPagedList(listFilter, pagingParameters.PageNumber, pagingParameters.PageSize);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(resultList.PagingData));

            return Ok(resultList);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get(int id)
        {
            var entity = await DbSet.FindAsync(id);
            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(T entity)
        {
            DbSet.Add(entity);
            await Context.SaveChangesAsync();
            //Context.Dispose();

            return Created(nameof(T), entity);
        }

        [HttpPut]
        public virtual async Task<IActionResult> Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var entity = await DbSet.FindAsync(id);
            if (entity == null)
                return NotFound();

            DbSet.Remove(entity);
            await Context.SaveChangesAsync();
            return Ok();
        }


    }
}
