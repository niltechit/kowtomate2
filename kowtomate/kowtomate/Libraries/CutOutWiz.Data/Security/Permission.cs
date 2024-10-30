using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Data.Security
{
	public class Permission
	{
		public short Id { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Value is required.")]
		[StringLength(100, ErrorMessage = "Value is too long.")]
		public string Value { get; set; }
		public byte Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }

		//For 
		public IEnumerable<string> SelectedModules { get; set; } = new List<string>();
		public IEnumerable<string> SelectedCompanyTypes { get; set; }
	}

	public class PermissionListModel
	{
		public short Id { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
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
