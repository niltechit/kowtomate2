
using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.UI
{
    public class GridViewFilterModel
	{
		public int Id { get; set; }
		public int GridViewSetupId { get; set; }
		public int ContactId { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		public string Name { get; set; } 
		public string FilterJson { get; set; } 
		public bool IsDefault { get; set; }
		public bool IsPublic { get; set; }
		public string LogicalOperator { get; set; }
		public string SortColumn { get; set; }
		public string SortOrder { get; set; }
		public DateTime UpdatedDate { get; set; } 
		public bool IsCreateNewTemplate { get; set; }

		public string DisplayName { get; set; }
	}
}
