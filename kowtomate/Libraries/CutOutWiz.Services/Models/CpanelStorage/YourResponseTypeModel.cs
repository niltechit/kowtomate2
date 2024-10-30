using ThirdParty.Json.LitJson;

namespace CutOutWiz.Core.Models.CpanelStorage
{
    public class YourResponseTypeModel
    {
        [JsonProperty]
        public Dictionary<string, string> ProjectStorageInfo { get; set; } = new Dictionary<string, string>();

    }

}
