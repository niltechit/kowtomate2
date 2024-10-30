
namespace CutOutWiz.Services.Models.IBRModels
{
    public class IbrDefaultSettingsApiResponse
    {
        public string model_base_url { get; set; }
        public Guid service_type_id { get; set; }
        public Guid menu_id { get; set; }
        public Guid subscription_plan_type_id { get; set; }
        public int processable_image_qty_limit { get; set; }
        public int downloadable_image_qty_limit { get; set; }

    }
}
