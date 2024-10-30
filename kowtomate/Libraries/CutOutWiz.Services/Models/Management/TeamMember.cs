
using CutOutWiz.Services.Models.Security;
using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Core.Management
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
        public bool IsSupportingMember { get; set; }
        public string TeamRoleName { get; set; }
		public string TeamName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FullName => FirstName + " " + LastName;

		//
		public bool IsPrimaryTeam { get; set; }
		public string EmployeeId { get; set; }
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
		public string TeamName { get; set; }
		public string EmployeeId { get; set; }

		public string FullName => FirstName + " " + LastName;

	}


	public class AssignTeamMemberForSupport
	{
        [Required(ErrorMessage = "Team is required.")]
        public int AssignTeamId { get; set; }
		public string AssignNote { get; set; }

    }
}
