using CutOutWiz.Data.Entities.Ftp;

namespace CutOutWiz.Data.DTOs.Ftp
{
	public class ClientExternalOrderFTPSetupDto : ClientOrderFtpEntity  //Database Table name: Client_ExternalOrderFTPSetup
	{
		//Common Fields
		public DateTime CreatedDate { get; set; }
		public int? CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }
	}
}
