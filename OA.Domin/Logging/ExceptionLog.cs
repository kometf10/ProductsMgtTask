using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OA.Domin.Logging
{
    public class ExceptionLog : BaseEntity
    {
        public string Path { get; set; }

        public string Message { get; set; }

        [Column(TypeName = "ntext")]
        public string StackTrace { get; set; }

    }
}
