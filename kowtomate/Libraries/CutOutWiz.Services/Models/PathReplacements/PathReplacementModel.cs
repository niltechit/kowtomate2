
namespace CutOutWiz.Services.Models.PathReplacements
{
	public class PathReplacementModel
	{
		public short Id { get; set; }
		public int Level { get; set; }
		public int CompanyId { get; set; }
		public string OldText { get; set; }
		public string DateFormat { get; set; }
		public string NewText { get; set; }
		public int Type { get; set; }
		public decimal ExecutionOrder { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsActive { get; set; }
		public DateTime? CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
	}
}
