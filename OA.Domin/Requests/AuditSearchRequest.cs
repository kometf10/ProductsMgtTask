using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OA.Domin.Requests
{
    public class AuditSearchRequest
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Required field")]
        public string TableName { get; set; }

        public string Operation { get; set; }

        public DateTimeOffset? FromDate { get; set; } = DateTime.Now.AddDays(-1);

        public DateTimeOffset? ToDate { get; set; } = DateTime.Now.AddDays(1);


    }
}
