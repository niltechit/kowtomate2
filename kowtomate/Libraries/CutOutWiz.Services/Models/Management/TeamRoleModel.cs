
using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Core.Management
{
	public class TeamRoleModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Company is required.")]
		public int CompanyId { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(50, ErrorMessage = "Name is too long.")]
		public string Name { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }
		public byte? Status { get; set; }
	}
}
