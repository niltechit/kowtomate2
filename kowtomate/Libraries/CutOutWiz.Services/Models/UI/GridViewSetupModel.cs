
using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.UI
{
    public class GridViewSetupModel
	{
		public int Id { get; set; }
		public int ContactId { get; set; }

	    [Required(ErrorMessage = "Name is required.")]
		public string Name { get; set; }
		
		public string OrderByColumn { get; set; }
		public string OrderDirection { get; set; }
		public bool IsDefault { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public string ObjectId { get; set; }
		public byte GridViewFor { get; set; }
		public bool IsPublic { get; set; }
        
		public int? DynamicReportInfoId { get; set; }

        //Extra Property
        public string DisplayName { get; set; }
		public bool IsCreateNewTemplate { get; set; }

        public string ViewStateJson { get; set; }

    }
}
