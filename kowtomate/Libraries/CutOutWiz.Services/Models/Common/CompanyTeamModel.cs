
namespace CutOutWiz.Services.Models.Common
{
    public class CompanyTeamModel
    {
		public int Id { get; set; }
		public int CompanyId { get; set; }
		public int TeamId { get; set; }
		public int Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int? CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }
	}
}
