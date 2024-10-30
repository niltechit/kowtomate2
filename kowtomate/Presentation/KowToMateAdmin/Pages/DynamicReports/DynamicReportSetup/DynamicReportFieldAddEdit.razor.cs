using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.DynamicReports;
using CutOutWiz.Core.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using static CutOutWiz.Core.Utilities.Enums;

namespace KowToMateAdmin.Pages.DynamicReports.DynamicReportSetup
{
    public partial class DynamicReportFieldAddEdit
    {
        [Parameter]
        public ReportTableColumnModel reportTableColumn { get; set; }
        List<CustomEnumNameValuePairModel> allTableFieldType = new List<CustomEnumNameValuePairModel>();
        List<ReportJoinInfoModel> ReportJoinInfoList = new List<ReportJoinInfoModel>();

        /// <summary>
        /// Load data after page loading
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await Task.Yield();

            if (firstRender)
            {
                await PopulateDropDowns();
                StateHasChanged();
            }
        }

        /// <summary>
        /// Field type change event
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        async Task OnSelectedFieldTypeChange(object value)
        {
            await Task.Yield();

            if (reportTableColumn.Id == 0)
            {
                SetDefaultDisplayFormat(reportTableColumn.FieldTypeEnum);
            }
        }

        /// <summary>
        /// Field name change event to populate other fields
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        async Task OnFieldNameChange(FocusEventArgs e)
        {
            if (reportTableColumn.Id > 0 || string.IsNullOrEmpty(reportTableColumn.FieldName))
            {
                return;
            }

            //Populate FieldWithPrefix
            if (string.IsNullOrEmpty(reportTableColumn.FieldWithPrefix))
            {
                reportTableColumn.FieldWithPrefix = reportTableColumn.FieldName;
            }

            //Check if exist in database
            var originalColumn = await _dynamicReportInfoService.GetTableColumnByTableFieldName(reportTableColumn.FieldName);

            if (originalColumn != null)
            {
                reportTableColumn.DisplayName = originalColumn.DisplayName;
                reportTableColumn.IsVisible = originalColumn.IsVisible;
                reportTableColumn.Filterable = originalColumn.Filterable;
                reportTableColumn.Sortable = originalColumn.Sortable;
                reportTableColumn.TextAlign = originalColumn.TextAlign;
                reportTableColumn.Width = originalColumn.Width;

                if (!string.IsNullOrEmpty(originalColumn.TextColor))
                {
                    reportTableColumn.TextColor = originalColumn.TextColor;
                    reportTableColumn.TextColorRGB = StringHelper.HexToRGB(originalColumn.TextColor);
                }

                reportTableColumn.FieldType = originalColumn.FieldType;
                reportTableColumn.DispalyFormat = originalColumn.DispalyFormat;

                if (!string.IsNullOrEmpty(originalColumn.BackgroundColor))
                {
                    reportTableColumn.BackgroundColor = originalColumn.BackgroundColor;
                    reportTableColumn.BackgroundColorRGB = StringHelper.HexToRGB(originalColumn.BackgroundColor);
                }
               
                reportTableColumn.BackgroundColorRules = originalColumn.BackgroundColorRules;
                reportTableColumn.ShowFooterTotal = originalColumn.ShowFooterTotal;
                reportTableColumn.FooterTotalLabel = originalColumn.FooterTotalLabel;
                reportTableColumn.ShowFooterAverage = originalColumn.ShowFooterAverage;
                reportTableColumn.FooterAverageLabel = originalColumn.FooterAverageLabel;
                reportTableColumn.ApplyInFilter1 = originalColumn.ApplyInFilter1;
                reportTableColumn.ApplyInFilter2 = originalColumn.ApplyInFilter2;
                reportTableColumn.ApplyInFilter3 = originalColumn.ApplyInFilter3;
                StateHasChanged();
                return;
            }

            //Populate Dispaly name 
            if (string.IsNullOrEmpty(reportTableColumn.DisplayName))
            {
                reportTableColumn.DisplayName = StringHelper.SeparateWordsFromString(reportTableColumn.FieldName);
            }

            //Populate Field Type
            if (reportTableColumn.FieldType == 0)
            {
                if (reportTableColumn.FieldName.ToLower().Contains("name") || reportTableColumn.FieldName.ToLower().Contains("description"))
                {
                    reportTableColumn.FieldTypeEnum = TableFieldTypeSm.ShortText;
                }
                else if (reportTableColumn.FieldName.ToLower().Contains("count") || reportTableColumn.FieldName.ToLower().Contains("total"))
                {
                    reportTableColumn.FieldTypeEnum = TableFieldTypeSm.Integer;
                }
                else if (reportTableColumn.FieldName.ToLower().Contains("sum") || reportTableColumn.FieldName.ToLower().Contains("avg")
                    || reportTableColumn.FieldName.ToLower().Contains("average"))
                {
                    reportTableColumn.FieldTypeEnum = TableFieldTypeSm.Decimal;
                }
                else if (reportTableColumn.FieldName.ToLower().Contains("date"))
                {
                    reportTableColumn.FieldTypeEnum = TableFieldTypeSm.Date;
                }
                else
                {
                    reportTableColumn.FieldTypeEnum = TableFieldTypeSm.ShortText;
                }

                SetDefaultDisplayFormat(reportTableColumn.FieldTypeEnum);
            }

            StateHasChanged();
        }

        /// <summary>
        /// Text color change event to set default text color
        /// </summary>
        /// <param name="value"></param>
        void OnTextColorChange(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                reportTableColumn.TextColor = StringHelper.RGBToHex(value);
            }
        }

        /// <summary>
        /// Background color change event to set defalut Backgound color
        /// </summary>
        /// <param name="value"></param>
        void OnBackgroundColorChange(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                reportTableColumn.BackgroundColor = StringHelper.RGBToHex(value);
            }
        }

        /// <summary>
        /// Set defualt display format and alignment by field type
        /// </summary>
        /// <param name="tableFieldTypeSm"></param>
        void SetDefaultDisplayFormat(TableFieldTypeSm tableFieldTypeSm)
        {
            if (tableFieldTypeSm == TableFieldTypeSm.Integer)
            {
                reportTableColumn.DispalyFormat = "N0";
                reportTableColumn.TextAlign = (int)TextAlign.Right;
            }
            else if (tableFieldTypeSm == TableFieldTypeSm.Decimal)
            {
                reportTableColumn.DispalyFormat = "C";
                reportTableColumn.TextAlign = (int)TextAlign.Right;
            }
            else if (tableFieldTypeSm == TableFieldTypeSm.Date)
            {
                reportTableColumn.DispalyFormat = "MM/dd/yyyy";
                reportTableColumn.TextAlign = (int)TextAlign.Center;
            }
            else
            {
                reportTableColumn.DispalyFormat = "";
                reportTableColumn.TextAlign = (int)TextAlign.Left;
            }
        }

        #region Populate Dropdowns
        /// <summary>
        /// Populate Field type dropdown values
        /// </summary>
        /// <returns></returns>
        private async Task PopulateDropDowns()
        {
            ReportJoinInfoList = await _dynamicReportInfoService.GetReportJoinInfoListByDynamicReportInfoId(reportTableColumn.DynamicReportInfoId);

            //ActivityLogTypeEnum
            foreach (TableFieldTypeSm item in Enum.GetValues(typeof(TableFieldTypeSm)))
            {
                allTableFieldType.Add(new CustomEnumNameValuePairModel
                {
                    EnumName = item.ToString(),
                    EnumValue = Convert.ToByte((int)item)
                });
            }
        }

        #endregion
    }
}
