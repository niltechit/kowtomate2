using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Data.SOP
{
	public class SOPStandardService
	{
		public short Id { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(500, ErrorMessage = "Name is too long.")]
		public string Name { get; set; }
		public decimal? SortOrder { get; set; }
		public byte Status { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }
	}
}
