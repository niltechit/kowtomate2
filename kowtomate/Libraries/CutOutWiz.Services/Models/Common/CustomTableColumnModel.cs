using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.Models.Common
{
	public class CustomTableColumnModel
	{
		public int Id { get; set; }
		public string DisplayName { get; set; }
		public string FieldName { get; set; }
		public bool IsVisible { get; set; }
		public int DisplayOrder { get; set; }
		public double Width { get; set; }
		public short FieldType { get; set; }
		public bool IsEditable { get; set; }
		public bool IsAdminCompanyColumn { get; set; }	
		public bool IsClientCompanyColumn { get; set; }	

		public TableFieldType FieldTypeEnum
		{
			get => (TableFieldType)FieldType;
			set => FieldType = (short)((int)value);
		}
	}
}
