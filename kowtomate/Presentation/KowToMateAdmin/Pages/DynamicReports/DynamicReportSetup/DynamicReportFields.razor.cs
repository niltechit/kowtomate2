using CutOutWiz.Services.Models.DynamicReports;
using CutOutWiz.Core;
using KowToMateAdmin.Models.Security;
using KowToMateAdmin.Pages.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Core.Utilities;

namespace KowToMateAdmin.Pages.DynamicReports.DynamicReportSetup
{
    public partial class DynamicReportFields
    {
        #region Private Variables

        #region Paramiters
        [Parameter]
        public string reportId { get; set; }
        #endregion

        protected ModalNotification ModalNotification { get; set; }

        bool isSubmitting;
        private List<ReportTableColumnModel> dynamicReportFields;
        IEnumerable<int> pageSizeOptions = new int[] { 10, 20, 50, 100, 500 };
        bool isLoading = true;
        private ReportTableColumnModel reportTableColumn = null;

        private DynamicReportInfoModel dynamicReportInfo = null;
        bool isReportInfoPopupVisible = false;
        bool isPopupVisible = false;
        private LoginUserInfoViewModel loginUser = null;
        private int selectedTableColumnId = 0;
        List<CustomEnumTypes> statuses = new List<CustomEnumTypes>();

        bool isReportJoinPopupVisible = false;

        #endregion

        #region Init

        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            loginUser = _workContext.LoginUserInfo;
        }

        /// <summary>
        /// Load data after render page
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isLoading = true;
                foreach (GeneralStatus item in Enum.GetValues(typeof(GeneralStatus)))
                {
                    statuses.Add(new CustomEnumTypes { EnumName = item.ToString(), EnumValue = Convert.ToByte((int)item) });
                }
                isLoading = false;

                dynamicReportInfo = await _dynamicReportInfoService.GetById(Convert.ToInt32(reportId));

                if (dynamicReportInfo == null)
                {
                    BackToReports();
                    return;
                }

                await LoadDynamicReportFields(dynamicReportInfo.Id);
                StateHasChanged();
            }
        }

        #endregion

        #region Load Data
        /// <summary>
        /// Load report fields
        /// </summary>
        /// <param name="dynamicReportId"></param>
        /// <returns></returns>
        private async Task LoadDynamicReportFields(int dynamicReportId)
        {
            dynamicReportFields = await _dynamicReportInfoService.GetAllTableColumnByDynamicReportInfoId(dynamicReportId);
        }

        #endregion

        #region RowDoubleClick
        /// <summary>
        /// Display Field edit popup on double click
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task RowDoubleClick(DataGridRowMouseEventArgs<ReportTableColumnModel> args)
        {
            await Edit(args.Data.Id);
        } 
        #endregion

        #region Dynamic Report
        /// <summary>
        /// Display dynamic report join Popup
        /// </summary>
        /// <returns></returns>
        private async Task ManageDynamicReportJoin()
        {
            await Task.Yield();
            isReportJoinPopupVisible = true;
        }

        /// <summary>
        /// Display dynamic report edit popup 
        /// </summary>
        /// <returns></returns>
        private async Task EditDynamicReportInfo()
        {
            await Task.Yield();
            //dynamicReportInfo = await _dynamicReportInfoService.GetByObjectId(objectId);
            ShowAddEditReportInoPopup();
        }

        /// <summary>
        /// Insert or update dynamic report info into database
        /// </summary>
        /// <returns></returns>
        private async Task InsertUpdateDynamicReportInfo()
        {
            isSubmitting = true;

            if (dynamicReportInfo.Id > 0)
            {
                dynamicReportInfo.UpdatedByContactId = loginUser.ContactId;
                var updateResponse = await _dynamicReportInfoService.Update(dynamicReportInfo);

                if (!updateResponse.IsSuccess)
                {
                    ModalNotification.ShowMessage("Error", updateResponse.Message);
                    ShowQuickNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error!", Detail = updateResponse.Message, Duration = 8000 });

                    isSubmitting = false;
                    return;
                }
            }

            isSubmitting = false;

            isReportInfoPopupVisible = false;
        }

        #endregion

        #region Field Add Edit

        /// <summary>
        /// Show add new field popup
        /// </summary>
        void AddNew()
        {
            reportTableColumn = new ReportTableColumnModel
            {
                DynamicReportInfoId = dynamicReportInfo.Id,
                IsVisible = true,
                Filterable = true,
                Sortable = true,
                Width = "120",
                DisplayOrder = 1,
                TextColorRGB = "rgb(0, 0, 0)",
                BackgroundColorRGB = "rgb(255, 255, 255)"
            };

            //Set display order
            var lastColumn = dynamicReportFields.OrderByDescending(f => f.DisplayOrder).FirstOrDefault();

            if (lastColumn != null)
            {
                reportTableColumn.DisplayOrder = lastColumn.DisplayOrder + 1;

                //Set colum width
                reportTableColumn.Width = lastColumn.Width;
            }

            isSubmitting = false;
            isPopupVisible = true;
        }

        /// <summary>
        /// Edit Dynamic Report Field
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task Edit(int id)
        {
            reportTableColumn = await _dynamicReportInfoService.GetTableColumnByTableColumnId(id);

            //Set Text Color
            if (string.IsNullOrEmpty(reportTableColumn.TextColor))
            {
                reportTableColumn.TextColorRGB = "rgb(0, 0, 0)";
            }
            else
            {
                reportTableColumn.TextColorRGB = StringHelper.HexToRGB(reportTableColumn.TextColor);
            }

            //Set Text Color
            if (string.IsNullOrEmpty(reportTableColumn.BackgroundColor))
            {
                reportTableColumn.BackgroundColorRGB = "rgb(255, 255, 255)";
            }
            else
            {
                reportTableColumn.BackgroundColorRGB = StringHelper.HexToRGB(reportTableColumn.BackgroundColor);
            }

            ShowAddEditPopup();
        }

        /// <summary>
        /// Add or update report column into database
        /// </summary>
        /// <returns></returns>
        private async Task InsertUpdateTableColumn()
        {
            isSubmitting = true;

            if (reportTableColumn.Id == 0)
            {
                reportTableColumn.DynamicReportInfoId = dynamicReportInfo.Id;
                reportTableColumn.CreatedByContactId = loginUser.ContactId;
                reportTableColumn.CreatedDate = DateTime.Now;

                var addResponse = await _dynamicReportInfoService.InsertTableColumn(reportTableColumn);

                if (addResponse.IsSuccess)
                {
                    ShowQuickNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Succes!", Detail = addResponse.Message, Duration = 8000 });
                }
                else
                {
                    ShowQuickNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error!", Detail = addResponse.Message, Duration = 8000 });
                    isSubmitting = false;
                    return;
                }

                reportTableColumn = new ReportTableColumnModel();
                await LoadDynamicReportFields(dynamicReportInfo.Id);
                AddNew();
            }
            else
            {
                reportTableColumn.CreatedByContactId = loginUser.ContactId;
                reportTableColumn.CreatedDate = DateTime.Now;
                var updateResponse = await _dynamicReportInfoService.UpdateTableColumn(reportTableColumn);

                if (updateResponse.IsSuccess)
                {
                    ShowQuickNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Succes!", Detail = updateResponse.Message, Duration = 8000 });
                }
                else
                {
                    ShowQuickNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Error!", Detail = updateResponse.Message, Duration = 8000 });
                    isSubmitting = false;
                    return;
                }

                reportTableColumn = new ReportTableColumnModel();
                await LoadDynamicReportFields(dynamicReportInfo.Id);
                CloseAddEditPopup();
            }

            isSubmitting = false;
            
        }

        /// <summary>
        /// Show edit report info popup
        /// </summary>
        void ShowAddEditReportInoPopup()
        {
            isReportInfoPopupVisible = true;
        }

        /// <summary>
        /// Hide Add/Eidt report info popup
        /// </summary>
        void CloseAddEditReportInoPopup()
        {
            isReportInfoPopupVisible = false;
            StateHasChanged();
        }

        /// <summary>
        /// Show Report Field add or edit popup
        /// </summary>
        void ShowAddEditPopup()
        {
            isPopupVisible = true;
        }

        /// <summary>
        /// Hide Report Field add or edit popup
        /// </summary>
        void CloseAddEditPopup()
        {
            isPopupVisible = false;
            StateHasChanged();
        }

        /// <summary>
        /// Hide manage join popup
        /// </summary>
        void CloseAddEditReportJoinPopup()
        {
            isReportJoinPopupVisible = false;
        }

        /// <summary>
        /// Show delete confirmaiton dialog
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        protected void Delete(int id, string name)
        {
            selectedTableColumnId = id;
            var msg = $"Are you sure you want to delete the column \"{name}\"?";
            ModalNotification.ShowConfirmation("Confirm Delete", msg);
        }

        /// <summary>
        /// Delete Column from database after confirmaiton
        /// </summary>
        /// <param name="deleteConfirmed"></param>
        /// <returns></returns>
        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                var deleteResponse = await _dynamicReportInfoService.DeleteTableColumn(dynamicReportInfo.Id, selectedTableColumnId);

                if (deleteResponse.IsSuccess)
                {
                    await LoadDynamicReportFields(dynamicReportInfo.Id);
                    reportTableColumn = new ReportTableColumnModel();
                    CloseAddEditPopup();
                }
                else
                {
                    ModalNotification.ShowMessage("Error", deleteResponse.Message);
                }
            }

            isSubmitting = false;
        }

        /// <summary>
        /// Back to report list page
        /// </summary>
        protected void BackToReports()
        {
            NavigationManager.NavigateTo($"/report/dynamicreportsconfiguration", true);
        }

        /// <summary>
        /// Show quick notificaiton
        /// </summary>
        /// <param name="message"></param>
        void ShowQuickNotification(NotificationMessage message)
        {
            _notificationService.Notify(message);
        }
        #endregion        
    }
}
