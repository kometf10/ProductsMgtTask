using OA.Domin.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Domin.Requests
{
    public class ExportRequest
    {
        public RequestParameters FilterOptions { get; set; }

        public string TypeName { get; set; }

        public string ExportAs { get; set; } = "Exel";

        public string FileName { get; set; } = "Report";
    }

}
