using Radzen;

namespace KowToMateAdmin.Services
{
    public interface IGridFilterService1
    {
        #region For Advance Filter
        void GetDateFilterQueryForGridHeaderForAdvance(ref string where, ref string and, FilterDescriptor filterItem, string actualFieldName);
        string PopulateDateFiltersForGridHeaderAdvance(Object FilterValue, FilterOperator filterOperator, string actualFieldName);
        #endregion

        void PopulateDateFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem);
        void PopulateNumberFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem);
        void PopulateStringFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem);
        void PopulateDateFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator);
        void PopulateNumberFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator);
        void PopulateNumberFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator, int? alternateValue);
        void PopulateStringFiltersForGridHeader(string actualFieldName, ref string where, ref string and, CompositeFilterDescriptor filterItem, string logicalOperator);
        string GetCommaSeperatedItems(IEnumerable<string> filterItems);
        void PopulateSetupFitlerValues(List<CompositeFilterDescriptor> listOfFilter);

        //void PopulateDateFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem);
        //void PopulateNumberFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem);
        //void PopulateStringFiltersForGridHeader(string actualFieldName, ref string where, ref string and, FilterDescriptor filterItem);
    }
}