namespace CutOutWiz.Core.Models.Accounting
{
	public class OverheadCostingListViewModel
	{
		public int Id { get; set; }
		public string Month { get; set; }
		public int Year { get; set; }
		public decimal Amount { get; set; }
		public string CreatedByUsername { get; set; }
		public DateTime? CreatedDate { get; set; }
		public string UpdatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
	}
}
