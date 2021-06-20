using OA.Domin.DataFilter;
using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Domin.RequestFeatures
{
    public class RequestParameters
    {

        const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        private int _pagesize = 10;

        public int PageSize {
            get
            {
                return _pagesize;
            }
            set
            {
                _pagesize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }

        public IEnumerable<FilterParams> FilterParams { get; set; }

        public string Gather { get; set; }

    }
}
