using OA.Domin.ProductsMgt.Catigories;
using OA.Domin.Responces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OA.Services.ProductsMgt
{
    public interface ICatigoriesService
    {

        Task<IEnumerable<Catigory>> GetAll();

        Task<Response<Catigory>> Get(int id);

        Task<Response<Catigory>> Create(Catigory catigory);

        Task<Response<Catigory>> Update(Catigory catigory);

        Task<Response<string>> Delete(int id);

    }
}
