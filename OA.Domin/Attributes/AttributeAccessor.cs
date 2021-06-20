using OA.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace OA.Domin.Attributes
{
    public static class AttributeAccessor
    {

        public static List<string> GetFlagPropAttributes(PropertyInfo propInfo)
        {
            List<string> Flags = new List<string>();

            // Get instance of the attribute.
            var propFlags = propInfo.GetCustomAttributes(typeof(PropFlagAttribute));
            foreach(PropFlagAttribute propFlag in propFlags)            
                Flags.Add(propFlag.Flag);


            return Flags;

        }

        public static string GetDisplayNameAttr(PropertyInfo propInfo)
        {
            var attr = (DisplayNameAttribute)propInfo.GetCustomAttribute(typeof(DisplayNameAttribute));

            return (attr != null)? attr.DisplayName : propInfo.Name;

        }

        public static string GetEnumDiscription(this Enum val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static bool IsForeginKey(PropertyInfo propInfo)
        {
            var flags = GetFlagPropAttributes(propInfo);

            return flags.Contains("FK");
        }

        public static bool IsForeginKeyRef(PropertyInfo propInfo)
        {
            var flags = GetFlagPropAttributes(propInfo);

            return flags.Contains("FK_REF");
        }

        public static bool IsForeginKeyRefColl(PropertyInfo propInfo)
        {
            var flags = GetFlagPropAttributes(propInfo);

            return flags.Contains("FK_REF_COLL");
        }




        public static Dictionary<string, string> GetPropsWithNames(Type t, List<string> Listed = null)
        {
            var PropNames = new Dictionary<string, string>();

            var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach(var prop in props)
            {
                if (!IsForeginKey(prop) && !IsForeginKeyRefColl(prop))
                {
                    if (IsForeginKeyRef(prop))
                    {
                        if (!Listed.Contains(prop.Name))
                        {
                            Listed.Add(prop.Name);

                            var propRefProps = GetPropsWithNames(prop.PropertyType, Listed) ;
                            foreach (var propRefProp in propRefProps)
                                PropNames.Add(prop.Name + "." + propRefProp.Key, propRefProp.Value);
                        }
                    }
                    else
                        PropNames.Add(prop.Name, GetDisplayNameAttr(prop));
                }
                    
            }

            return PropNames;
        }

    }
}
