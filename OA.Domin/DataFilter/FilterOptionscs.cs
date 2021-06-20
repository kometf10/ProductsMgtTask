using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace OA.Domin.DataFilter
{
    public enum FilterOptions
    {
        [Description("Starts With")]
        StartsWith = 1,
        [Description("Ends With")]
        EndsWith,
        Contains,
        [Description("Does Not Contain")]
        DoesNotContain,
        [Description("IS Empty")]
        IsEmpty,
        [Description("IS Not Empty")]
        IsNotEmpty,
        [Description("IS Grater Than")]
        IsGreaterThan,
        [Description("IS Grater Than Or Equal To")]
        IsGreaterThanOrEqualTo,
        [Description("IS Less Than")]
        IsLessThan,
        [Description("IS Less Than Or Equal To")]
        IsLessThanOrEqualTo,
        [Description("Is Equal To")]
        IsEqualTo,
        [Description("Is Not Equal To")]
        IsNotEqualTo
    }

    public static class TypeOperators
    {
        public static Dictionary<FilterOptions, List<Type>> SupportList = new Dictionary<FilterOptions, List<Type>>()
        {
            { FilterOptions.StartsWith, new List<Type>(){ typeof(string) } },
            { FilterOptions.EndsWith, new List<Type>(){ typeof(string) } },
            { FilterOptions.Contains, new List<Type>(){ typeof(string) } },
            { FilterOptions.DoesNotContain, new List<Type>(){ typeof(string) } },
            { FilterOptions.IsEmpty, new List<Type>(){ typeof(string) } },
            { FilterOptions.IsNotEmpty, new List<Type>(){ typeof(string) } },
            { FilterOptions.IsGreaterThan, new List<Type>(){ typeof(int), typeof(DateTime), typeof(int?), typeof(DateTime?), typeof(decimal), typeof(decimal?) } },
            { FilterOptions.IsGreaterThanOrEqualTo, new List<Type>(){ typeof(int), typeof(DateTime), typeof(int?), typeof(DateTime?), typeof(decimal), typeof(decimal?) } },
            { FilterOptions.IsLessThan, new List<Type>(){ typeof(int), typeof(DateTime), typeof(int?), typeof(DateTime?), typeof(decimal), typeof(decimal?) } },
            { FilterOptions.IsLessThanOrEqualTo, new List<Type>(){ typeof(int), typeof(DateTime), typeof(int?), typeof(DateTime?), typeof(decimal), typeof(decimal?) } },
            { FilterOptions.IsEqualTo, new List<Type>(){ typeof(int), typeof(DateTime), typeof(string), typeof(int?), typeof(DateTime?), typeof(object), typeof(decimal), typeof(decimal?) } },
            { FilterOptions.IsNotEqualTo, new List<Type>(){ typeof(int), typeof(DateTime), typeof(string), typeof(int?), typeof(DateTime?), typeof(object), typeof(decimal), typeof(decimal?) } },

        };

        public static string GetDescription(Enum enumerationValue)
        {
            Type type = enumerationValue.GetType();

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute) attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        public static Dictionary<string, string> GetTypeOperators(Type type)
        {
            Dictionary<string, string> operators = new Dictionary<string, string>();
            foreach (var item in SupportList)
                if (item.Value.Contains(type))
                    operators.Add(item.Key.ToString(), GetDescription(item.Key));
            return operators;
        }
        
    }
}
