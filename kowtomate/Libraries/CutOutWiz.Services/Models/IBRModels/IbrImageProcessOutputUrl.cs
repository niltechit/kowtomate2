
namespace CutOutWiz.Services.Models.IBRModels
{
    public class IbrImageProcessOutputUrl
    {
        public string order_image_master_id { get; set; }
        public string order_image_detail_id { get; set; }
        public int order_image_detail_sequence_no { get; set; }
        public string compressed_raw_image_public_url { get; set; }
        public string default_compressed_output_public_url { get; set; }
        public string original_output_public_url { get; set; }
        public string png_image_output_url { get; set; }
        public string psd_image_output_public_url { get; set; }
        public bool is_ai_processed { get; set; }
    }

    public class IbrImageProcessResults
    {
        public List<IbrImageProcessOutputUrl> output_urls { get; set; }
    }
}
