using CutOutWiz.Core;
using CutOutWiz.Services.Models.DynamicReports;
using KowToMateAdmin.Models.Security;
using KowToMateAdmin.Pages.Shared;
using DocumentFormat.OpenXml.Office2010.Excel;
using Radzen;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Core.Utilities;

namespace KowToMateAdmin.Pages.DynamicReports.DynamicReportSetup
{
    public partial class DynamicReportInfoList
    {
        protected ModalNotification ModalNotification { get; set; }

        int selectedId = 0;
        string msgCloneConfirm = "";
        bool showCloneConfirmPopup;
        bool isSubmitting;
        private List<DynamicReportInfoModel> dynamicReportInfos;
        IEnumerable<int> pageSizeOptions = new int[] { 10, 20, 50, 100, 500 };
        bool isLoading = true;
        private DynamicReportInfoModel dynamicReportInfo = null;
        bool isPopupVisible = false;
        private LoginUserInfoViewModel loginUser = null;
        private string selectedObjectId = "";
        List<CustomEnumTypes> statuses = new List<CustomEnumTypes>();
        List<CustomEnumTypes> reportTypes = new List<CustomEnumTypes>();
        private byte? filterStatus;
        private byte? filterReportType;
        public void closeCloneConfirmPopup() => showCloneConfirmPopup = false;

        /// <summary>
        /// Initialized before page render
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            loginUser = _workContext.LoginUserInfo;
        }

        /// <summary>
        /// Load data after page render
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                isLoading = true;
                loginUser = _workContext.LoginUserInfo;
                await LoadDynamicReportInfos();

                foreach (DynamicReportType item in Enum.GetValues(typeof(DynamicReportType)))
                {
                    reportTypes.Add(new CustomEnumTypes { EnumName = item.ToString(), EnumValue = Convert.ToByte((int)item) });
                }

                foreach (GeneralStatus item in Enum.GetValues(typeof(GeneralStatus)))
                {
                    statuses.Add(new CustomEnumTypes { EnumName = item.ToString(), EnumValue = Convert.ToByte((int)item) });
                }

                isLoading = false;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Load dynmaicreport info
        /// </summary>
        /// <returns></returns>
        private async Task LoadDynamicReportInfos()
        {
            dynamicReportInfos = await _dynamicReportInfoService.GetAll();
        }

        /// <summary>
        /// Show Dynamic report info add/edit popup
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task RowDoubleClick(DataGridRowMouseEventArgs<DynamicReportInfoModel> args)
        {
            await Edit(args.Data.ObjectId);
        }

        /// <summary>
        /// Insert or Update Dynamic Report Info
        /// </summary>
        /// <returns></returns>
        private async Task InsertUpdateDynamicReportInfo()
        {
            isSubmitting = true;

            if (dynamicReportInfo.Id == 0)
            {
                dynamicReportInfo.CreatedByContactId = loginUser.ContactId;
                dynamicReportInfo.ObjectId = Guid.NewGuid().ToString();

                var addResponse = await _dynamicReportInfoService.Insert(dynamicReportInfo);

                if (!addResponse.IsSuccess)
                {
                    ModalNotification.ShowMessage("Error", addResponse.Message);
                    isSubmitting = false;
                    return;
                }
            }
            else
            {
                dynamicReportInfo.UpdatedByContactId = loginUser.ContactId;
                var updateResponse = await _dynamicReportInfoService.Update(dynamicReportInfo);

                if (!updateResponse.IsSuccess)
                {
                    ModalNotification.ShowMessage("Error", updateResponse.Message);
                    isSubmitting = false;
                    return;
                }
            }

            dynamicReportInfo = new DynamicReportInfoModel();
            isSubmitting = false;

            await LoadDynamicReportInfos();
            CloseAddEditPopup();
        }

        /// <summary>
        /// Return Tags by Text
        /// </summary>
        /// <returns></returns>
        string StatusText()
        {
            if (dynamicReportInfo.Status == (int)GeneralStatus.Active)
            {
                return "<span class='badge bg-success'>Active</span>";
            }

            return "<span class='badge bg-info text-dark'>Inactive</span>";
        }

        /// <summary>
        /// Add new Dynamic report event
        /// </summary>
        void AddNew()
        {
            dynamicReportInfo = new DynamicReportInfoModel
            {
                PageSize = 20,
                AllowPaging = true,
                AllowHtmlPreview = true,
                AllowSorting = true,
                SqlType = (int)DynamicReportSqlType.Procedure,
                Status = (int)GeneralStatus.Inactive
            };

            isSubmitting = false;
            ShowAddEditPopup();
        }

        /// <summary>
        /// Show copy confirmatoin dialong
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected async Task Copy(int id, string name)
        {
            await Task.Yield();

            selectedId = id;
            msgCloneConfirm = $"Are you sure you want to Copy the Report Setup \"{name}\"?";
            showCloneConfirmPopup = true;
            StateHasChanged();
        }

        /// <summary>
        /// Show Dynamic report info Edit Popup
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        private async Task Edit(string objectId)
        {
            dynamicReportInfo = await _dynamicReportInfoService.GetByObjectId(objectId);
            ShowAddEditPopup();
        }

        /// <summary>
        /// Show Dynamic report add/edit poup
        /// </summary>
        void ShowAddEditPopup()
        {
            isPopupVisible = true;
            StateHasChanged();
        }

        /// <summary>
        /// Hide Dynamic report add/edit poup
        /// </summary>
        void CloseAddEditPopup()
        {
            isPopupVisible = false;
            StateHasChanged();
        }

        /// <summary>
        /// Show dynamic report delete confirmation popup
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="name"></param>
        protected void Delete(string objectId, string name)
        {
            selectedObjectId = objectId;
            var msg = $"Are you sure you want to delete the Report Setup \"{name}\"?";
            ModalNotification.ShowConfirmation("Confirm Delete", msg);
        }

        /// <summary>
        /// Delete dynamic report from database if confirmed
        /// </summary>
        /// <param name="deleteConfirmed"></param>
        /// <returns></returns>
        protected async Task ConfirmDelete_Click(bool deleteConfirmed)
        {
            if (deleteConfirmed)
            {
                var deleteResponse = await _dynamicReportInfoService.Delete(selectedObjectId);

                if (deleteResponse.IsSuccess)
                {
                    await LoadDynamicReportInfos();
                    dynamicReportInfo = new DynamicReportInfoModel();
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
        /// Clone Dynamic report and columns
        /// </summary>
        /// <returns></returns>
        protected async Task ConfirmCloneReport_Click()
        {
            isSubmitting = true;
            var model = new DynamicReportInfoModel { Id = selectedId };
            var response = await _dynamicReportInfoService.CloneDynamicReportInfo(model);

            if (response.IsSuccess)
            {
                selectedId = 0;
                await LoadDynamicReportInfos();
                dynamicReportInfo = new DynamicReportInfoModel();
                showCloneConfirmPopup = false;

                GoToManageReportColumns(model.Id);
            }
            else
            {
                ModalNotification.ShowMessage("Error", response.Message);
            }

            isSubmitting = false;
            StateHasChanged();
        }

        /// <summary>
        /// Redirect to report setup page
        /// </summary>
        /// <param name="id"></param>
        protected void GoToManageReportColumns(int id)
        {
            NavigationManager.NavigateTo($"/report/dynamicReportSetup/{id}", true);
        }

    }
}
