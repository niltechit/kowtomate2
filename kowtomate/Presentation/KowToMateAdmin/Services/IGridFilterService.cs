using Radzen;

namespace KowToMateAdmin.Services
{
    public interface IGridFilterService
    {
        string GetCommaSeperatedItems(IEnumerable<string> filterItems);
        void GetDateFilterQueryForGridHeaderForAdvance(ref string where, ref string and, FilterDescriptor filterItem, string actualFieldName);
        void PopulateDateFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator);
        void PopulateDateFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem);
        string PopulateDateFiltersForGridHeaderAdvance(object FilterValue, FilterOperator filterOperator, string actualFieldName);
        void PopulateNumberFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator);
        void PopulateNumberFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator, int? alternateValue);
        void PopulateNumberFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem);
        void PopulateStringFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator);
        void PopulateStringFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem);
    }
}