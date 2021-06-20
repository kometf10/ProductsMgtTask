using Microsoft.EntityFrameworkCore;
using OA.DataAccess;
using OA.Domin.ProductsMgt.Catigories;
using OA.Domin.Responces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OA.Services.ProductsMgt
{
    public class CatigoriesService : ICatigoriesService
    {

        private readonly AppDbContext DbContext;
        public CatigoriesService(AppDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<IEnumerable<Catigory>> GetAll()
        {
            var catigories = await DbContext.Catigories.ToListAsync();

            return catigories;
        }

        public async Task<Response<Catigory>> Get(int id)
        {
            var result = new Response<Catigory>();
            var catigory = await DbContext.Catigories.FindAsync(id);

            if(catigory == null)
            {
                result.HasErrors = true;
                result.AddValidationError("Id", "Catigory Not Found");
                return result;
            }

            result.Result = catigory;
            return result;           
        }

        public async Task<Response<Catigory>> Create(Catigory catigory)
        {
            DbContext.Catigories.Add(catigory);
            _ = await DbContext.SaveChangesAsync();

            var result = new Response<Catigory>();
            result.Result = catigory;
            return result;
        }

        public async Task<Response<string>> Delete(int id)
        {
            var catigory = await DbContext.Catigories.FindAsync(id);

            var result = new Response<string>();

            if (catigory == null)
            {
                result.HasErrors = true;
                result.AddValidationError("Id", "Catigory Not Found");
                return result;
            }

            DbContext.Catigories.Remove(catigory);
            _ = await DbContext.SaveChangesAsync();

            result.Result = "Deleted";
            return result;
        }

        public async Task<Response<Catigory>> Update(Catigory catigory)
        {
            var existedCatigory = await DbContext.Catigories.FindAsync(catigory.Id);

            var result = new Response<Catigory>();

            if (existedCatigory == null)
            {
                result.HasErrors = true;
                result.AddValidationError("Id", "Catigory Not Found");
                return result;
            }

            existedCatigory.Name = catigory.Name;

            _ = await DbContext.SaveChangesAsync();

            result.Result = existedCatigory;
            return result;
        }
    }
}
