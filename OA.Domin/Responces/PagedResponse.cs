using OA.Domin.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Domin.Responces
{
    public class PagedResponse<T>
    {

        public List<T> Items { get; set; }
        public PagingMetaData PagingData { get; set; }

    }
}
