using System.ComponentModel.DataAnnotations;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.Models.DynamicReports
{
    public class ReportTableColumnModel
    {
        public int Id { get; set; }
        public int DynamicReportInfoId { get; set; }
        [Required(ErrorMessage = "Display Name is required.")]
        [StringLength(255, ErrorMessage = "Display Name is too long.")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage = "Field Name is required.")]
        [StringLength(255, ErrorMessage = "Field Name is too long.")]
        public string FieldName { get; set; }

        [Required(ErrorMessage = "Field with Prefix is required.")]
        [StringLength(1000, ErrorMessage = "Field with Prefix is too long.")]
        public string FieldWithPrefix { get; set; }
        
        public bool IsVisible { get; set; }
        public bool Filterable { get; set; }
        public bool Sortable { get; set; }
        public short TextAlign { get; set; }

        public bool Groupable { get; set; }
        public bool IsDefaultGroup { get; set; }

        public bool ShowGroupTotal { get; set; }

        [Required(ErrorMessage = "Display Order is required.")]
        public int? DisplayOrder { get; set; }

        [Required(ErrorMessage = "Width is required.")] 
        public string Width { get; set; }

        public string TextColor { get; set; }

        [Required(ErrorMessage = "Field Type is required.")]
        public byte FieldType { get; set; }

        public string DispalyFormat { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedByContactId { get; set; }

        public string BackgroundColor { get; set; }
        public string BackgroundColorRules { get; set; }
        public bool ShowFooterTotal { get; set; }
        public string FooterTotalLabel { get; set; }
        public bool ShowFooterAverage { get; set; }
        public string FooterAverageLabel { get; set; }
        public bool ApplyInFilter1 { get; set; }
        public bool ApplyInFilter2 { get; set; }
        public bool ApplyInFilter3 { get; set; }

        public string TextColorRGB { get; set; }
        public string BackgroundColorRGB { get; set; }

        public int? JoinInfoId { get; set; }
        public int? JoinInfo2Id { get; set; }
        public int? JoinInfo3Id { get; set; }
        public int? JoinInfo4Id { get; set; }
        public int? JoinInfo5Id { get; set; }
        
        public short? SortingType { get; set; }

        public TableFieldTypeSm FieldTypeEnum
        {
            get => (TableFieldTypeSm)FieldType;
            set => FieldType = (byte)((int)value);
        }

        public string WidthWithPx
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Width))
                {
                    return "150px";
                }

                return $"{Convert.ToInt32(Width) + 50}px";
            }
        }

        public decimal TotalSum { get; set; }

    }
}
