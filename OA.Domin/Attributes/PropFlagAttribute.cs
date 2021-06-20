using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Domin.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class PropFlagAttribute : Attribute
    {
        public string Flag { get; set; } = "";

        public PropFlagAttribute(string flag)
        {
            this.Flag = flag;
        }

    }
}
