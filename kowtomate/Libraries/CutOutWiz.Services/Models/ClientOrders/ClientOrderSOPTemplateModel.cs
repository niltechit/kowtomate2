using CutOutWiz.Services.Models.SOP;

namespace CutOutWiz.Services.Models.ClientOrders
{
    public class ClientOrderSOPTemplateModel
    {
        public int Id { get; set; }
        public int Order_ClientOrder_Id { get; set; }
        public int SOP_Template_Id { get; set; }
        public int OrderSOP_Template_Id { get; set; }
        public string SOP_Template_Name { get; set; }
        public int SortOrder { get; set; }
        public virtual List<SOPTemplateModel> TemplateList { get; set; }
    }
}
