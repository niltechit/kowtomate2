
namespace CutOutWiz.Services.Models.SOP
{
    public class SopPdfGeneratorModel
    {
        public string Name { get; set; }
        public string HeaderForInstruction { get;set; }
        public string HeaderForService { get; set; }
        public string Instruction { get; set; }
        public List<SOPTemplateFile> SopTemplateFileList { get; set; } = new List<SOPTemplateFile>();
        public List<SOPStandardServiceModel> SopStandardServiceList { get; set; } = new List<SOPStandardServiceModel>();
    }
}
