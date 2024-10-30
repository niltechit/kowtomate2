
namespace CutOutWiz.Services.Models.Security
{
	public class SecuritySwhiechOperationModel
	{
		public int Id { get; set; }
		public long? ClientCompanyId { get; set; }
		public bool IsEnable { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
		public int? CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; } = DateTime.Now;
		public int? UpdatedByContactId { get; set; }
	}
}
