
namespace CutOutWiz.Core.Management
{
    public class ManageTeamMemberChangeLogModel
    {
        public int? TeamId { get; set; }
        public int? MemberContactId { get; set; }
        public int? AssignByContactId { get; set; }
        public DateTime? AssignTime { get; set; }
        public int? CancelByContactId { get; set; }
        public DateTime? CancelTime { get; set; }
        public string AssignNote { get; set; }
        public string CancelNote { get; set; }
        public bool? IsSupportingMember { get; set; }
        public int ManagementTeamMemberId { get;set; }
        public int? SupportFromTeamId { get; set; }
        public bool? IsCancel { get;set;}
    }
}
