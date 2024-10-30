using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Data
{
	public class Company
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public int Status { get; set; }
		public int? CreatedByContactId { get; set; }
		public DateTime CreatedDateUtc { get; set; }
		public DateTime? ChangedDateUtc { get; set; }
		public string? FileRootFolderPath { get; set; }
	}
}
