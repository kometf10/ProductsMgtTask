using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Domin.Responces
{
    public class Response<T> where T : class
    {

        public T Result { get; set; }

        public bool HasErrors { get; set; } = false;

        public List<ValidationResult> ValidationErrors { get; set; } = new List<ValidationResult>();

        public void AddValidationError(string field, string error)
        {
            ValidationErrors.Add(new ValidationResult() { Field = field, Errors = new List<string> { error } });
        }

    }
}
