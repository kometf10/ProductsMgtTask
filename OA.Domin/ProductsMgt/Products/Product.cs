using OA.Domin.Attributes;
using OA.Domin.ProductsMgt.Catigories;
using OA.Domin.ProductsMgt.ProductCatigories;
using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Domin.ProductsMgt.Products
{
    public class Product : BaseEntity
    {

        public string Name { get; set; }

        public double Price { get; set; }

        [PropFlag("FK_REF_COLL")]
        public virtual ICollection<ProductCatigory> ProductCatigories { get; set; }
    }
}
