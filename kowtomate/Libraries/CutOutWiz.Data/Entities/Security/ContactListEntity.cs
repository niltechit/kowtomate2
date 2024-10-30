
namespace CutOutWiz.Data.Security
{
	public class ContactListEntity
	{
		public int Id { get; set; }
		public string CompanyName { get; set; }
		public int CompanyId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int UserId { get; set; }
		public string UserName { get; set; }
		public string UserObjectId { get; set; }
		public string DesignationName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public int Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int? CreatedByContactId { get; set; }
		public string ObjectId { get; set; }

		public string EmployeeId { get; set; }
		public int TeamId { get; set; }
	}
}
