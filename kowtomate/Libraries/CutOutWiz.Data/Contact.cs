using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Data
{
	public class Contact
	{
		public int Id { get; set; }
		public int? CompanyId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string ImageUrl { get; set; }
		public string ContactGuid { get; set; }
		public DateTime CreatedDateUtc { get; set; }
		public DateTime? ChangedDateUtc { get; set; }

	}
}
