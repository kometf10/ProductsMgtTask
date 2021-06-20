using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Domin.Responces
{
    public class ValidationResult
    {
        public string Field { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

    }
}
