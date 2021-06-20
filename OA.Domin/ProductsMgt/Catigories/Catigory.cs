using OA.Domin.Attributes;
using OA.Domin.ProductsMgt.ProductCatigories;
using OA.Domin.ProductsMgt.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OA.Domin.ProductsMgt.Catigories
{
    public class Catigory : BaseEntity
    {

        [DisplayName("Catigory Name")]
        public string Name { get; set; }


        [PropFlag("FK_REF_COLL")]
        public virtual ICollection<ProductCatigory> ProductCatigories { get; set; }

    }
}
