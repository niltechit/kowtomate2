using CutOutWiz.Services.Models.DynamicReports;
using CutOutWiz.Services.Models.UI;
using CutOutWiz.Services.DynamicReports;
using CutOutWiz.Services.UI;
using KowToMateAdmin.Models.DynamicFilter;
using Radzen;
using Radzen.Blazor;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static CutOutWiz.Core.Utilities.Enums;
using SortOrder = Radzen.SortOrder;

namespace KowToMateAdmin.Services
{
    public class DynamicReportPageViewFilterService : IDynamicReportPageViewFilterService
    {
        public readonly IGridViewSetupService _gridViewSetupService;
        public readonly IGridFilterService _gridFilterService;
        public readonly IDynamicReportInfoService _dynamicReportInfoService;

        public DynamicReportPageViewFilterService(IGridViewSetupService gridViewSetupService,
            IGridFilterService gridFilterService,
            IDynamicReportInfoService dynamicReportInfoService)
        {
            _gridViewSetupService = gridViewSetupService;
            _gridFilterService = gridFilterService;
            _dynamicReportInfoService = dynamicReportInfoService;
        }

        /// <summary>
        /// Populate filter values
        /// </summary>
        /// <param name="listOfFilter"></param>
        /// <param name="allTableColumns"></param>
        public void PopulateSetupFitlerValues(List<CompositeFilterDescriptor> listOfFilter, List<ReportTableColumnModel> allTableColumns)
        {
            bool isSuccess = false;

            foreach (var filter in listOfFilter)
            {
                if (filter.Filters != null && filter.Filters.Any())
                {
                    PopulateSetupFitlerValues(filter.Filters.ToList(), allTableColumns);
                    continue;
                }

                string pattern = @"it\[\""([^\""]+)\""\]\.ToString\(\)";

                Match match = Regex.Match(filter.Property, pattern);
                var actualFiledName = match.Groups[1].Value;

                //For dynamic query builder
                var column = allTableColumns.FirstOrDefault(f => f.FieldName == actualFiledName);

                switch (column.FieldType)
                {
                    // bool
                    case (int)TableFieldType.Boolean:
                        bool output;
                        isSuccess = Boolean.TryParse(filter.FilterValue.ToString(), out output);
                        if (isSuccess)
                        {
                            filter.FilterValue = output;
                        }
                        else
                        {
                            filter.FilterValue = (bool?)null;
                        }
                        break;
                    //datetime
                    case (int)TableFieldType.Date:

                        DateTime date;
                        isSuccess = DateTime.TryParse(filter.FilterValue.ToString(), out date);
                        if (isSuccess)
                        {
                            filter.FilterValue = date;
                        }
                        else
                        {
                            filter.FilterValue = (DateTime?)null;
                        }
                        break;
                    //Decimal                     
                    case (int)TableFieldType.Decimal:
                        decimal dec;

                        isSuccess = decimal.TryParse(filter.FilterValue?.ToString(), out dec);

                        if (isSuccess)
                        {
                            filter.FilterValue = dec;
                        }
                        else
                        {
                            filter.FilterValue = (decimal?)null;
                        }
                        break;
                    //case "Number":
                    case (int)TableFieldType.Integer:
                        int intValue;
                        isSuccess = int.TryParse(filter.FilterValue?.ToString(), out intValue);

                        if (isSuccess)
                        {
                            filter.FilterValue = intValue;
                        }
                        else
                        {
                            filter.FilterValue = (int?)null;
                        }
                        break;

                    //    short
                    case (int)TableFieldType.Short:
                        short shortValue;

                        isSuccess = short.TryParse(filter.FilterValue?.ToString(), out shortValue);

                        if (isSuccess)
                        {
                            filter.FilterValue = shortValue;
                        }
                        else
                        {
                            filter.FilterValue = (short?)null;
                        }
                        break;

                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// TODO: need to update this
        /// </summary>
        /// <returns></returns>
        public async Task<List<GridViewSetupModel>> LoadGridViewTemplates(int? selectedDynamicReportInfoId, DynamicReportFilter dynamicReportFilter,
            DynamicReportPagePropertyVM dynamicReportPagePropertyVM, int loginUserContactId
            )
        {
            //TODO: update it
            var userTemplates = await _gridViewSetupService.GetListByDynamicReportIdContactId((int)selectedDynamicReportInfoId, loginUserContactId);

            if (userTemplates == null)
            {
                userTemplates = new List<GridViewSetupModel>();
            }

            var defaultSystemView = new GridViewSetupModel { Id = -1, DynamicReportInfoId = selectedDynamicReportInfoId, GridViewFor = (byte)GridViewFor.Others, DisplayName = "Default System View", Name = "Default System View" };

            userTemplates.Insert(0, defaultSystemView);

            if (dynamicReportPagePropertyVM.selectedGridViewSetupId > 0)
            {
                var defaultUserView = userTemplates.FirstOrDefault(f => f.Id == dynamicReportPagePropertyVM.selectedGridViewSetupId);

                if (defaultUserView == null)
                {
                    defaultUserView = defaultSystemView;
                }

                dynamicReportPagePropertyVM.selectedGridViewSetup = defaultUserView;
                dynamicReportPagePropertyVM.selectedGridViewSetupId = defaultUserView.Id;
            }
            else
            {
                var defaultUserView = userTemplates.FirstOrDefault(f => f.IsDefault);

                if (defaultUserView == null)
                {
                    defaultUserView = defaultSystemView;
                }

                dynamicReportPagePropertyVM.selectedGridViewSetup = defaultUserView;
                dynamicReportPagePropertyVM.selectedGridViewSetupId = defaultUserView.Id;
            }

            List<GridViewSetupModel> gridViewSetups = userTemplates;
            //Load All table columns
            //tableColumns = _mainProductService.GetAllTableColumns();


            var templateTableCoumns = new List<GridViewSetupColumnSlim>();

            if (dynamicReportPagePropertyVM.selectedGridViewSetup.Id == -1)
            {
                dynamicReportPagePropertyVM.selectedTableColumnIds = dynamicReportPagePropertyVM.tableColumns.Where(f => f.IsVisible == true).Select(f => f.Id).ToList();

                //currentSelectedTableColumnIds = selectedTableColumnIds.ToList();

                templateTableCoumns = dynamicReportPagePropertyVM.tableColumns.Select(f => new GridViewSetupColumnSlim { ColoumnId = f.Id, Width = Convert.ToDouble(f.Width), DisplayOrder = (int)f.DisplayOrder }).ToList();
            }
            else
            {
                templateTableCoumns = await _gridViewSetupService.GetColumnsByGridViewSetupId(dynamicReportPagePropertyVM.selectedGridViewSetup.Id);

                if (templateTableCoumns != null)
                {
                    dynamicReportPagePropertyVM.selectedTableColumnIds = templateTableCoumns.Select(f => f.ColoumnId).ToList();
                    //currentSelectedTableColumnIds = selectedTableColumnIds.ToList();

                    //Populate Table Columns
                    foreach (var col in templateTableCoumns)
                    {
                        var tabCol = dynamicReportPagePropertyVM.tableColumns.FirstOrDefault(f => f.Id == col.ColoumnId);
                        tabCol.DisplayOrder = col.DisplayOrder;
                        tabCol.Width = col.Width.ToString();
                    }

                    //var addedNotYetAddedList
                    var notInTemplateTableCoumns = dynamicReportPagePropertyVM.tableColumns.Where(f => !templateTableCoumns.Any(i => i.ColoumnId == f.Id))
                        .Select(f => new GridViewSetupColumnSlim { ColoumnId = f.Id, Width = Convert.ToDouble(f.Width), DisplayOrder = (int)f.DisplayOrder }).ToList();

                    if (notInTemplateTableCoumns != null)
                    {
                        templateTableCoumns.AddRange(notInTemplateTableCoumns);
                    }
                }
            }

            return gridViewSetups;
        }

        public async Task LoadGridViewFilters(int loginUserContactId, DynamicReportFilter dynamicReportFilter,
            DynamicReportPagePropertyVM dynamicReportPagePropertyVM,  bool relaodGrid = true, int newSelectedGridViewFilterId = 0)
        {
            //Move to main call
            //dataFilter.Filters = new List<CompositeFilterDescriptor>();
            //StateHasChanged();

            //Load Filter
            if (dynamicReportPagePropertyVM.selectedGridViewSetupId > 0)
            {
                dynamicReportPagePropertyVM.gridViewFilters = await _gridViewSetupService.GetGridViewFiltersBySetupId(loginUserContactId, dynamicReportPagePropertyVM.selectedGridViewSetupId);

                if (dynamicReportPagePropertyVM.gridViewFilters == null)
                {
                    dynamicReportPagePropertyVM.gridViewFilters = new List<GridViewFilterModel>();
                }

                //Add default Filter
                dynamicReportPagePropertyVM.gridViewFilters.Insert(0, new GridViewFilterModel { Id = -1, DisplayName = "Default Filter", Name = "Default Filter", LogicalOperator = LogicalFilterOperator.And.ToString(), GridViewSetupId = dynamicReportPagePropertyVM.selectedGridViewSetupId });

                GridViewFilterModel gridViewTempFilter;

                if (dynamicReportPagePropertyVM.gridViewFilters.Any(f => f.Id == dynamicReportPagePropertyVM.selectedGridViewFilterId))
                {
                    newSelectedGridViewFilterId = dynamicReportPagePropertyVM.selectedGridViewFilterId;
                }

                if (newSelectedGridViewFilterId > 0)
                {
                    gridViewTempFilter = dynamicReportPagePropertyVM.gridViewFilters.FirstOrDefault(f => f.Id == newSelectedGridViewFilterId);
                }
                else
                {
                    gridViewTempFilter = dynamicReportPagePropertyVM.gridViewFilters.FirstOrDefault(f => f.IsDefault);

                    if (gridViewTempFilter == null)
                    {
                        gridViewTempFilter = dynamicReportPagePropertyVM.gridViewFilters.FirstOrDefault(f => f.Id == -1);
                    }
                }

                if (gridViewTempFilter != null && gridViewTempFilter.Id != -1)
                {
                    //SEt parmiters
                    if (gridViewTempFilter.LogicalOperator == "Or")
                    {
                        dynamicReportPagePropertyVM.dataFilter.LogicalFilterOperator = LogicalFilterOperator.Or;
                    }
                    else
                    {
                        dynamicReportPagePropertyVM.dataFilter.LogicalFilterOperator = LogicalFilterOperator.And;
                    }

                    dynamicReportPagePropertyVM.selectedGridViewFilterId = gridViewTempFilter.Id;

                    var listOfFilter = JsonSerializer.Deserialize<List<CompositeFilterDescriptor>>(gridViewTempFilter.FilterJson);

                    if (listOfFilter != null && listOfFilter.Any())
                    {
                        //Populate Filter values
                        PopulateSetupFitlerValues(listOfFilter, dynamicReportPagePropertyVM.tableColumns);
                        //listOfFilter
                        dynamicReportPagePropertyVM.dataFilter.Filters = listOfFilter;
                    }
                    else
                    {
                        dynamicReportPagePropertyVM.selectedGridViewFilterId = 0;
                        dynamicReportPagePropertyVM.dataFilter.Filters = new List<CompositeFilterDescriptor>();
                    }
                    //dataFilter.Filters = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<CompositeFilterDescriptor>>(defaltFilter.FilterJson);

                }
                else
                {
                    dynamicReportPagePropertyVM.selectedGridViewFilterId = -1;

                    if (dynamicReportPagePropertyVM.gridViewFilters == null)
                    {
                        dynamicReportPagePropertyVM.gridViewFilters = new List<GridViewFilterModel>();
                        dynamicReportPagePropertyVM.gridViewFilters.Insert(0, new GridViewFilterModel { Id = -1, DisplayName = "Default Filter", Name = "Default Filter", LogicalOperator = LogicalFilterOperator.And.ToString(), GridViewSetupId = dynamicReportPagePropertyVM.selectedGridViewSetupId });
                    }

                    dynamicReportPagePropertyVM.dataFilter.LogicalFilterOperator = LogicalFilterOperator.And;
                }
            }
            else
            {
                dynamicReportPagePropertyVM.selectedGridViewFilterId = -1;

                if (dynamicReportPagePropertyVM.gridViewFilters == null)
                {
                    dynamicReportPagePropertyVM.gridViewFilters = new List<GridViewFilterModel>();
                    dynamicReportPagePropertyVM.gridViewFilters.Insert(0, new GridViewFilterModel { Id = -1, DisplayName = "Default Filter", Name = "Default Filter", LogicalOperator = LogicalFilterOperator.And.ToString(), GridViewSetupId = dynamicReportPagePropertyVM.selectedGridViewSetupId });
                }

                dynamicReportPagePropertyVM.dataFilter.LogicalFilterOperator = LogicalFilterOperator.And;
            }
        }

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

        public string ExtractName(string input)
        {
            // Use regular expression to extract the name within quotes
            Match match = Regex.Match(input, "\"(.*?)\"");

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty; // or handle the case when the name is not found
        }

        public string ConvertToTitleCase(string input)
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

        public async Task<IEnumerable<IDictionary<string, object>>> PopulateFilterColumnsAndQueryAndLoadData(LoadDataArgs args,
            DynamicReportFilter dynamicReportFilter, RadzenDataFilter<IDictionary<string, object>> dataFilter,
            List<ReportTableColumnModel> tableColumns, List<ReportTableColumnModel> allTableColumns, DynamicReportInfoModel selectDynamicReportInfo,
            List<ReportJoinInfoModel> selectedJoinList)
        {
            if (args != null)
            {
                dynamicReportFilter.Top = args.Top ?? 20;
                dynamicReportFilter.Skip = args.Skip ?? 0;
                //Add sorting
            }

            dynamicReportFilter.Where = ""; // PrepareFilterQuery(args?.Filters);

            if (dynamicReportFilter.Skip == 0)
            {
                dynamicReportFilter.IsCalculateTotal = true;
            }
            else
            {
                dynamicReportFilter.IsCalculateTotal = false;
            }

            var columnsNameOnFilter = new List<string>();
            dynamicReportFilter.Where = PrepareFilterQuery(args?.Filters, dataFilter?.Filters, columnsNameOnFilter,
                tableColumns, dataFilter.LogicalFilterOperator.ToString());

            //Add sorting
            PopulateSortFilterQuery(dynamicReportFilter, args?.Sorts, allTableColumns);

            dynamicReportFilter.SqlQuery = selectDynamicReportInfo.SqlScript;

            dynamicReportFilter.SqlType = selectDynamicReportInfo.SqlType;

            //
            var allFilterColumnIds = new List<int>();

            if (columnsNameOnFilter != null && columnsNameOnFilter.Any())
            {
                allFilterColumnIds = tableColumns.Where(f => columnsNameOnFilter.Any(i => i == f.FieldName)).Select(f => f.Id).ToList();
            }

            var selectedColumns = allTableColumns.Where(f => f.IsVisible == true && f.FieldName != "Select" && f.FieldName != "ActionsStart").ToList();

            if (selectedColumns != null && selectedColumns.Any())
            {
                allFilterColumnIds.AddRange(selectedColumns.Select(f => f.Id).ToList());
            }

            dynamicReportFilter.SelectedColumns = GetProductListSelectedFields(selectedColumns);

            dynamicReportFilter.ExtraJoin = GetProductListFilterJoins(allFilterColumnIds.ToList(),allTableColumns,selectedJoinList);

            dynamicReportFilter.MainQuery = selectDynamicReportInfo.SqlScript;

            //mainProducts = await _dynamicReportInfoService.GetRecordsDirectoryByFilterWithoutPaging(dynamicReportFilter);
            return await _dynamicReportInfoService.GetDirectoryUsingDynamicQueryByFilterWithPaging(dynamicReportFilter);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> PopulateFilterColumnsAndQueryAndLoadDataNewReport(LoadDataArgs args,
            DynamicReportFilter dynamicReportFilter, RadzenDataFilter<IDictionary<string, object>> dataFilter,
            List<ReportTableColumnModel> tableColumns, List<ReportTableColumnModel> allTableColumns, DynamicReportInfoModel selectDynamicReportInfo,
            List<ReportJoinInfoModel> selectedJoinList)
        {
            if (args != null)
            {
                dynamicReportFilter.Top = args.Top ?? 20;
                dynamicReportFilter.Skip = args.Skip ?? 0;
                //Add sorting
            }

            dynamicReportFilter.Where = ""; // PrepareFilterQuery(args?.Filters);

            if (dynamicReportFilter.Skip == 0)
            {
                dynamicReportFilter.IsCalculateTotal = true;
            }
            else
            {
                dynamicReportFilter.IsCalculateTotal = false;
            }

            var columnsNameOnFilter = new List<string>();
            dynamicReportFilter.Where = PrepareFilterQuery(args?.Filters, dataFilter?.Filters, columnsNameOnFilter,
                allTableColumns, dataFilter.LogicalFilterOperator.ToString());

            //Add sorting
            PopulateSortFilterQuery(dynamicReportFilter, args?.Sorts, tableColumns);

            dynamicReportFilter.SqlQuery = selectDynamicReportInfo.SqlScript;

            dynamicReportFilter.SqlType = selectDynamicReportInfo.SqlType;

            //
            var allFilterColumnIds = new List<int>();

            if (columnsNameOnFilter != null && columnsNameOnFilter.Any())
            {
                allFilterColumnIds = tableColumns.Where(f => columnsNameOnFilter.Any(i => i == f.FieldName)).Select(f => f.Id).ToList();
            }

            var selectedColumns = allTableColumns.Where(f => f.IsVisible == true && f.FieldName != "Select" && f.FieldName != "ActionsStart"
                                                            && tableColumns.Any(a=>a.Id== f.Id) ).ToList();

            if (selectedColumns != null && selectedColumns.Any())
            {
                allFilterColumnIds.AddRange(selectedColumns.Select(f => f.Id).ToList());
            }

            dynamicReportFilter.SelectedColumns = GetProductListSelectedFields(selectedColumns);

            dynamicReportFilter.ExtraJoin = GetProductListFilterJoins(allFilterColumnIds.ToList(), allTableColumns, selectedJoinList);

            dynamicReportFilter.MainQuery = selectDynamicReportInfo.SqlScript;

            //mainProducts = await _dynamicReportInfoService.GetRecordsDirectoryByFilterWithoutPaging(dynamicReportFilter);
            return await _dynamicReportInfoService.GetDirectoryUsingDynamicQueryByFilterWithPaging(dynamicReportFilter);
        }

        public async Task PopulateSelectedTableColIdsOnViewChange(DynamicReportPagePropertyVM dynamicReportPagePropertyVM, List<ReportTableColumnModel> allTableColumns,
            List<GridViewSetupModel> gridViewSetups)
        {
            dynamicReportPagePropertyVM.tableColumns = allTableColumns.Where(f => f.IsVisible == true).ToList();

            dynamicReportPagePropertyVM.selectedGridViewSetup = gridViewSetups.FirstOrDefault(f => f.Id == dynamicReportPagePropertyVM.selectedGridViewSetupId);

            if (dynamicReportPagePropertyVM.selectedGridViewSetupId == -1)
            {
                dynamicReportPagePropertyVM.selectedTableColumnIds = dynamicReportPagePropertyVM.tableColumns.Where(f => f.IsVisible == true).Select(f => f.Id).ToList();

                //templateTableCoumns = tableColumns.Select(f => new GridViewSetupColumnSlim { ColoumnId = f.Id, Width = Convert.ToDouble(f.Width), DisplayOrder = (int)f.DisplayOrder }).ToList();
            }
            else
            {
                var templateTableCoumns = await _gridViewSetupService.GetColumnsByGridViewSetupId(dynamicReportPagePropertyVM.selectedGridViewSetupId);

                if (templateTableCoumns != null)
                {
                    dynamicReportPagePropertyVM.selectedTableColumnIds = templateTableCoumns.Select(f => f.ColoumnId).ToList();

                    //Populate Table Columns
                    foreach (var col in templateTableCoumns)
                    {
                        var tabCol = dynamicReportPagePropertyVM.tableColumns.FirstOrDefault(f => f.Id == col.ColoumnId);
                        tabCol.DisplayOrder = col.DisplayOrder;
                        tabCol.Width = col.Width.ToString();
                    }
                }
            }
        }

        public Type GetTypeByString(TableFieldTypeSm fieldTypeEnum)
        {
            if (fieldTypeEnum == TableFieldTypeSm.Boolean)
            {
                return typeof(bool?);
            }
            else if (fieldTypeEnum == TableFieldTypeSm.Integer)
            {
                return typeof(int?);
            }
            else if (fieldTypeEnum == TableFieldTypeSm.Decimal)
            {
                return typeof(decimal?);
            }
            else if (fieldTypeEnum == TableFieldTypeSm.Date)
            {
                return typeof(DateTime?);
            }
            else
            {
                return typeof(string);
            }
        }
        #region Private Methods
        private string PrepareFilterQuery(IEnumerable<FilterDescriptor> filters, 
            IEnumerable<CompositeFilterDescriptor> topFilters, 
            List<string> filterColumns,
            List<ReportTableColumnModel> tableColumns,
            string topLogicalFilterOperator
            )
        {
            //IEnumerable<string> selectedNames;
            string where = string.Empty;
            string and = string.Empty;

            //Set group filter
            var items = string.Empty;
            #region grid header custom dropdown filters

            //Add client Company Filter

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

                    //For dynamic query builder
                    filterColumns.Add(actualFiledName);

                    var tableColumn = tableColumns.FirstOrDefault(f => f.FieldName == actualFiledName);
                    //var type = columns.Where(f => f.Key == actualFiledName).FirstOrDefault().Value;

                    actualFiledName = tableColumn.FieldWithPrefix;

                    if (tableColumn.FieldTypeEnum == TableFieldTypeSm.ShortText)
                    {
                        _gridFilterService.PopulateStringFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem);
                    }
                    else if (tableColumn.FieldTypeEnum == TableFieldTypeSm.Integer || tableColumn.FieldTypeEnum == TableFieldTypeSm.Decimal)
                    {
                        _gridFilterService.PopulateNumberFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem);
                    }
                    else if (tableColumn.FieldTypeEnum == TableFieldTypeSm.Boolean)
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
                    else if (tableColumn.FieldTypeEnum == TableFieldTypeSm.Date)
                    {
                        _gridFilterService.PopulateDateFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem);
                    }
                }
            }

            //For top filter
            if (topFilters != null && topFilters.Any())
            {
                //var topLogicalFilterOperator = dataFilter.LogicalFilterOperator.ToString();
                var topWhere = PopulateTopFiltersQuery(topFilters, topLogicalFilterOperator, filterColumns, tableColumns);

                if (!string.IsNullOrWhiteSpace(topWhere))
                {
                    where = $"{where}{and} ({topWhere})";
                    and = " AND ";
                }

            }

            if (!string.IsNullOrWhiteSpace(where))
                where = " WHERE " + where;

            return where;
        }

        private string PopulateTopFiltersQuery(IEnumerable<CompositeFilterDescriptor> topFilters, string logicalOperator,
            List<string> filterColumns, List<ReportTableColumnModel> tableColumns
            )
        {
            string where = "";
            var and = "";
            foreach (var filterItem in topFilters)
            {
                if (filterItem.Filters != null && filterItem.Filters.Any())
                {
                    var innerWhere = PopulateTopFiltersQuery(filterItem.Filters, filterItem.LogicalFilterOperator.ToString(), filterColumns, tableColumns);
                    where = $"{where} {and} ({innerWhere})";
                    and = $" {logicalOperator} ";
                    continue;
                }

                //For Optimize Query

                string pattern = @"it\[\""([^\""]+)\""\]\.ToString\(\)";

                Match match = Regex.Match(filterItem.Property, pattern);
                var actualFiledName = match.Groups[1].Value;

                filterColumns.Add(actualFiledName);

                var tableColumn = tableColumns.FirstOrDefault(f => f.FieldName == actualFiledName);
                //var type = columns.Where(f => f.Key == actualFiledName).FirstOrDefault().Value;

                actualFiledName = tableColumn.FieldWithPrefix;

                //var actualFiledName = GetActualFieldName(filterItem.Property);
                //NOt in database need to add those
                //Check boxes
                if (tableColumn.FieldTypeEnum == TableFieldTypeSm.ShortText)
                {
                    _gridFilterService.PopulateStringFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem, logicalOperator);
                }
                else if (tableColumn.FieldTypeEnum == TableFieldTypeSm.Integer || tableColumn.FieldTypeEnum == TableFieldTypeSm.Decimal)
                {
                    _gridFilterService.PopulateNumberFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem, logicalOperator);
                }
                else if (tableColumn.FieldTypeEnum == TableFieldTypeSm.Boolean)
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
                else if (tableColumn.FieldTypeEnum == TableFieldTypeSm.Date)
                {
                    _gridFilterService.PopulateDateFiltersForGridHeader(actualFiledName, ref where, ref and, filterItem, logicalOperator);
                }

            }

            return where;
        }

        private void PopulateSortFilterQuery(DynamicReportFilter filter, IEnumerable<SortDescriptor> sorts, List<ReportTableColumnModel> allTableColumns)
        {
            string sortClouse = string.Empty;

            //Set group filter
            if (sorts != null && sorts.Any())
            {
                var sortColumn = sorts.FirstOrDefault();

                string pattern = @"it\[\""([^\""]+)\""\]\.ToString\(\)";

                Match match = Regex.Match(sortColumn.Property, pattern);
                var actualFiledName = match.Groups[1].Value;

                var selectedColumn = allTableColumns.FirstOrDefault(f => f.FieldName == actualFiledName);

                if (selectedColumn != null)
                {
                    filter.SortColumn = selectedColumn.FieldName;
                }
                else
                {
                    filter.SortColumn = actualFiledName;
                }

                if (sortColumn.SortOrder == SortOrder.Ascending)
                {
                    filter.SortDirection = "ASC";
                }
                else
                {
                    filter.SortDirection = "DESC";
                }
            }
            else
            {
                //TODO: Read default order column
                var selectedColumn = allTableColumns.FirstOrDefault(f => f.SortingType != null);

                if (selectedColumn != null)
                {
                    filter.SortColumn = selectedColumn.FieldName;

                    if (selectedColumn.SortingType == (int)SortOrder.Ascending)
                    {
                        filter.SortDirection = "ASC";
                    }
                    else
                    {
                        filter.SortDirection = "DESC";
                    }
                }
                else
                {
                    var selectedColumn2 = allTableColumns.FirstOrDefault(f => f.IsVisible);
                    if (selectedColumn2 != null)
                    {
                        filter.SortColumn = selectedColumn2.FieldName;
                        filter.SortDirection = "ASC";
                    }
                }
            }
        }

        /// <summary>
        /// For Select query
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private string GetProductListSelectedFields(List<ReportTableColumnModel> columns)
        {
            if (columns == null || !columns.Any())
            {
                return "";
            }

            var listOfString = new List<string>();

            foreach (var column in columns)
            {
                if (!string.IsNullOrWhiteSpace(column.FieldWithPrefix))
                {
                    listOfString.Add(column.FieldWithPrefix);
                }
                else
                {
                    listOfString.Add(column.FieldName);
                }
            }

            if (listOfString.Any())
            {
                return "," + string.Join(",", listOfString);
            }

            return "";

        }

        private string GetProductListFilterJoins(List<int> columns, List<ReportTableColumnModel> allTableColumns, List<ReportJoinInfoModel> selectedJoinList)
        {
            if (columns == null || !columns.Any())
            {
                return "";
            }

            var listOfUsableJoinIds = new List<int>();

            var selectedColumns = allTableColumns.Where(f => columns.Any(i => i == f.Id)).ToList();

            foreach (var reportTableColumn in selectedColumns)
            {
                var joinInfoIds = new List<int>();

                if (reportTableColumn.JoinInfoId > 0)
                    joinInfoIds.Add((int)reportTableColumn.JoinInfoId);

                if (reportTableColumn.JoinInfo2Id > 0)
                    joinInfoIds.Add((int)reportTableColumn.JoinInfo2Id);

                if (reportTableColumn.JoinInfo3Id > 0)
                    joinInfoIds.Add((int)reportTableColumn.JoinInfo3Id);

                if (reportTableColumn.JoinInfo4Id > 0)
                    joinInfoIds.Add((int)reportTableColumn.JoinInfo4Id);

                if (reportTableColumn.JoinInfo5Id > 0)
                    joinInfoIds.Add((int)reportTableColumn.JoinInfo5Id);

                foreach (var joinInfoId in joinInfoIds)
                {
                    if (!listOfUsableJoinIds.Any(f => f== joinInfoId))
                    {
                        listOfUsableJoinIds.Add(joinInfoId);
                    }
                }
            }


            var listOfJoin = selectedJoinList.Where(f => listOfUsableJoinIds.Any(i => i == f.Id)).ToList();
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var item in listOfJoin)
            {
                stringBuilder.Append($" {item.JoinScript} ");
            }

            return stringBuilder.ToString();


            //var listOfJoinIds = allTableColumns.Where(f => columns.Any(i => i == f.Id)).Select(f => f.JoinInfoId).ToList();

            //StringBuilder stringBuilder = new StringBuilder();

            //var listOfJoin = selectedJoinList.Where(f => listOfJoinIds.Any(i => i == f.Id)).ToList();

            //foreach (var item in listOfJoin)
            //{
            //    stringBuilder.Append($" {item.JoinScript} ");
            //}

            //return stringBuilder.ToString();
        }
        #endregion end of private methods

    }
}
