using CutOutWiz.Services.Models.DynamicReports;
using KowToMateAdmin.Models.Security;
using KowToMateAdmin.Pages.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace KowToMateAdmin.Pages.DynamicReports.DynamicReportSetup
{
    public partial class DynamicReportManageJoin
    {
        #region Private Members

        [Parameter]
        public DynamicReportInfoModel dynamicReportInfo { get; set; }

        [Parameter]
        public EventCallback CloseAddEditPopupChanged { get; set; }

        RadzenDataGrid<ReportJoinInfoModel> grid;

        IEnumerable<ReportJoinInfoModel> ReportJoinData = new List<ReportJoinInfoModel>();

        protected ReportJoinInfoModel reportJoinInfo = new ReportJoinInfoModel();
        private LoginUserInfoViewModel loginUser = null;
        protected ModalNotification ModalNotification { get; set; }

        int countReportJoin = 0;
        int selectedId = 0;

        bool isLoadingReportJoin = false;
        bool isSubmitting = false;

        #endregion

        #region Init
        /// <summary>
        /// Initial page load before render
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            loginUser = _workContext.LoginUserInfo;

            await Task.Yield();
        }

        /// <summary>
        /// Data loading after render the page
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await Task.Yield();

            if (firstRender)
            {
                reportJoinInfo.DynamicReportInfoId = dynamicReportInfo.Id;
                await grid.FirstPage(true);
                StateHasChanged();
            }
        }

        #endregion

        #region Load Data
        /// <summary>
        /// Load join data for grid
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        async Task LoadData(LoadDataArgs args)
        {
            isLoadingReportJoin = true;
            ReportJoinData = await _dynamicReportInfoService.GetReportJoinInfoListByDynamicReportInfoId(dynamicReportInfo.Id);

            countReportJoin = ReportJoinData.Count();
            isLoadingReportJoin = false;
            StateHasChanged();
        }

        #endregion

        #region Insert Or Update
        /// <summary>
        /// Insert or update join info
        /// </summary>
        /// <returns></returns>
        private async Task InsertUpdateJoinfInfo()
        {
            isSubmitting = true;

            if (reportJoinInfo == null || reportJoinInfo.Id == 0)
            {
                reportJoinInfo.DynamicReportInfoId = dynamicReportInfo.Id;
                var addResponse = await _dynamicReportInfoService.InsertReportJoinInfo(reportJoinInfo);

                if (!addResponse.IsSuccess)
                {
                    ModalNotification.ShowMessage("Error", addResponse.Message);
                    ShowQuickNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error!", Detail = addResponse.Message, Duration = 8000 });

                    isSubmitting = false;
                    return;
                }
            }
            else {
                var updateResponse = await _dynamicReportInfoService.UpdateReportJoinInfo(reportJoinInfo);

                if (!updateResponse.IsSuccess)
                {
                    ModalNotification.ShowMessage("Error", updateResponse.Message);
                    ShowQuickNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error!", Detail = updateResponse.Message, Duration = 8000 });

                    isSubmitting = false;
                    return;
                }
            }

            isSubmitting = false;
            reportJoinInfo = new ReportJoinInfoModel();
            await grid.FirstPage(true);
        }

        /// <summary>
        /// Dispaly Join info add / edit popuup
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task Edit(int id)
        {
            reportJoinInfo = await _dynamicReportInfoService.GetReportJoinInfoById(id);
            StateHasChanged();
        }

        #endregion

        #region Delete Report Join
        /// <summary>
        /// Display Delete Report Join dialog
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        protected void Delete(int id, string name)
        {
            selectedId = id;
            var msg = $"Are you sure you want to delete Report Join \"{name}\"?";
            ModalNotification.ShowConfirmation("Confirm Delete", msg);
        }

        /// <summary>
        /// Delete Report join from database after confirmaiton
        /// </summary>
        /// <param name="deleteConfirmed"></param>
        /// <returns></returns>
        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                var deleteResponse = await _dynamicReportInfoService.DeleteReportJoinInfo(selectedId);

                if (deleteResponse.IsSuccess)
                {
                    await grid.FirstPage(true);
                }
                else
                {
                    ModalNotification.ShowMessage("Error", deleteResponse.Message);
                }
            }

            isSubmitting = false;
        }

        #endregion

        #region Close Modal Popup
        /// <summary>
        /// Hide add /edit report join popup
        /// </summary>
        void CloseAddEditPopup()
        {            
            CloseAddEditPopupChanged.InvokeAsync();
        }

        #endregion

        #region Notification
        /// <summary>
        /// Show top right quick notificaiton
        /// </summary>
        /// <param name="message"></param>
        void ShowQuickNotification(NotificationMessage message)
        {
            _notificationService.Notify(message);
        }

        #endregion

    }
}
