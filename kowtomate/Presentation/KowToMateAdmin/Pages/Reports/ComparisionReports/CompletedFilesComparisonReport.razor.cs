using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core.ComparisionReports;
using CutOutWiz.Core.Utilities;
using KowToMateAdmin.Models.Security;
using Radzen.Blazor;
using System.Globalization;

namespace KowToMateAdmin.Pages.Reports.ComparisionReports
{
    public partial class CompletedFilesComparisonReport
    {
        #region Private Members

        //instance
        private LoginUserInfoViewModel loginUser = null;

        public List<CompanyModel> companies { get; set; }

        //list
        //monthly sales amount
        IEnumerable<ChartDataDeciamlViewModel> currentYearSalesAmountData = new List<ChartDataDeciamlViewModel>();
        IEnumerable<ChartDataDeciamlViewModel> previousYearSalesAmountData = new List<ChartDataDeciamlViewModel>();

        IEnumerable<ChartDataDeciamlViewModel> currentYearSalesQtyData = new List<ChartDataDeciamlViewModel>();
        IEnumerable<ChartDataDeciamlViewModel> previousYearSalesQtyData = new List<ChartDataDeciamlViewModel>();

        //last 30 days sales amount        
        IEnumerable<ChartDataDeciamlViewModel> currentYearLast30DaysSalesAmountData = new List<ChartDataDeciamlViewModel>();
        IEnumerable<ChartDataDeciamlViewModel> previousYearLast30DaysAmountData = new List<ChartDataDeciamlViewModel>();

        IEnumerable<ChartDataDeciamlViewModel> currentYearLast30DaysSalesQtyData = new List<ChartDataDeciamlViewModel>();
        IEnumerable<ChartDataDeciamlViewModel> previousYearLast30DaysQtyData = new List<ChartDataDeciamlViewModel>();

        List<CustomNameIntValuePairModel> salesYearDataList = new List<CustomNameIntValuePairModel>();

        //boolean
        bool showDataLabels = false;
        bool showDataLabelsForRefund = false;
        bool showDataLabelsLast30Days = false;

        //int 
        int endMonthOfCurrDate;
        int endMonthOfPrevDate;

        //int currentYearMonth;
       // int currentYearDay;

        //string
        string currentYear = "Current Year";
        string previousYear = "Previous Year";

        string currentYearLast30Days = "Current Year";
        string previousYearLast30Days = "Previous Year";

        int selectedSalesYear = 0;

        List<int?> selectedCompanyIds;

        Interpolation interpolation = Interpolation.Line;
        #endregion

        #region Init

        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            loginUser = _workContext.LoginUserInfo;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await GetSalesYearsForDropdown();

                companies = await _companyService.GetAll();

                var filter = new CompletedFilesComparisonReportFilter();
                PopulateYearDateFilter(selectedSalesYear, filter);
                PopulateYearLast30DaysDateFilter(selectedSalesYear, filter);
                await LoadChartLineSeriesData(filter);
                await LoadLast30DaysSalesChartLineSeriesData(filter);
                StateHasChanged();
            }
        }

        #endregion

        #region Chart With Line Series

        #region On Change Events
        private async Task OnSelectedPlatformChange(object value)
        {
            await Task.Yield();

            if (selectedCompanyIds != null && !selectedCompanyIds.Any())
            {
                selectedCompanyIds = null;
            }

            var filter = new CompletedFilesComparisonReportFilter();
            PopulateYearDateFilter(selectedSalesYear, filter);
            PopulateYearLast30DaysDateFilter(selectedSalesYear, filter);

            await LoadChartLineSeriesData(filter);
            await LoadLast30DaysSalesChartLineSeriesData(filter);
            StateHasChanged();
        }

        async Task OnSalesYearChange(object value)
        {
            var filter = new CompletedFilesComparisonReportFilter();
            PopulateYearDateFilter(selectedSalesYear, filter);
            PopulateYearLast30DaysDateFilter(selectedSalesYear, filter);

            await LoadChartLineSeriesData(filter);
            await LoadLast30DaysSalesChartLineSeriesData(filter);
            StateHasChanged();
        }

        async Task LoadChartLineSeriesData(CompletedFilesComparisonReportFilter filter)
        {
            filter.SelectedCompanyIds = selectedCompanyIds;

            //If December then show year instead of current 
            if (filter.CurrentEndDate.Month != 12)
            {
                currentYear = $"{filter.CurrentStartDate.ToString("MMM")} {(filter.CurrentStartDate.Year - 2000).ToString()} - {filter.CurrentEndDate.ToString("MMM")} {(filter.CurrentEndDate.Year - 2000).ToString()} (Cur)";
                previousYear = $"{filter.PreviousStartDate.ToString("MMM")} {(filter.PreviousStartDate.Year - 2000).ToString()} - {filter.PreviousEndDate.ToString("MMM")} {(filter.PreviousEndDate.Year - 2000).ToString()} (Pre)";
            }
            else
            {
                currentYear = filter.CurrentEndDate.Year.ToString();
                previousYear = filter.PreviousEndDate.Year.ToString();
            }

            //monthly sales
            await GetMonthlyDeliveredAmountData(filter);
            await GetMonthlyDeliveryQtyData(filter);
        }

        async Task LoadLast30DaysSalesChartLineSeriesData(CompletedFilesComparisonReportFilter filter)
        {
            filter.SelectedCompanyIds = selectedCompanyIds;

            currentYearLast30Days = filter.CurrentLast30EndDate.Year.ToString();
            previousYearLast30Days = filter.PreviousLast30EndDate.Year.ToString();
            
            //monthly sales
            await GetLast30DaysDeliveryAmountData(filter);
            await GetLast30DaysSalesQuantityData(filter);
        }

        #endregion

        async Task GetMonthlyDeliveredAmountData(CompletedFilesComparisonReportFilter filter)
        {
            try
            {
                var salesAnalysisData = await _completedFilesComparisionReportService.GetDeliveryAnalysisReportAmountData(filter);

                //Populate Curent Year Date
                currentYearSalesAmountData = GetFinalChartData(filter.CurrentStartDate, filter.CurrentEndDate, salesAnalysisData);

                //Populate Old year data
                previousYearSalesAmountData = GetFinalChartData(filter.PreviousStartDate, filter.PreviousEndDate, salesAnalysisData);
            }
            catch (Exception ex)
            {

            }
        }

        async Task GetMonthlyDeliveryQtyData(CompletedFilesComparisonReportFilter filter)
        {
            try
            {
                var salesAnalysisData = await _completedFilesComparisionReportService.GetDeliveryAnalysisReportQtyData(filter);

                //Populate Curent Year Date
                currentYearSalesQtyData = GetFinalChartData(filter.CurrentStartDate, filter.CurrentEndDate, salesAnalysisData);

                //Populate Old year data
                previousYearSalesQtyData = GetFinalChartData(filter.PreviousStartDate, filter.PreviousEndDate, salesAnalysisData);
            }
            catch (Exception ex)
            {

            }
        }       

        async Task GetLast30DaysDeliveryAmountData(CompletedFilesComparisonReportFilter filter)
        {
            try
            {
                var salesAnalysisData = await _completedFilesComparisionReportService.GetLast30DaysDeliveryAnalysisReportAmountData(filter);

                //Populate Curent Year Date
                currentYearLast30DaysSalesAmountData = GetLast30DaysFinalChartData(filter.CurrentLast30StartDate, filter.CurrentLast30EndDate, salesAnalysisData);

                //Populate Old year data
                previousYearLast30DaysAmountData = GetLast30DaysFinalChartData(filter.PreviousLast30StartDate, filter.PreviousLast30EndDate, salesAnalysisData);
            }
            catch (Exception ex)
            {
            }
        }

        async Task GetLast30DaysSalesQuantityData(CompletedFilesComparisonReportFilter filter)
        {
            try
            {
                var salesAnalysisData = await _completedFilesComparisionReportService.GetLast30DaysDeliveryAnalysisReportQtyData(filter);

                //Populate Curent Year Date
                currentYearLast30DaysSalesQtyData = GetLast30DaysFinalChartData(filter.CurrentLast30StartDate, filter.CurrentLast30EndDate, salesAnalysisData);

                //Populate Old year data
                previousYearLast30DaysQtyData = GetLast30DaysFinalChartData(filter.PreviousLast30StartDate, filter.PreviousLast30EndDate, salesAnalysisData);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Populdate final chart date
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="sourceData"></param>
        /// <returns></returns>
        private List<ChartDataDeciamlViewModel> GetFinalChartData(DateTime startDate, DateTime endDate, List<AnalysisChartReportModel> sourceData)
        {
            var newSalesAnalysisData = new List<ChartDataDeciamlViewModel>();

            DateTime currentLoopDate = startDate;

            for (int i = 1; i <= 12; i++)
            {
                // Your logic for each iteration
                var foundItem = sourceData.FirstOrDefault(f => f.Year == currentLoopDate.Year && f.Month == currentLoopDate.Month);


                if (foundItem != null)
                {
                    newSalesAnalysisData.Add(new ChartDataDeciamlViewModel { Label = new DateTime(currentLoopDate.Year, currentLoopDate.Month, 1).ToString("MMM"), Value = foundItem.Value });
                }
                else
                {
                    newSalesAnalysisData.Add(new ChartDataDeciamlViewModel { Label = new DateTime(currentLoopDate.Year, currentLoopDate.Month, 1).ToString("MMM"), Value = 0 });
                }

                // Move to the next month
                currentLoopDate = currentLoopDate.AddMonths(1);
            }

            return newSalesAnalysisData;
        }

        private List<ChartDataDeciamlViewModel> GetLast30DaysFinalChartData(DateTime startDate, DateTime endDate, List<AnalysisChartReportModel> sourceData)
        {
            var newSalesAnalysisData = new List<ChartDataDeciamlViewModel>();

            DateTime currentLoopDate = startDate;

            for (int i = 1; i <= 31; i++)
            {
                // Your logic for each iteration
                var foundItem = sourceData.FirstOrDefault(f => f.Year == currentLoopDate.Year && f.Month == currentLoopDate.Month && f.Day == currentLoopDate.Day);

                if (foundItem != null)
                {
                    newSalesAnalysisData.Add(new ChartDataDeciamlViewModel { Label = new DateTime(currentLoopDate.Year, currentLoopDate.Month, currentLoopDate.Day).ToString("dd MMM"), Value = foundItem.Value });
                }
                else
                {
                    newSalesAnalysisData.Add(new ChartDataDeciamlViewModel { Label = new DateTime(currentLoopDate.Year, currentLoopDate.Month, currentLoopDate.Day).ToString("dd MMM"), Value = 0 });
                }

                // Move to the next month
                currentLoopDate = currentLoopDate.AddDays(1);
            }

            return newSalesAnalysisData;
        }

        private void PopulateYearDateFilter(int? selYear, CompletedFilesComparisonReportFilter filter)
        {
            if (selYear == null || selYear == 0)
            {

                // Get the current date
                DateTime currentDate = DateTime.UtcNow.AddHours(-5);
                // End date is the last day of the current month
                filter.CurrentEndDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            }
            else
            {
                // End date is the last day of the current year
                filter.CurrentEndDate = new DateTime((int)selYear, 12, DateTime.DaysInMonth((int)selYear, 12));
            }

            // Start date is 12 months before the end date
            filter.CurrentStartDate = filter.CurrentEndDate.AddMonths(-12).AddDays(1);

            filter.PreviousEndDate = filter.CurrentStartDate.AddDays(-1);
            filter.PreviousStartDate = filter.PreviousEndDate.AddMonths(-12).AddDays(1);

            //Set end time of the day
            filter.CurrentEndDate = filter.CurrentEndDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            filter.PreviousEndDate = filter.PreviousEndDate.AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        private void PopulateYearLast30DaysDateFilter(int? selYear, CompletedFilesComparisonReportFilter filter)
        {
            // Get the current date
            DateTime currentDate = DateTime.UtcNow.AddHours(-5);
            
            if (filter.CurrentEndDate <= currentDate)
            {
                filter.CurrentLast30EndDate = filter.CurrentEndDate;
                filter.CurrentLast30StartDate = filter.CurrentLast30EndDate.AddDays(-30);

                filter.PreviousLast30EndDate = filter.PreviousEndDate;
                filter.PreviousLast30StartDate = filter.PreviousLast30EndDate.AddDays(-30);
            }
            else
            {
                filter.CurrentLast30EndDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);
                filter.CurrentLast30StartDate = filter.CurrentLast30EndDate.AddDays(-30);

                filter.PreviousLast30EndDate = filter.CurrentLast30EndDate.AddYears(-1);
                filter.PreviousLast30StartDate = filter.PreviousLast30EndDate.AddDays(-30);

                filter.CurrentLast30EndDate = filter.CurrentLast30EndDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                filter.PreviousLast30EndDate = filter.PreviousLast30EndDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
        }

        string FormatAsUSD(object value)
        {
            return ((double)value).ToString("C0", CultureInfo.CreateSpecificCulture("en-US"));
        }

        string FormatAsComma(object value)
        {
            return ((double)value).ToString("N0");
        }

        #endregion

        #region Dropdown list

        public async Task GetSalesYearsForDropdown()
        {
            await Task.Yield();

            salesYearDataList = new List<CustomNameIntValuePairModel> { new CustomNameIntValuePairModel { Name = "Last 12 Months", Value = 0 } };
            var currentYear = DateTime.UtcNow.AddHours(-6).Year;

            for (int i = 1; i <= 10; i++)
            {
                salesYearDataList.Add(new CustomNameIntValuePairModel { Name = $"Year {currentYear}", Value = currentYear });
                currentYear--;
            }
        }

        #endregion

        #region Private Methods      

        #endregion
    }
}
