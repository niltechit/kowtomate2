using Radzen;

namespace KowToMateAdmin.Services
{
    public class GridFilterService : IGridFilterService
    {
        #region For Advance Filter
        public void GetDateFilterQueryForGridHeaderForAdvance(ref string where, ref string and, FilterDescriptor filterItem, string actualFieldName)
        {
            try
            {
                var firstQuery = PopulateDateFiltersForGridHeaderAdvance(filterItem.FilterValue, filterItem.FilterOperator, actualFieldName);

                if (!string.IsNullOrWhiteSpace(firstQuery))
                {
                    var secondQuery = PopulateDateFiltersForGridHeaderAdvance(filterItem.SecondFilterValue, filterItem.SecondFilterOperator, actualFieldName);

                    if (string.IsNullOrWhiteSpace(secondQuery))
                    {
                        where = $"{where}{and} {firstQuery}";
                        //Only first filter
                    }
                    else
                    {
                        where = $"{where}{and} ({firstQuery} {filterItem.LogicalFilterOperator.ToString()} {secondQuery})";
                        //First and second filter
                    }

                    and = " AND ";
                }
            }
            catch (Exception ex)
            {

            }
        }

        public string PopulateDateFiltersForGridHeaderAdvance(Object FilterValue, FilterOperator filterOperator, string actualFieldName)
        {
            if (!string.IsNullOrEmpty(FilterValue?.ToString()))
            {
                if (filterOperator == FilterOperator.Equals)
                {
                    return $"DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) = ''{((DateTime)FilterValue).ToShortDateString()}''";
                }
                else if (filterOperator == FilterOperator.NotEquals)
                {
                    return $"DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) <> ''{((DateTime)FilterValue).ToShortDateString()}''";
                }
                else if (filterOperator == FilterOperator.LessThan)
                {
                    return $"DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) < ''{((DateTime)FilterValue).ToShortDateString()}''";
                }
                else if (filterOperator == FilterOperator.LessThanOrEquals)
                {
                    return $"DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) <= ''{((DateTime)FilterValue).ToShortDateString()}''";
                }
                else if (filterOperator == FilterOperator.GreaterThan)
                {
                    return $"DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) > ''{((DateTime)FilterValue).ToShortDateString()}''";
                }
                else if (filterOperator == FilterOperator.GreaterThanOrEquals)
                {
                    return $"DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) >= ''{((DateTime)FilterValue).ToShortDateString()}''";
                }
            }
            else
            {
                if (filterOperator == FilterOperator.IsNull)
                {
                    return $"{actualFieldName} IS NULL";
                }
                else if (filterOperator == FilterOperator.IsNotNull)
                {
                    return $"{actualFieldName} IS NOT NULL";
                }
            }

            return "";
        }

        #endregion
        #region For Simple Search
        public void PopulateDateFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem)
        {
            if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
            {
                if (filterItem.FilterOperator == FilterOperator.Equals)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) = ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.NotEquals)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) <> ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThan)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) < ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) <= ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) > ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) >= ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = " AND ";
                }
            }
            else
            {
                if (filterItem.FilterOperator == FilterOperator.IsNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NULL";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.IsNotNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NOT NULL";
                    and = " AND ";
                }
            }
        }

        public void PopulateNumberFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem)
        {
            if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
            {
                if (filterItem.FilterOperator == FilterOperator.Equals)
                {
                    where = $"{where}{and} {actualFieldName} = {filterItem.FilterValue}";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.NotEquals)
                {
                    where = $"{where}{and} {actualFieldName} <> {filterItem.FilterValue}";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThan)
                {
                    where = $"{where}{and} {actualFieldName} < {filterItem.FilterValue}";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} <= {filterItem.FilterValue}";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
                {
                    where = $"{where}{and} {actualFieldName} > {filterItem.FilterValue}";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} >= {filterItem.FilterValue}";
                    and = " AND ";
                }
            }
            else
            {
                if (filterItem.FilterOperator == FilterOperator.IsNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NULL";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.IsNotNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NOT NULL";
                    and = " AND ";
                }
            }
        }

        public void PopulateStringFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem)
        {
            if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
            {
                var filterValue = filterItem.FilterValue.ToString().Replace("'", "''+ CHAR(39)+''");

                if (filterItem.FilterOperator == FilterOperator.Contains)
                {
                    where = $"{where}{and} {actualFieldName} like ''%{filterValue}%''";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.DoesNotContain)
                {
                    where = $"{where}{and} ({actualFieldName} IS NULL OR {actualFieldName} NOT like ''%{filterValue}%'')";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.StartsWith)
                {
                    where = $"{where}{and} {actualFieldName} like ''{filterValue}%''";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.EndsWith)
                {
                    where = $"{where}{and} {actualFieldName}  like ''%{filterValue}''";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.Equals)
                {
                    where = $"{where}{and} {actualFieldName} = ''{filterValue}''";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.NotEquals)
                {
                    where = $"{where}{and} {actualFieldName} <> ''{filterValue}''";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThan)
                {
                    where = $"{where}{and} {actualFieldName} < ''{filterValue}''";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} <= ''{filterValue}''";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
                {
                    where = $"{where}{and} {actualFieldName} > ''{filterValue}''";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} >= ''{filterValue}''";
                    and = " AND ";
                }
            }
            else
            {
                if (filterItem.FilterOperator == FilterOperator.IsEmpty || filterItem.FilterOperator == FilterOperator.IsNull)
                {
                    where = $"{where}{and} ({actualFieldName} = '''' OR {actualFieldName} IS NULL)";
                    and = " AND ";
                }
                else if (filterItem.FilterOperator == FilterOperator.IsNotEmpty || filterItem.FilterOperator == FilterOperator.IsNotNull)
                {
                    where = $"{where}{and} ({actualFieldName} <> '''' AND {actualFieldName} IS NOT NULL)";
                    and = " AND ";
                }
            }
        }

        public void PopulateDateFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator)
        {
            if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
            {
                if (filterItem.FilterOperator == FilterOperator.Equals)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) = ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.NotEquals)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) <> ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThan)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) < ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) <= ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) > ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
                {
                    where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) >= ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
                    and = $" {logicalOperator} ";
                }
            }
            else
            {
                if (filterItem.FilterOperator == FilterOperator.IsNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NULL";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.IsNotNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NOT NULL";
                    and = $" {logicalOperator} ";
                }
            }
        }

        public void PopulateNumberFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator)
        {
            if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
            {
                if (filterItem.FilterOperator == FilterOperator.Equals)
                {
                    where = $"{where}{and} {actualFieldName} = {filterItem.FilterValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.NotEquals)
                {
                    where = $"{where}{and} {actualFieldName} <> {filterItem.FilterValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThan)
                {
                    where = $"{where}{and} {actualFieldName} < {filterItem.FilterValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} <= {filterItem.FilterValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
                {
                    where = $"{where}{and} {actualFieldName} > {filterItem.FilterValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} >= {filterItem.FilterValue}";
                    and = $" {logicalOperator} ";
                }
            }
            else
            {
                if (filterItem.FilterOperator == FilterOperator.IsNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NULL";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.IsNotNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NOT NULL";
                    and = $" {logicalOperator} ";
                }
            }
        }

        public void PopulateNumberFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator, int? alternateValue)
        {
            if (alternateValue != null)
            {
                if (filterItem.FilterOperator == FilterOperator.Equals)
                {
                    where = $"{where}{and} {actualFieldName} = {alternateValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.NotEquals)
                {
                    where = $"{where}{and} {actualFieldName} <> {alternateValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThan)
                {
                    where = $"{where}{and} {actualFieldName} < {alternateValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} <= {alternateValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
                {
                    where = $"{where}{and} {actualFieldName} > {alternateValue}";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} >= {alternateValue}";
                    and = $" {logicalOperator} ";
                }
            }
            else
            {
                if (filterItem.FilterOperator == FilterOperator.IsNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NULL";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.IsNotNull)
                {
                    where = $"{where}{and} {actualFieldName} IS NOT NULL";
                    and = $" {logicalOperator} ";
                }
            }
        }

        public void PopulateStringFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator)
        {
            if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
            {
                var filterValue = filterItem.FilterValue.ToString().Replace("'", "''+ CHAR(39)+''");

                if (filterItem.FilterOperator == FilterOperator.Contains)
                {
                    where = $"{where}{and} {actualFieldName} like ''%{filterValue}%''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.In)
                {
                    var inItems = "";
                    var listOfFilterValue = filterItem.FilterValue.ToString().Split(',').ToList();

                    foreach (var item in listOfFilterValue)
                    {
                        inItems += $"''{item}'',";
                    }

                    inItems = inItems.TrimEnd(',');

                    where = $"{where}{and} {actualFieldName} IN ({inItems}) ";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.NotIn)
                {
                    var inItems = "";
                    var listOfFilterValue = filterItem.FilterValue.ToString().Split(',').ToList();

                    foreach (var item in listOfFilterValue)
                    {
                        inItems += $"''{item}'',";
                    }

                    inItems = inItems.TrimEnd(',');

                    where = $"{where}{and} {actualFieldName} NOT IN ({inItems}) ";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.DoesNotContain)
                {
                    where = $"{where}{and} ({actualFieldName} IS NULL OR {actualFieldName} NOT like ''%{filterValue}%'')";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.StartsWith)
                {
                    where = $"{where}{and} {actualFieldName} like ''{filterValue}%''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.EndsWith)
                {
                    where = $"{where}{and} {actualFieldName}  like ''%{filterValue}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.Equals)
                {
                    where = $"{where}{and} {actualFieldName} = ''{filterValue}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.NotEquals)
                {
                    where = $"{where}{and} {actualFieldName} <> ''{filterValue}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThan)
                {
                    where = $"{where}{and} {actualFieldName} < ''{filterValue}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} <= ''{filterValue}''";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
                {
                    where = $"{where}{and} {actualFieldName} > ''{filterValue}''";

                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
                {
                    where = $"{where}{and} {actualFieldName} >= ''{filterValue}''";
                    and = $" {logicalOperator} ";
                }
            }
            else
            {
                if (filterItem.FilterOperator == FilterOperator.IsEmpty || filterItem.FilterOperator == FilterOperator.IsNull)
                {
                    where = $"{where}{and} ({actualFieldName} = '''' OR {actualFieldName} IS NULL)";
                    and = $" {logicalOperator} ";
                }
                else if (filterItem.FilterOperator == FilterOperator.IsNotEmpty || filterItem.FilterOperator == FilterOperator.IsNotNull)
                {
                    where = $"{where}{and} ({actualFieldName} <> '''' AND {actualFieldName} IS NOT NULL)";
                    and = $" {logicalOperator} ";
                }
            }
        }

        public string GetCommaSeperatedItems(IEnumerable<string> filterItems)
        {
            var items = "";

            var seperator = "";
            var planItem = "";

            foreach (var item in filterItems)
            {
                planItem = item.Replace("'", " "); //TODO
                items = $"{items}{seperator}''{planItem}''";
                seperator = ",";
            }

            return items;
        }

        #endregion

        //public void PopulateDateFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem)
        //{
        //    if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
        //    {
        //        if (filterItem.FilterOperator == FilterOperator.Equals)
        //        {
        //            where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) = ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.NotEquals)
        //        {
        //            where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) <> ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.LessThan)
        //        {
        //            where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) < ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
        //        {
        //            where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) <= ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
        //        {
        //            where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) > ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
        //        {
        //            where = $"{where}{and} DATEADD(dd, DATEDIFF(dd, 0, {actualFieldName}), 0) >= ''{((DateTime)filterItem.FilterValue).ToShortDateString()}''";
        //            and = " AND ";
        //        }
        //    }
        //    else
        //    {
        //        if (filterItem.FilterOperator == FilterOperator.IsNull)
        //        {
        //            where = $"{where}{and} {actualFieldName} IS NULL";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.IsNotNull)
        //        {
        //            where = $"{where}{and} {actualFieldName} IS NOT NULL";
        //            and = " AND ";
        //        }
        //    }
        //}

        //public void PopulateNumberFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem)
        //{
        //    if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
        //    {
        //        if (filterItem.FilterOperator == FilterOperator.Equals)
        //        {
        //            where = $"{where}{and} {actualFieldName} = {filterItem.FilterValue}";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.NotEquals)
        //        {
        //            where = $"{where}{and} {actualFieldName} <> {filterItem.FilterValue}";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.LessThan)
        //        {
        //            where = $"{where}{and} {actualFieldName} < {filterItem.FilterValue}";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
        //        {
        //            where = $"{where}{and} {actualFieldName} <= {filterItem.FilterValue}";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
        //        {
        //            where = $"{where}{and} {actualFieldName} > {filterItem.FilterValue}";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
        //        {
        //            where = $"{where}{and} {actualFieldName} >= {filterItem.FilterValue}";
        //            and = " AND ";
        //        }
        //    }
        //    else
        //    {
        //        if (filterItem.FilterOperator == FilterOperator.IsNull)
        //        {
        //            where = $"{where}{and} {actualFieldName} IS NULL";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.IsNotNull)
        //        {
        //            where = $"{where}{and} {actualFieldName} IS NOT NULL";
        //            and = " AND ";
        //        }
        //    }
        //}

        //public void PopulateStringFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem)
        //{
        //    if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
        //    {
        //        var filterValue = filterItem.FilterValue.ToString().Replace("'", "''+ CHAR(39)+''");

        //        if (filterItem.FilterOperator == FilterOperator.Contains)
        //        {
        //            where = $"{where}{and} {actualFieldName} like ''%{filterValue}%''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.DoesNotContain)
        //        {
        //            where = $"{where}{and} {actualFieldName} NOT like ''%{filterValue}%''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.StartsWith)
        //        {
        //            where = $"{where}{and} {actualFieldName} like ''{filterValue}%''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.EndsWith)
        //        {
        //            where = $"{where}{and} {actualFieldName}  like ''%{filterValue}''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.Equals)
        //        {
        //            where = $"{where}{and} {actualFieldName} = ''{filterValue}''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.NotEquals)
        //        {
        //            where = $"{where}{and} {actualFieldName} <> ''{filterValue}''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.LessThan)
        //        {
        //            where = $"{where}{and} {actualFieldName} < ''{filterValue}''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.LessThanOrEquals)
        //        {
        //            where = $"{where}{and} {actualFieldName} <= ''{filterValue}''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.GreaterThan)
        //        {
        //            where = $"{where}{and} {actualFieldName} > ''{filterValue}''";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.GreaterThanOrEquals)
        //        {
        //            where = $"{where}{and} {actualFieldName} >= ''{filterValue}''";
        //            and = " AND ";
        //        }
        //    }
        //    else
        //    {
        //        if (filterItem.FilterOperator == FilterOperator.IsEmpty || filterItem.FilterOperator == FilterOperator.IsNull)
        //        {
        //            where = $"{where}{and} ({actualFieldName} = '''' OR {actualFieldName} IS NULL)";
        //            and = " AND ";
        //        }
        //        else if (filterItem.FilterOperator == FilterOperator.IsNotEmpty || filterItem.FilterOperator == FilterOperator.IsNotNull)
        //        {
        //            where = $"{where}{and} ({actualFieldName} <> '''' AND {actualFieldName} IS NOT NULL)";
        //            and = " AND ";
        //        }
        //    }
        //}
    }
}
