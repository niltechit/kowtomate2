
using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.Security
{
	public class PermissionModel
	{
		public short Id { get; set; }

		[Required(ErrorMessage = "Display Name is required.")]
		[StringLength(200, ErrorMessage = "Name is too long.")]
		public string DisplayName { get; set; }

		[Required(ErrorMessage = "Permission Value is required.")]
		[StringLength(100, ErrorMessage = "Value is too long.")]
		public string PermissionValue { get; set; }  //Permission Note
		public byte Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }

		public decimal DisplayOrder { get; set; }
		//For 
		public IEnumerable<string> SelectedModules { get; set; } = new List<string>();
		public IEnumerable<string> SelectedCompanyTypes { get; set; }
	}

	public class PermissionListModel
	{
		public short Id { get; set; }
		public string DisplayName { get; set; }
		public string PermissionValue { get; set; }
		public decimal DisplayOrder { get; set; }
		public byte Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }
		public string ModuleNames { get; set; }
		public string CompanyTypes { get; set; }
		public string MenuNames { get; set; }
	}
}
