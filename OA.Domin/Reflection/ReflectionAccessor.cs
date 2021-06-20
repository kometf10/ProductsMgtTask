using OA.Domain;
using OA.Domin.Attributes;
using OA.Domin.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OA.Domin.Reflection
{
    public class ReflectionAccessor
    {
        public static Dictionary<string, string> GetEntityTypes()
        {
            return new Dictionary<string, string> {

            };

            //typeof(BaseEntity).GetNestedTypes();
        }

        public static Dictionary<string, Type> EntityTypes = new Dictionary<string, Type>()
        {
           
            { nameof(ExceptionLog), typeof(ExceptionLog)}
        };

        public static Type GetType(string name, string nameSpace = "OA.Domin")
        {
            if (EntityTypes.Keys.Contains(name))
                return EntityTypes[name];

            return null;
            //var asemblies = AppDomain.CurrentDomain.GetAssemblies();
            //foreach (var assymbly in asemblies)
            //{
            //    var type = assymbly.GetType($"{nameSpace}.{name}");
            //    if (type != null)
            //        return type;
            //}
            //return null;
        }

        public static object GetPropertyValue(string typeName, string propName, object prop)
        {
            var getter = DynamicModuleLambdaCompiler.GetPropertyGetter(typeName, propName);

            return getter(prop);
        }

        public static class FastActivator<T> where T : new()
        {
            public static readonly Func<T> Create = DynamicModuleLambdaCompiler.GenerateFactory<T>();

            public static T SeedCreate(int i)
            {
                var t = Create();

                if (!(t is BaseEntity))
                    return t;

                var props = t.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (var prop in props)
                {
                    if (AttributeAccessor.IsForeginKey(prop) || AttributeAccessor.IsForeginKeyRef(prop) || AttributeAccessor.IsForeginKeyRefColl(prop) || !prop.CanWrite)
                        continue;
                    
                    var propType = prop.PropertyType;

                    if (propType == typeof(int) || propType == typeof(int?))
                        prop.SetValue(t, i);
                    else if (propType == typeof(DateTime) || propType == typeof(DateTime?))
                        prop.SetValue(t, DateTime.Now);
                    else if(propType == typeof(string))
                        prop.SetValue(t, $"Seed_{i}__O-o");
                }

                return t;
            }

        }


    }
}
