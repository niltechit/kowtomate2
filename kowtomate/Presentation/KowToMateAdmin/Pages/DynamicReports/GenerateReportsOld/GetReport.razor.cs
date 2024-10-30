using CutOutWiz.Core.Utilities;
using CutOutWiz.Data.Common;
using CutOutWiz.Data.DynamicReports;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Text.RegularExpressions;
using static CutOutWiz.Core.Utilities.Enums;
namespace KowToMateAdmin.Pages.DynamicReports.GenerateReports
{
    public partial class GetReport
    {
        #region FIlter
        RadzenDataFilter<IDictionary<string, object>> dataFilter;
        DynamicReportFilter dynamicReportFilter = new DynamicReportFilter();
        int count = 0;
        bool isLoading = false;
        IEnumerable<int> pageSizeOptions = new int[] { 10, 20, 50, 100, 200, 500, 1000 };
        string pagingSummaryFormat = "Displaying page {0} of {1} (total {2} records)";

        private bool isExporting = false;
        #endregion

        RadzenDataGrid<IDictionary<string, object>> grid;
        //IEnumerable<MainProduct> mainProducts = new List<MainProduct>();
        //IList<MainProduct> selectedProducts;

        IList<IDictionary<string, object>> selectedProducts;

        public IEnumerable<IDictionary<string, object>> mainProducts { get; set; }

        public IDictionary<string, Type> columns = new Dictionary<string, Type>();

        private List<ReportTableColumn> allTableColumns = new List<ReportTableColumn>();
        private List<ReportTableColumn> tableColumns = new List<ReportTableColumn>();

        public List<DynamicReportInfo> dynamicReportList { get; set; }
        private int? selectedDynamicReportInfoId = null;
        private DynamicReportInfo selectDynamicReportInfo = new DynamicReportInfo();
        //public IDictionary<string, Type> columns = new // { get; set; }

        public string GetColumnPropertyExpression(string name, Type type)
        {
            var expression = $@"it[""{name}""].ToString()";

            if (type == typeof(int?))
            {
                return $"int.Parse({expression})";
            }
            else if (type == typeof(DateTime?))
            {
                return $"DateTime.Parse({expression})";
            }
            else if (type == typeof(decimal?))
            {
                return $"decimal.Parse({expression})";
            }
            else if (type == typeof(bool?))
            {
                return $"bool.Parse({expression})";
            }

            return expression;
        }
       
        List<Company> companies;
        private bool isSearching = false;

        private async Task SetDirectoryColumns()
        {
            //Set dynamicReportList
            var allReports = await _dynamicReportInfoService.GetAll();
            dynamicReportList = allReports.Where(f => f.Status == StatusConstants.Active).ToList();
        }

        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            await SetDirectoryColumns();
            companies = await _companyService.GetAllClientCompany();
            //var filter = new MainProductFilter();
            //filter.Top = 10000;
            //data = await _mainProductService.GetProductsDirectoryByFilterWithPaging(filter);

            //Set defualt date
            dynamicReportFilter.StartDate = DateTime.Now.AddDays(-1);
            dynamicReportFilter.EndDate = DateTime.Now;
        }

        async Task OnSelectedReportChange(object value)
        {
            mainProducts = new List<IDictionary<string, object>>();
            columns = new Dictionary<string, Type>();
            allTableColumns = new List<ReportTableColumn>();
            tableColumns = new List<ReportTableColumn>();

            if (selectedDynamicReportInfoId == 0 || selectedDynamicReportInfoId == null)
            {
                StateHasChanged();
                return;
            }

            //grid.Reset(true, true);
            //StateHasChanged();

            //Get Report Id
            selectDynamicReportInfo = await _dynamicReportInfoService.GetById((int)selectedDynamicReportInfoId);

            //Set columns
            var dicColumns = new Dictionary<string, Type>();

            allTableColumns  = await _dynamicReportInfoService.GetTableColumnByDynamicReportInfoId((int)selectedDynamicReportInfoId);

            //tableColumns = tableColumns.Where(f => f.Id != 1 && f.Id != 4).ToList();
            var visibleColumns = allTableColumns.Where(f => f.IsVisible == true).ToList();
            foreach (var row in visibleColumns)
            {
                dicColumns.Add(row.FieldName, GetTypeByString(row.FieldTypeEnum));
            }

            tableColumns = visibleColumns;
            columns = dicColumns;
            StateHasChanged();

            // await grid.Reload();
            // await grid.FirstPage(true);
            await LoadData(null);
            StateHasChanged();
        }

        public async Task FindResults()
        {
            isSearching = true;
            // await grid.Reload();
            //await grid.FirstPage(true);
            await LoadData(null);
            isSearching = false;
        }


        async Task LoadData(LoadDataArgs args)
        {
           // await Task.Yield();
            //Open next time
            //selectedProducts = new List<MainProduct>();

            if (selectDynamicReportInfo == null || selectDynamicReportInfo.Id == 0)
            {
                //isFilterLoadingComplete = true;
                mainProducts = new List<IDictionary<string, object>>();
                isLoading = false;
                count = 0;
                return;
            }

            isLoading = true;

            if (args != null)
            {
                dynamicReportFilter.Top = args.Top ?? 20;
                dynamicReportFilter.Skip = args.Skip ?? 0;
                //Add sorting
            }

            dynamicReportFilter.Where = PrepareFilterQuery(args?.Filters);
            PopulateSortFilterQuery(args?.Sorts);
            //if (dynamicReportFilter.Skip == 0)
            //{
            //    dynamicReportFilter.IsCalculateTotal = true;
            //}
            //else
            //{
            //    dynamicReportFilter.IsCalculateTotal = false;
            //}

            //dynamicReportFilter.Where = PrepareFilterQuery(args.Filters, dataFilter.Filters);

            dynamicReportFilter.SqlQuery = selectDynamicReportInfo.SqlScript;

            mainProducts = await _dynamicReportInfoService.GetRecordsDirectoryByFilterWithoutPaging(dynamicReportFilter);

            // Important!!! Make sure the Count property of RadzenDataGrid is set.
            count = dynamicReportFilter.TotalCount;

            // Perform paginv via Skip and Take.
            isLoading = false;
        }

        private Type GetTypeByString(TableFieldType fieldTypeEnum)
        {
            if (fieldTypeEnum == TableFieldType.Boolean)
            {                
                return typeof(bool?);
            }
            else if (fieldTypeEnum == TableFieldType.Integer)
            {
                return typeof(int?);                 
            }
            else if (fieldTypeEnum == TableFieldType.Decimal)
            {
                return typeof(decimal?);
            }
            else if (fieldTypeEnum == TableFieldType.Date)
            {
                return typeof(DateTime?);
            }
            else
            {
                return typeof(string);
            }
            //else if (fieldTypeEnum == TableFieldType.ShortText
            //    || fieldTypeEnum == TableFieldType.Paragraph
            //     || fieldTypeEnum == TableFieldType.HtmlParagraph
            //    )
            //if (fieldTypeEnum == TableFieldType.Multiselect
            //    || fieldTypeEnum == TableFieldType.Dropdown)
            //{
            //    return "string";
            //}

        }

        #region Filter Section

        void PopulateSortFilterQuery(IEnumerable<SortDescriptor> sorts)
        {
            try
            {
                string sortClouse = string.Empty;

                //Set group filter
                if (sorts != null && sorts.Any())
                {
                    var sortColumn = sorts.FirstOrDefault();

                    dynamicReportFilter.SortColumn = sortColumn.Property;

                    if (sortColumn.SortOrder == SortOrder.Ascending)
                    {
                        dynamicReportFilter.SortDirection = "ASC";
                    }
                    else
                    {
                        dynamicReportFilter.SortDirection = "DESC";
                    }
                }
                else
                {
                    dynamicReportFilter.SortColumn = "";
                    dynamicReportFilter.SortDirection = "";
                }
            }
            catch (Exception ex)
            {
            }
        }

        string PrepareFilterQuery(IEnumerable<FilterDescriptor> filters) //, IEnumerable<CompositeFilterDescriptor> topFilters)
        {
            //IEnumerable<string> selectedNames;
            string where = string.Empty;
            string and = string.Empty;

            //Set group filter
            var items = string.Empty;
            #region grid header custom dropdown filters

            //Add client Company Filter

            if (selectDynamicReportInfo.AllowCompanyFilter)
            {
                if (dynamicReportFilter.ClientCompanyId > 0)
                {
                    var companyIdFilter = allTableColumns.FirstOrDefault(f => f.FieldName.ToLower().Contains("companyid"));

                    if (companyIdFilter != null)
                    {
                        //        //Add where close
                        where = $"{where}{and} {companyIdFilter.FieldWithPrefix} = {dynamicReportFilter.ClientCompanyId} ";
                        and = " AND ";
                    }
                }
            }

            //Add client Company Filter

            if (selectDynamicReportInfo.AllowStartDateFilter)
            {
                if (selectDynamicReportInfo.AllowEndDateFilter)
                {
                    if (dynamicReportFilter.StartDate != null && dynamicReportFilter.EndDate != null)
                    {
                        //Start and end date
                        var companyIdFilter = allTableColumns.FirstOrDefault(f => f.FieldName.ToLower().Contains("date"));

                        if (companyIdFilter != null)
                        {
                            //Add where close
                            if (selectDynamicReportInfo.AllowDateOnlyFilter)
                            {
                                where = $"{where}{and} (CONVERT(DATE, {companyIdFilter.FieldWithPrefix})  BETWEEN '{dynamicReportFilter.StartDate.Value.ToShortDateString()}' AND '{dynamicReportFilter.EndDate.Value.ToShortDateString()}') ";
                            }
                            else
                            {
                                where = $"{where}{and} ({companyIdFilter.FieldWithPrefix}  BETWEEN '{dynamicReportFilter.StartDate.Value}' AND '{dynamicReportFilter.EndDate.Value}') ";
                            }
                            and = " AND ";
                        }
                    }
                }
            }


            //if (selectedGroupNames != null && selectedGroupNames.Any())
            //{
            //    var notOrEmpty = selectedGroupNames.Any(f => f == " Blank");

            //    if (notOrEmpty)
            //    {
            //        //Add where close
            //        where = $"{where}{and} ([group] IS NULL OR [group] = '''') ";
            //        and = " AND ";

            //        //Add list
            //        selectedNames = selectedGroupNames.Where(f => f != " Blank").ToList();
            //    }
            //    else
            //    {
            //        selectedNames = selectedGroupNames;
            //    }

            //    if (selectedNames.Count() == 1)
            //    {
            //        where = $"{where}{and} [group] = ''{selectedNames.FirstOrDefault()}'' ";
            //        and = " AND ";
            //    }
            //    else if (selectedNames.Count() > 1)
            //    {
            //        items = _gridFilterService.GetCommaSeperatedItems(selectedNames);
            //        where = $"{where}{and} [group] in ({items})";
            //        and = " AND ";
            //    }
            //}

            ////Set brand filter
            //if (selectedBrandNames != null && selectedBrandNames.Any())
            //{
            //    var notOrEmpty = selectedBrandNames.Any(f => f == " Blank");

            //    if (notOrEmpty)
            //    {
            //        //Add where close
            //        where = $"{where}{and} ([manufacturer] IS NULL OR [manufacturer] = '''') ";
            //        and = " AND ";

            //        //Add list
            //        selectedNames = selectedBrandNames.Where(f => f != " Blank").ToList();
            //    }
            //    else
            //    {
            //        selectedNames = selectedBrandNames;
            //    }

            //    if (selectedNames.Count() == 1)
            //    {
            //        where = $"{where}{and} [manufacturer] = ''{selectedNames.FirstOrDefault()}'' ";
            //        and = " AND ";
            //    }
            //    else if (selectedNames.Count() > 1)
            //    {
            //        items = _gridFilterService.GetCommaSeperatedItems(selectedNames);
            //        where = $"{where}{and} [manufacturer] in ({items})";
            //        and = " AND ";
            //    }
            //}

            ////Set Product Type filter
            //if (selectedProductTypeNames != null && selectedProductTypeNames.Any())
            //{
            //    var notOrEmpty = selectedProductTypeNames.Any(f => f == " Blank");

            //    if (notOrEmpty)
            //    {
            //        //Add where close
            //        where = $"{where}{and} ([ProdType] IS NULL OR [ProdType] = '''') ";
            //        and = " AND ";

            //        //Add list
            //        selectedNames = selectedProductTypeNames.Where(f => f != " Blank").ToList();
            //    }
            //    else
            //    {
            //        selectedNames = selectedProductTypeNames;
            //    }

            //    if (selectedNames.Count() == 1)
            //    {
            //        where = $"{where}{and} [ProdType] = ''{selectedNames.FirstOrDefault()}'' ";
            //        and = " AND ";
            //    }
            //    else if (selectedNames.Count() > 1)
            //    {
            //        items = _gridFilterService.GetCommaSeperatedItems(selectedNames);
            //        where = $"{where}{and} [ProdType] in ({items})";
            //        and = " AND ";
            //    }
            //}

            ////Set ProductLine filter
            //if (selectedProductLineNames != null && selectedProductLineNames.Any())
            //{
            //    var notOrEmpty = selectedProductLineNames.Any(f => f == " Blank");

            //    if (notOrEmpty)
            //    {
            //        //Add where close
            //        where = $"{where}{and} ([ProductLine] IS NULL OR [ProductLine] = '''') ";
            //        and = " AND ";

            //        //Add list
            //        selectedNames = selectedProductLineNames.Where(f => f != " Blank").ToList();
            //    }
            //    else
            //    {
            //        selectedNames = selectedProductLineNames;
            //    }

            //    if (selectedNames.Count() == 1)
            //    {
            //        where = $"{where}{and} [ProductLine] = ''{selectedNames.FirstOrDefault()}'' ";
            //        and = " AND ";
            //    }
            //    else if (selectedNames.Count() > 1)
            //    {
            //        items = _gridFilterService.GetCommaSeperatedItems(selectedNames);
            //        where = $"{where}{and} [ProductLine] in ({items})";
            //        and = " AND ";
            //    }
            //}
            #endregion
            if (filters != null && filters.Any())
            {
                string actualFiledName = "";

                foreach (var filterItem in filters)
                {
                    //actualFiledName = GetActualFieldName(filterItem.Property);
                    string pattern = @"it\[\""([^\""]+)\""\]\.ToString\(\)";
                  
                    Match match = Regex.Match(filterItem.Property, pattern);
                    actualFiledName = match.Groups[1].Value;

                    var tableColumn = tableColumns.FirstOrDefault(f => f.FieldName == actualFiledName);
                    //var type = columns.Where(f => f.Key == actualFiledName).FirstOrDefault().Value;

                    actualFiledName = tableColumn.FieldWithPrefix;

                    if (tableColumn.FieldTypeEnum == TableFieldType.ShortText || tableColumn.FieldTypeEnum == TableFieldType.Paragraph || tableColumn.FieldTypeEnum == TableFieldType.HtmlParagraph)
                    {
                        _gridFilterService.PopulateStringFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem);
                    }
                    else if (tableColumn.FieldTypeEnum == TableFieldType.Integer || tableColumn.FieldTypeEnum == TableFieldType.Decimal)
                    {
                       _gridFilterService.PopulateNumberFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem);
                    }
                    else if (tableColumn.FieldTypeEnum == TableFieldType.Boolean)
                    {
                        if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
                        {
                            if (filterItem.FilterValue.ToString().ToLower() == "true")
                            {
                                where = $"{where}{and} {actualFiledName} = ''{filterItem.FilterValue.ToString()}'' ";
                            }
                            else
                            {
                                where = $"{where}{and} ({actualFiledName} = ''{filterItem.FilterValue.ToString()}'' OR {actualFiledName} IS NULL) ";
                            }
                        }

                        and = " AND ";
                    }
                    else if (tableColumn.FieldTypeEnum == TableFieldType.Date)
                    {
                        _gridFilterService.PopulateDateFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem);
                    }
                    //else
                    //{
                    //    _gridFilterService.PopulateStringFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem);
                    //}



                    //NOt in database need to add those
                    //Check boxes
                    //if (filterItem.Property == "HasImage"
                    //    || filterItem.Property == "availability"
                    //    || filterItem.Property == "ecom_avail"
                    //    || filterItem.Property == "newarrival"
                    //    || filterItem.Property == "hot"
                    //    || filterItem.Property == "Sale"
                    //    || filterItem.Property == "ListEbay"
                    //    || filterItem.Property == "ListWalmart"
                    //    || filterItem.Property == "discontinued"
                    //    || filterItem.Property == "OnChannelAdvisor"
                    //    || filterItem.Property == "IsFrbSynced"
                    //    || filterItem.Property == "IsLuxuryUSA"
                    //    || filterItem.Property == "IsLuxuryCA"

                    //)
                    //{
                    //    if (!string.IsNullOrEmpty(filterItem.FilterValue?.ToString()))
                    //    {
                    //        if (filterItem.FilterValue.ToString().ToLower() == "true")
                    //        {
                    //            where = $"{where}{and} {actualFiledName} = ''{filterItem.FilterValue.ToString()}'' ";
                    //        }
                    //        else
                    //        {
                    //            where = $"{where}{and} ({actualFiledName} = ''{filterItem.FilterValue.ToString()}'' OR {actualFiledName} IS NULL) ";
                    //        }
                    //    }

                    //    and = " AND ";
                    //}  //Date time fields
                    //else if (filterItem.Property == "retailupdate"
                    //    || filterItem.Property == "LastRcvdDate"
                    //    || filterItem.Property == "CreatedTime"
                    //    || filterItem.Property == "updatedate"
                    //)
                    //{
                    //    _gridFilterService.PopulateDateFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem);
                    //}
                    //else if (filterItem.Property == "lq"
                    //    || filterItem.Property == "ecom_price"
                    //    || filterItem.Property == "retail"
                    //    || filterItem.Property == "frbprice"
                    //    || filterItem.Property == "FrbFormulaPrice"
                    //    || filterItem.Property == "FrbQty"
                    //    || filterItem.Property == "QtyAvail"
                    //    || filterItem.Property == "EComQty"
                    //    || filterItem.Property == "weight"
                    //    || filterItem.Property == "LaunchYear"
                    //)
                    //{
                    //    _gridFilterService.PopulateNumberFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem);
                    //}
                    //else  // String fields
                    //{
                    //    _gridFilterService.PopulateStringFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem);
                    //}
                }
            }

            //For top filter
            //if (topFilters != null && topFilters.Any())
            //{
            //    var topLogicalFilterOperator = dataFilter.LogicalFilterOperator.ToString();
            //    var topWhere = PopulateTopFiltersQuery(topFilters, topLogicalFilterOperator);

            //    if (!string.IsNullOrWhiteSpace(topWhere))
            //    {
            //        where = $"{where}{and} ({topWhere})";
            //        and = " AND ";
            //    }
                
            //}

            //if (!string.IsNullOrWhiteSpace(where))
            //    where = " WHERE " + where;

            return where;
        }

        #endregion


        #region Common Methods
        private string ConvertToTitleCase(string input)
        {
            string output = "";

            for (int i = 0; i < input.Length; i++)
            {
                if (i > 0 && char.IsUpper(input[i]))
                {
                    output += " ";
                }

                output += input[i];
            }

            return output;
        }
        #endregion

        #region Export Excel
        public async Task DownloadExcelDocument()
        {
            await Task.Yield();
            isExporting = true;
            var fileName = $"{selectDynamicReportInfo.Name} {DateTime.Now.ToString("dd MM yyyy h mm tt")}.xlsx";

            //var exceptColumns = new List<string>();

            //exceptColumns.Add("GroupId");
            //exceptColumns.Add("EntryDateTime");

            var bytes = _dataImportService.GenerateExcel( mainProducts, tableColumns, "Report");

            await js.InvokeAsync<object>("saveAsFile", fileName, Convert.ToBase64String(bytes));

            isExporting = false;
        }
        #endregion

    }
}
    
