using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OA.Domin.DataFilter
{
    public class EntityFilter<T>
    {
        public static IEnumerable<T> FilteredData(IEnumerable<FilterParams> filterParams, IEnumerable<T> data, string gather = "AND")
        {

            IEnumerable<string> distinctColumns = filterParams.Where(x => !String.IsNullOrEmpty(x.ColumnName)).Select(x => x.ColumnName).Distinct();

            bool firstFilter = true;
            IEnumerable<T> filteredData = (distinctColumns.Count() > 0)? Enumerable.Empty<T>() : data;
            foreach (string colName in distinctColumns)
            {
                var filterColumn = typeof(T).GetProperty(colName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (filterColumn != null)
                {
                    IEnumerable<FilterParams> filterValues = filterParams.Where(x => x.ColumnName.Equals(colName)).Distinct();
                    
                    foreach (var val in filterValues)
                    {
                        if (firstFilter)
                        {
                            filteredData = FilterData(val.FilterOption, data, filterColumn, val.FilterValue);
                            firstFilter = false;
                        }
                        else
                        {
                            if(gather == "AND")
                            {
                                filteredData = FilterData(val.FilterOption, filteredData, filterColumn, val.FilterValue);
                            }
                            else
                            {
                                filteredData = filteredData.Concat(FilterData(val.FilterOption, data, filterColumn, val.FilterValue)).Distinct();
                            }
                        }
                    }
                }
            }
            return filteredData;
        }

        //[TODO]: OCP , IQueriable
        private static IEnumerable<T> FilterData(FilterOptions filterOption, IEnumerable<T> data, PropertyInfo filterColumn, string filterValue)
        {
            int outValue;
            decimal dOutValue;
            DateTime dateValue;
            switch (filterOption)
            {
                #region [StringDataType]  

                case FilterOptions.StartsWith:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString().ToLower().StartsWith(filterValue.ToString().ToLower())).ToList();
                    break;
                case FilterOptions.EndsWith:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString().ToLower().EndsWith(filterValue.ToString().ToLower())).ToList();
                    break;
                case FilterOptions.Contains:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString().ToLower().Contains(filterValue.ToString().ToLower())).ToList();
                    break;
                case FilterOptions.DoesNotContain:
                    data = data.Where(x => filterColumn.GetValue(x, null) == null ||
                                     (filterColumn.GetValue(x, null) != null && !filterColumn.GetValue(x, null).ToString().ToLower().Contains(filterValue.ToString().ToLower()))).ToList();
                    break;
                case FilterOptions.IsEmpty:
                    data = data.Where(x => filterColumn.GetValue(x, null) == null ||
                                     (filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString() == string.Empty)).ToList();
                    break;
                case FilterOptions.IsNotEmpty:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString() != string.Empty).ToList();
                    break;
                #endregion

                #region [Custom]  

                case FilterOptions.IsGreaterThan:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) > outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>) || filterColumn.PropertyType == typeof(DateTime) ) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) > dateValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Decimal) || filterColumn.PropertyType == typeof(Nullable<Decimal>)) && Decimal.TryParse(filterValue, out dOutValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) > dOutValue).ToList();
                    }
                    break;

                case FilterOptions.IsGreaterThanOrEqualTo:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) >= outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) >= dateValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Decimal) || filterColumn.PropertyType == typeof(Nullable<Decimal>)) && Decimal.TryParse(filterValue, out dOutValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) >= dOutValue).ToList();
                    }
                    break;

                case FilterOptions.IsLessThan:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) < outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) < dateValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Decimal) || filterColumn.PropertyType == typeof(Nullable<Decimal>)) && Decimal.TryParse(filterValue, out dOutValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) < dOutValue).ToList();
                    }
                    break;

                case FilterOptions.IsLessThanOrEqualTo:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) <= outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) <= dateValue).ToList();
                        break;
                    }
                    else if ((filterColumn.PropertyType == typeof(Decimal) || filterColumn.PropertyType == typeof(Nullable<Decimal>)) && Decimal.TryParse(filterValue, out dOutValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) <= dOutValue).ToList();
                    }
                    break;

                case FilterOptions.IsEqualTo:
                    if (filterValue == string.Empty)
                    {
                        data = data.Where(x => filterColumn.GetValue(x, null) == null
                                        || (filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString().ToLower() == string.Empty)).ToList();
                    }
                    else
                    {
                        if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                        {
                            data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) == outValue).ToList();
                        }
                        else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                        {
                            data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) == dateValue).ToList();
                        }
                        else if ((filterColumn.PropertyType == typeof(Decimal) || filterColumn.PropertyType == typeof(Nullable<Decimal>)) && Decimal.TryParse(filterValue, out dOutValue))
                        {
                            data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) == dOutValue).ToList();
                        }
                        else
                        {
                            data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString().ToLower() == filterValue.ToLower()).ToList();
                        }
                    }
                    break;

                case FilterOptions.IsNotEqualTo:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) != outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) != dateValue).ToList();
                        break;
                    }
                    else if ((filterColumn.PropertyType == typeof(Decimal) || filterColumn.PropertyType == typeof(Nullable<Decimal>)) && Decimal.TryParse(filterValue, out dOutValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) != dOutValue).ToList();
                    }
                    else
                    {
                        data = data.Where(x => filterColumn.GetValue(x, null) == null ||
                                         (filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null).ToString().ToLower() != filterValue.ToLower())).ToList();
                    }
                    break;
                    #endregion
            }
            return data;
        }
    }
}
