using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Data.Security
{
	public class Contact
	{
		public int Id { get; set; }
		public int CompanyId { get; set; }
		[Required(ErrorMessage = "Name is required.")]
		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public int  DesignationId { get; set; }
		[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email")]
		public string Email { get; set; }
		public string Phone { get; set; }
		public string ProfileImageUrl { get; set; }
		public int Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int? CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }
	}

	public class ContactListModel
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
	}
}
