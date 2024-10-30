
namespace CutOutWiz.Core.OrderTeams
{
    public class OrderTeamModel
    {
        public int Id { get; set; }
        public long OrderId { get; set; }
        public long TeamId { get; set; } 
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsItemAssignToTeam { get; set; } 
    }
}
