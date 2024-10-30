using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.DynamicReports
{
    public class DynamicReportInfoModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Report Name is required.")]
        [StringLength(100, ErrorMessage = "Report Name is too long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Report Code is required.")]
        [StringLength(100, ErrorMessage = "Report Code is too long.")]
        public string ReportCode { get; set; }

        [StringLength(500, ErrorMessage = "Description is too long.")]
        public string Description { get; set; }
        public byte? SqlType { get; set; }
        public string SqlScript { get; set; }
        public byte? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByContactId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedByContactId { get; set; }
        public string ObjectId { get; set; }
        public bool AllowCompanyFilter { get; set; }
        public bool AllowStartDateFilter { get; set; }
        public bool AllowEndDateFilter { get; set; }
        public bool AllowDateOnlyFilter { get; set; }
        public bool AllowFiltering { get; set; }
        public bool AllowPaging { get; set; }
        public bool AllowSorting { get; set; }
        public bool AllowHtmlPreview { get; set; }
        public string DefaultSortColumn { get; set; }
        public string DefaultSortOrder { get; set; }
        public bool AllowVirtualization { get; set; }
        public int PageSize { get; set; }
        public string PermissionObjectId { get; set; }
        public byte? ReportType { get; set; }
        public string WhereClause { get; set; }
    }

    public class DynamicReportInfoSlim
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
