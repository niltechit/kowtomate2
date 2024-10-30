
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CutOutWiz.Services.Models.SOP
{
	public class SOPTemplateModel
	{
		public int Id { get; set; }


		[Required(ErrorMessage = "Company is required.")]
		public int CompanyId { get; set; }

		//[Required(ErrorMessage = "File Server is required.")]
		public short? FileServerId { get; set; }

		[Required(ErrorMessage = "Name is required.")]
		[StringLength(100, ErrorMessage = "Name is too long.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Version is required.")]
		public decimal Version { get; set; }
		public int? ParentTemplateId { get; set; }
        [Required(ErrorMessage = "Instraction is required.")]
        public string Instruction { get; set; }

		[DisplayName("Unit Price")]
		public decimal? UnitPrice { get; set; }
		public byte? Status { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public int? InstructionModifiedByContactId { get; set; }
		public string ObjectId { get; set; }

        public IReadOnlyList<IBrowserFile> SopAttachment { get; set; } 

		//public List<string> SOPStandardServiceList { get; set; } = new List<string>();
        public List<SOPTemplateFile> SopTemplateFileList { get; set; } = new List<SOPTemplateFile>();

		public List<SOPTemplateServiceModel> SopTemplateServiceList { get; set; } = new List<SOPTemplateServiceModel>();
		public List<SOPStandardServiceModel> SopStandardServiceList { get; set; } = new List<SOPStandardServiceModel>();
		//public string FileType { get; set; }
		//public string FileObjectId { get; set; }
		public List<int> selectedTemplateList = new List<int>();
		public int? ParentSopServiceId { get; set; }
		public int OrderTemplateId { get; set; }
	}
}
