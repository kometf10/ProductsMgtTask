using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Domin.RequestFeatures
{
    public class PagingMetaData
    {
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public bool HasPrevios => CurrentPage > 1;

        public bool HasNext => CurrentPage < TotalPages;

    }
}
