using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Data.Management
{
	public class TeamMember
	{
		public int Id { get; set; }
		public int ContactId { get; set; }
		public int TeamId { get; set; }
		public int TeamRoleId { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }

	}

	public class TeamMemberListModel
	{
		public int Id { get; set; }
		public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public int TeamId { get; set; }
		public int TeamRoleId { get; set; }
        public string TeamRoleName { get; set; }
        public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }

	}
}
