using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Models.Security
{
	public class MenuModel
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Name is required.")]
		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string Name { get; set; }
		public int? ParentId { get; set; }
		[StringLength(50, ErrorMessage = "Icon is too long.")]
		public string Icon { get; set; }
		public bool IsLeftMenu { get; set; }
		public bool IsTopMenu { get; set; }
		public bool IsExternalMenu { get; set; }
		public string MenuUrl { get; set; }
		public int? Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int? CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }
		public decimal DisplayOrder { get; set; }

		public IEnumerable<string> SelectedMenuPermissons { get; set; }
	}

	public class MenuListModel
	{
		public int Id { get; set; }		
		public string Name { get; set; }
		public string ParentMenuName { get; set; }		
		public string Icon { get; set; }		
		public string MenuUrl { get; set; }
		public int? Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public string ObjectId { get; set; }
		public decimal DisplayOrder { get; set; }
		public string PermissionNames { get; set; }
	}

	public class SideMenuListModel
	{
		public int Id { get; set; }
		public string Name { get; set; }		
		public string Icon { get; set; }
		public string MenuUrl { get; set; }
		public int? ParentId { get; set; }
		public decimal DisplayOrder { get; set; }
	}
}
