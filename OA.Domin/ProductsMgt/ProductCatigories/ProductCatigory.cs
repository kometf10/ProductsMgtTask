using OA.Domin.ProductsMgt.Catigories;
using OA.Domin.ProductsMgt.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Domin.ProductsMgt.ProductCatigories
{
    public class ProductCatigory : BaseEntity
    {

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int CatigoryId { get; set; }
        public virtual Catigory Catigory { get; set; }

    }
}
