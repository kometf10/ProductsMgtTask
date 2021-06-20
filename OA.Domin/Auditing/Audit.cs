using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OA.Domin.Auditing
{
    public class Audit
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime Date { get; set; }

        public string Operation { get; set; }

        public string TableName { get; set; }

        public string OldValues { get; set; }

        public string NewValues { get; set; }



    }
}
