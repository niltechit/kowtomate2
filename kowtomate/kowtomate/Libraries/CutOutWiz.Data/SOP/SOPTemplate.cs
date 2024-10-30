using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Data.SOP
{
	public class SOPTemplate
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Company is required.")]
		public int CompanyId { get; set; }

		[Required(ErrorMessage = "File Server is required.")]
		public short? FileServerId { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Version is required.")]
		public decimal Version { get; set; }
		public int? ParentTemplateId { get; set; }
		public string Instruction { get; set; }
		public decimal? UnitPrice { get; set; }
		public byte? Status { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public int? InstructionModifiedByContactId { get; set; }
		public string ObjectId { get; set; }

	}
}
