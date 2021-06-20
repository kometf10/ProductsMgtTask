using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OA.Domin
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? CreatedAt { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? LastModefiedAt { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(255)]
        public string CreatedBy { get; set; }

        [ScaffoldColumn(false)]
        [StringLength(255)]
        public string LastModifiedBy { get; set; }

        [ScaffoldColumn(false)]
        public bool IsDeleted { get; set; } = false;


    }
}
