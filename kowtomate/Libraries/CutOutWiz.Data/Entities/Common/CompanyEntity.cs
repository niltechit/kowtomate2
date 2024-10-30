
namespace CutOutWiz.Data.Models.Common
{
	public class CompanyEntity
    {
		public int Id { get; set; }

		public string Name { get; set; }
		public string Code { get; set; }
		public byte? CompanyType { get; set; }
		public string Telephone { get; set; }
		public string Email { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zipcode { get; set; }
		public string Country { get; set; }
		public string FileServer { get; set; }
		public short Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int? CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }
		public short FileServerId { get; set; }

		// Contact 
		//public virtual ContactModel Contact { get; set; }

        public string UserName { get; set; }
		public string Password { get; set; }
		
		public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
		public string LastName { get; set; }
		public int TeamId { get; set; }
		public int UserId { get; set; }
		public int ContactId { get; set; }
		public int? DeliveryDeadlineInMinute { get; set; }
	}
}
