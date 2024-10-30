using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Data
{
	public class Role
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime? ChangedDateUtc { get; set; }
		public string RoleGuid { get; set; }

	}

}
