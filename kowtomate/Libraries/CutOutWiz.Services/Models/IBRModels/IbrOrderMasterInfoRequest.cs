
namespace CutOutWiz.Services.Models.IBRModels
{
    public class IbrOrderMasterInfoRequest
    {
        public Guid? service_type_id { get; set; }
        public Guid? menu_id { get; set; }
        public Guid? subscription_plan_type_id { get; set; }
        public int file_upload_from { get; set; }
        public int file_upload_by { get; set; }

    }
}
