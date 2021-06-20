using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OA.Domin.DataFilter
{
    public class FilterParams
    {
        [Required]
        public string ColumnName { get; set; } = string.Empty;

        [Required]
        public string FilterValue { get; set; } = string.Empty;

        [Required]
        public FilterOptions FilterOption { get; set; } = FilterOptions.IsEqualTo;

    }
}
