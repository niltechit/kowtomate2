using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.DynamicReports;
using CutOutWiz.Services.Models.UI;
using KowToMateAdmin.Models.DynamicFilter;
using KowToMateAdmin.Models.Security;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Text.Json;

namespace KowToMateAdmin.Pages.DynamicReports.GenerateReports
{
    public partial class GetReportNew
    {
        #region FIlter
        DynamicReportPagePropertyVM dynamicReportPagePropertyVM = new DynamicReportPagePropertyVM();
        //RadzenDataFilter<IDictionary<string, object>> dataFilter;
        DynamicReportFilter dynamicReportFilter = new DynamicReportFilter();
        int count = 0;
        bool showGrid = false;
        bool isLoading = false;
        IEnumerable<int> pageSizeOptions = new int[] { 10, 20, 50, 100, 200, 500, 1000 };
        string pagingSummaryFormat = "Displaying page {0} of {1} (total {2} records)";

        private bool isExporting = false;
        #endregion

        #region Top Properties
        //RadzenDataFilter<IDictionary<string, object>> dataFilter;

        RadzenDataGrid<IDictionary<string, object>> grid;
        //IEnumerable<MainProduct> mainProducts = new List<MainProduct>();
        //IList<MainProduct> selectedProducts;

        IList<IDictionary<string, object>> selectedProducts;

        public IEnumerable<IDictionary<string, object>> mainProducts { get; set; }

        //public IDictionary<string, Type> columns = new Dictionary<string, Type>();

        private List<ReportTableColumnModel> allTableColumns = new List<ReportTableColumnModel>();
        

        private int? selectedDynamicReportInfoId = null;
        public List<DynamicReportInfoModel> dynamicReportList { get; set; }
        
        private DynamicReportInfoModel selectDynamicReportInfo = new DynamicReportInfoModel();
        
        private bool isSearching = false;
        private LoginUserInfoViewModel loginUser = null;

        private List<ReportJoinInfoModel> selectedJoinList = new List<ReportJoinInfoModel>();
        #endregion

        #region Top Filter Section
        private bool isShowTopFilter = true;
        private bool isFilterLoadingComplete = false;


        //[Parameter]
        //public int selectedGridViewSetupId { get; set; } = 0;
        private bool isShowManageGridViewTemplate = false;
        //private GridViewSetup selectedGridViewSetup = null;
        private List<GridViewSetupColumnSlim> selectedGridViewSetupColumn = new List<GridViewSetupColumnSlim>();

        //[Parameter]
        //public int selectedGridViewFilterId { get; set; } = 0;
        //List<GridViewFilter> gridViewFilters;
        //private GridViewFilter selectedGridViewFilter = null;
        bool isShowManageGridViewFilterTemplate = false;
        private string sortColumn = "";
        private string sortOrder = "";

        List<GridViewSetupModel> gridViewSetups;

        //List<GridViewSetupColumnSlim> selectedGridColumns = new List<GridViewSetupColumnSlim>();
        //dynamicReportFilter.selectedTableColumnIds = new List<int>();

        #endregion end of top filter section

        #region On Init and After and on render
        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            
            loginUser = _workContext.LoginUserInfo;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadActiveReports();
                StateHasChanged();
            }
        }

        private async Task LoadActiveReports()
        {
            //Set dynamicReportList
            dynamicReportList = await _dynamicReportInfoService.GetOnlyAssignedReportsByRoleObjectId(loginUser.UserObjectId);
        }

        //For grpuing
        void OnGridRender(DataGridRenderEventArgs<IDictionary<string, object>> args)
        {
            if (args.FirstRender)
            {
                if (allTableColumns.Any(f => f.IsDefaultGroup == true))
                {
                    var groupColumn = allTableColumns.FirstOrDefault(f => f.IsDefaultGroup);

                    args.Grid.Groups.Add(new GroupDescriptor() { Title = groupColumn.DisplayName, Property = _dynamicReportPageViewFilterService.GetColumnPropertyExpression(groupColumn.FieldName, _dynamicReportPageViewFilterService.GetTypeByString(groupColumn.FieldTypeEnum)), SortOrder = SortOrder.Ascending });
                    StateHasChanged();
                }
            }
        }
        #endregion


        async Task OnSelectedReportChange(object value)
        {
            isExporting = false;
            isSearching = false;
            showProgressBar = false;
            showGrid = false;
            mainProducts = new List<IDictionary<string, object>>();
            //columns = new Dictionary<string, Type>();
            allTableColumns = new List<ReportTableColumnModel>();
            dynamicReportPagePropertyVM.tableColumns = new List<ReportTableColumnModel>();
            await Task.Delay(10);

            if (selectedDynamicReportInfoId == 0 || selectedDynamicReportInfoId == null)
            {
                StateHasChanged();
                return;
            }

            //Get Report Id
            selectDynamicReportInfo = await _dynamicReportInfoService.GetById((int)selectedDynamicReportInfoId);

            //Set columns
            var dicColumns = new Dictionary<string, Type>();

            allTableColumns  = await _dynamicReportInfoService.GetTableColumnByDynamicReportInfoId((int)selectedDynamicReportInfoId);

            selectedJoinList = await _dynamicReportInfoService.GetJoinInfosByDynamicReportInfoId((int)selectedDynamicReportInfoId);

            //Load initial grid view and filter
            dynamicReportPagePropertyVM.tableColumns = allTableColumns.Where(f => f.IsVisible == true).ToList();

            gridViewSetups = await _dynamicReportPageViewFilterService.LoadGridViewTemplates(selectedDynamicReportInfoId,
                dynamicReportFilter, dynamicReportPagePropertyVM, loginUser.ContactId);

            dynamicReportPagePropertyVM.dataFilter.Filters = new List<CompositeFilterDescriptor>();
            //StateHasChanged(); //TODO: review this section remove it need
            await _dynamicReportPageViewFilterService.LoadGridViewFilters(loginUser.ContactId, dynamicReportFilter,dynamicReportPagePropertyVM, true);

            isFilterLoadingComplete = false;

            //Enable first time load
            //if (dataFilter.Filters != null && dataFilter.Filters.Any())
            //{
            //    await grid.FirstPage(true);
            //}

            isLoading = false;

            StateHasChanged();
        }

        /// <summary>
        /// Display report data
        /// </summary>
        /// <returns></returns>
        public async Task FindResults()
        {
            if (selectedDynamicReportInfoId == 0 || selectedDynamicReportInfoId == null)
            {
                ShowQuickNotification(new NotificationMessage { Severity = NotificationSeverity.Warning, Summary = "Warning!", Detail = "Select a Report.", Duration = 8000 });
                return;
            }

            showGrid = true;
            isSearching = true;
            await Task.Delay(10); // Introduce a small delay
            StateHasChanged();
            // await grid.Reload();
            //await grid.FirstPage(true);

            await LoadData(null);
            isSearching = false;
            await Task.Delay(10); // Introduce a small delay
            StateHasChanged();
        }

        /// <summary>
        /// Load report grid data
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        async Task LoadData(LoadDataArgs args)
        {
            if (selectDynamicReportInfo == null || selectDynamicReportInfo.Id == 0)
            {
                //isFilterLoadingComplete = true;
                mainProducts = new List<IDictionary<string, object>>();
                isLoading = false;
                count = 0;
                return;
            }

            if (!isFilterLoadingComplete)
            {
                //isFilterLoadingComplete = true;
                mainProducts = new List<IDictionary<string, object>>();
                isLoading = false;
                count = 0;
                return;
            }

            isLoading = true;

            var selectedTableColumns = dynamicReportPagePropertyVM.tableColumns.Where(f=> dynamicReportPagePropertyVM.selectedTableColumnIds.Any(s=>s==f.Id)).ToList();

            mainProducts = await _dynamicReportPageViewFilterService.PopulateFilterColumnsAndQueryAndLoadDataNewReport(args, 
                dynamicReportFilter,dynamicReportPagePropertyVM.dataFilter,
                selectedTableColumns, //dynamicReportPagePropertyVM.tableColumns,
                allTableColumns,selectDynamicReportInfo, selectedJoinList);

            // Important!!! Make sure the Count property of RadzenDataGrid is set.
            count = dynamicReportFilter.TotalCount;

            // Perform paginv via Skip and Take.
            isLoading = false;
            StateHasChanged();
        }

        #region Common Grid Methods

        void PickedColumnsChanged(DataGridPickedColumnsChangedEventArgs<IDictionary<string, object>> args)
        {
            Parallel.ForEach(allTableColumns, col =>
            {
                col.IsVisible = false;
            });

            var allTableColumnsDict = allTableColumns.ToDictionary(c => c.FieldName, c => c);

            foreach (var col in args.Columns)
            {
                var propertyName = _dynamicReportPageViewFilterService.ExtractName(col.Property);
                if (allTableColumnsDict.TryGetValue(propertyName, out var foundCol))
                {
                    foundCol.IsVisible = true;
                }
            }
        }

        void OnColumnReordered(DataGridColumnReorderedEventArgs<IDictionary<string, object>> args)
        {
            if (args.NewIndex > args.OldIndex)
            {
                var columns = allTableColumns.Where(f => f.DisplayOrder > args.OldIndex && f.DisplayOrder <= args.NewIndex).ToList();

                foreach (var column in columns)
                {
                    column.DisplayOrder = column.DisplayOrder - 1;
                }
            }
            else if (args.NewIndex < args.OldIndex)
            {
                var columns = allTableColumns.Where(f => f.DisplayOrder >= args.NewIndex && f.DisplayOrder < args.OldIndex).ToList();

                foreach (var column in columns)
                {
                    column.DisplayOrder = column.DisplayOrder + 1;
                }
            }
            var propertyName = _dynamicReportPageViewFilterService.ExtractName(args.Column.Property);
            
            var reorderedColumn = allTableColumns.FirstOrDefault(f => f.FieldName == propertyName);
            reorderedColumn.DisplayOrder = args.NewIndex;
        }

        #endregion

        #region Filter top section
        private async Task ApplyFilterClicked()
        {
            //StateHasChanged();
            isFilterLoadingComplete = true;
            await grid.FirstPage(true);
        }

        #region Grid View Filter
        private async Task ShowHideTopFilter_Click()
        {
            isShowTopFilter = !isShowTopFilter;

            await js.InvokeVoidAsync("showHideTopFilter", isShowTopFilter);
        }

        async Task OnGridViewFilterChange(object value)
        {
            //await Task.Yield();
            dynamicReportPagePropertyVM.dataFilter.Filters = new List<CompositeFilterDescriptor>();
            StateHasChanged();

            if (dynamicReportPagePropertyVM.selectedGridViewFilterId > 0)
            {
                dynamicReportPagePropertyVM.selectedGridViewFilter = dynamicReportPagePropertyVM.gridViewFilters.FirstOrDefault(f => f.Id == dynamicReportPagePropertyVM.selectedGridViewFilterId);
                //SEt parmiters
                if (dynamicReportPagePropertyVM.selectedGridViewFilter.LogicalOperator == "Or")
                {
                    dynamicReportPagePropertyVM.dataFilter.LogicalFilterOperator = LogicalFilterOperator.Or;
                }
                else
                {
                    dynamicReportPagePropertyVM.dataFilter.LogicalFilterOperator = LogicalFilterOperator.And;
                }

                var listOfFilter = JsonSerializer.Deserialize<List<CompositeFilterDescriptor>>(dynamicReportPagePropertyVM.selectedGridViewFilter.FilterJson);

                if (listOfFilter != null && listOfFilter.Any())
                {
                    //Populate Filter values
                    _dynamicReportPageViewFilterService.PopulateSetupFitlerValues(listOfFilter,allTableColumns);

                    dynamicReportPagePropertyVM.dataFilter.Filters = listOfFilter;

                    await grid.FirstPage(true);
                }
                else
                {
                    mainProducts = new List<IDictionary<string, object>>();
                    isLoading = false;
                    count = 0;
                }

                StateHasChanged();
            }
            else
            {
                dynamicReportPagePropertyVM.dataFilter.LogicalFilterOperator = LogicalFilterOperator.And;
                //await grid.FirstPage(true);

                mainProducts = new List<IDictionary<string, object>>();
                isLoading = false;
                count = 0;
                //StateHasChanged();
            }

        }

        private async Task UpdateConfirmGridViewFilter_Click(int selId = 0)
        {
            isShowManageGridViewFilterTemplate = false;

            if (selId > 0)
            {
                //Load Filter
                dynamicReportPagePropertyVM.dataFilter.Filters = new List<CompositeFilterDescriptor>();
                StateHasChanged();
                await  _dynamicReportPageViewFilterService.LoadGridViewFilters(loginUser.ContactId, dynamicReportFilter, dynamicReportPagePropertyVM, true, selId);
                StateHasChanged();

                //isFilterLoadingComplete = true;
                //await grid.FirstPage(true);

            }
            else if (selId == -100)
            {
                dynamicReportPagePropertyVM.selectedGridViewFilterId = 0;
                dynamicReportPagePropertyVM.dataFilter.Filters = new List<CompositeFilterDescriptor>();
                StateHasChanged();
                await _dynamicReportPageViewFilterService.LoadGridViewFilters(loginUser.ContactId, dynamicReportFilter, dynamicReportPagePropertyVM, true, selId);
                StateHasChanged();
            }
        }

        async Task SaveGridViewFilter()
        {
            await Task.Yield();

            if (dynamicReportPagePropertyVM.selectedGridViewSetupId == -1)
            {
                ShowQuickNotification(new NotificationMessage { Severity = NotificationSeverity.Warning, Summary = "Warning!", Detail = "Please choose other template for save filter, instead of 'Default System View'.", Duration = 5000 });
                return;
            }

            if (dynamicReportPagePropertyVM.dataFilter.Filters == null || !dynamicReportPagePropertyVM.dataFilter.Filters.Any())
            {
                ShowQuickNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error!", Detail = "Set atleast one filter.", Duration = 5000 });
                return;
            }

            dynamicReportPagePropertyVM.selectedGridViewFilter = dynamicReportPagePropertyVM.gridViewFilters.FirstOrDefault(f => f.Id == dynamicReportPagePropertyVM.selectedGridViewFilterId);

            if (dynamicReportPagePropertyVM.selectedGridViewFilter == null)
            {
                dynamicReportPagePropertyVM.selectedGridViewFilter = new GridViewFilterModel();
                dynamicReportPagePropertyVM.selectedGridViewFilter.ContactId = loginUser.ContactId;
            }

            dynamicReportPagePropertyVM.selectedGridViewFilter.GridViewSetupId = dynamicReportPagePropertyVM.selectedGridViewSetupId;
            dynamicReportPagePropertyVM.selectedGridViewFilter.FilterJson = JsonSerializer.Serialize(dynamicReportPagePropertyVM.dataFilter.Filters.ToList());

            dynamicReportPagePropertyVM.selectedGridViewFilter.LogicalOperator = dynamicReportPagePropertyVM.dataFilter.LogicalFilterOperator.ToString();

            dynamicReportPagePropertyVM.selectedGridViewFilter.SortColumn = sortColumn;
            dynamicReportPagePropertyVM.selectedGridViewFilter.SortOrder = sortOrder;

            isShowManageGridViewFilterTemplate = true;
        }

        //void OnSort(DataGridColumnSortEventArgs<MainProduct> args)
        //{
        //    sortOrder = args.SortOrder.ToString();
        //    sortColumn = args.Column.Property;
        //}

        private async Task UpdateConfirmGridViewTemplate_Click(int selId = 0)
        {
            isShowManageGridViewTemplate = false;

            if (selId > 0)
            {
                dynamicReportPagePropertyVM.selectedGridViewSetupId = selId;
                
            }
            else if (selId == -100)
            {
                dynamicReportPagePropertyVM.selectedGridViewSetupId = 0;
            }

            dynamicReportPagePropertyVM.tableColumns = allTableColumns.Where(f => f.IsVisible == true).ToList();
            gridViewSetups = await _dynamicReportPageViewFilterService.LoadGridViewTemplates(selectedDynamicReportInfoId,
                dynamicReportFilter, dynamicReportPagePropertyVM, loginUser.ContactId);

            StateHasChanged();
        }

        async Task OnGridViewChange(object value)
        {
            //var templateTableCoumns = new List<GridViewSetupColumnSlim>();

            //tableColumns = _mainProductService.GetAllTableColumns();
            await  _dynamicReportPageViewFilterService.PopulateSelectedTableColIdsOnViewChange(dynamicReportPagePropertyVM,allTableColumns,gridViewSetups);

            dynamicReportPagePropertyVM.dataFilter.Filters = new List<CompositeFilterDescriptor>();
            StateHasChanged(); //TODO: review this section remove it need
            await _dynamicReportPageViewFilterService.LoadGridViewFilters(loginUser.ContactId, dynamicReportFilter, dynamicReportPagePropertyVM, true, 0);

            //isFilterLoadingComplete = true;
            isFilterLoadingComplete = true;
            if (dynamicReportPagePropertyVM.dataFilter.Filters != null && dynamicReportPagePropertyVM.dataFilter.Filters.Any())
            {
                await grid.FirstPage(true);
            }
            else
            {
                mainProducts = new List<IDictionary<string, object>>();
                isLoading = false;
                count = 0;
            }

            StateHasChanged();

        }

        async Task OnSelectedTableColumnChange(object value)
        {
            await Task.Yield();
            await grid.Reload();
            StateHasChanged();
        }

        async Task openGridViewTemplateManage()
        {
            await Task.Yield();
            if (dynamicReportPagePropertyVM.selectedTableColumnIds == null || !dynamicReportPagePropertyVM.selectedTableColumnIds.Any())
            {
                ShowQuickNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error!", Detail = "Select atleast one column.", Duration = 4000 });
                return;
            }

            isShowManageGridViewTemplate = true;

            var newTableColumns = new List<GridViewSetupColumnSlim>();

            foreach (var columnId in dynamicReportPagePropertyVM.selectedTableColumnIds)
            {
                var tableColumn = dynamicReportPagePropertyVM.tableColumns.FirstOrDefault(f => f.Id == columnId);
                newTableColumns.Add(new GridViewSetupColumnSlim { ColoumnId = columnId, Width = Convert.ToDouble(tableColumn.Width), DisplayOrder = (int)tableColumn.DisplayOrder });
            }

            selectedGridViewSetupColumn = newTableColumns;
        }
        #endregion

        #endregion

        #region Export Excel
 
        bool showProgressBar = false;
        int progressBarCurrentValue = 0;

        public async Task DownloadExcelDocument()
        {
            if (selectedDynamicReportInfoId == 0  || selectedDynamicReportInfoId == null)
            {
                ShowQuickNotification(new NotificationMessage { Severity = NotificationSeverity.Warning, Summary = "Warning!", Detail = "Select a Report.", Duration = 8000 });
                return;
            }

            isExporting = true;
            showProgressBar = true;
            progressBarCurrentValue = 0;
            await Task.Delay(20); // Introduce a small delay
            StateHasChanged();

            var fileName = $"{selectDynamicReportInfo.Name} {DateTime.Now.ToString("dd MM yyyy h mm tt")}.xlsx";

            IEnumerable<IDictionary<string, object>> itemsForExport;

            if (selectedProducts != null && selectedProducts.Any())
            {
                itemsForExport = selectedProducts.ToList();
            }
            else
            {
                itemsForExport = await _dynamicReportInfoService.GetDirectoryUsingDynamicQueryByFilterWithoutPaging(dynamicReportFilter);
            }

            if (itemsForExport == null)
            {
                ShowQuickNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error!", Detail = "Select atleast one product.", Duration = 4000 });
                showProgressBar = false;
                isExporting = false;
                return;
            }

            var tableColumnsToDisplay = allTableColumns.Where(f => dynamicReportPagePropertyVM.selectedTableColumnIds.Any(o => o == f.Id)).OrderBy(f => f.DisplayOrder).ToList();

            var defaultColumns = tableColumnsToDisplay.FirstOrDefault(f => f.IsDefaultGroup);

            byte[] bytes = null;

            if (defaultColumns == null)
            {
                bytes = await _dataImportService.GenerateExcel(itemsForExport, tableColumnsToDisplay, "Report", progressCount =>
                {
                    progressBarCurrentValue = (int)(((decimal)progressCount / count) * 100);
                    StateHasChanged();
                    // Update the UI with the progress count
                    // e.g., display it in a progress bar, show a message, etc.
                });
            }
            else
            {
                bytes = await _dataImportService.GenerateExcel(itemsForExport, tableColumnsToDisplay, "Report", progressCount =>
                {
                    progressBarCurrentValue = (int)(((decimal)progressCount / count) * 100);
                    StateHasChanged();
                    // Update the UI with the progress count
                    // e.g., display it in a progress bar, show a message, etc.
                }, defaultColumns.FieldName);
            }

            await js.InvokeAsync<object>("saveAsFile", fileName, Convert.ToBase64String(bytes));

            isExporting = false;
            showProgressBar = false;
            await Task.Delay(20); // Introduce a small delay
            StateHasChanged();
        }

        void ShowQuickNotification(NotificationMessage message)
        {
            _notificationService.Notify(message);
        }
        #endregion

    }
}
    
