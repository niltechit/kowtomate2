
namespace CutOutWiz.Data.Entities.Accounting
{
	public class OverheadCostEntity
	{
		public int Id { get; set; }
		public int Month { get; set; }
		public int Year { get; set; }
		public decimal Amount { get; set; }
		public int? CreatedByContactId { get; set; }
		public DateTime? CreatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
	}
}
