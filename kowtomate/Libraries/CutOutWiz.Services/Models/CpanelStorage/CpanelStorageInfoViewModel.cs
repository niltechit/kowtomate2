using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdParty.Json.LitJson;

namespace CutOutWiz.Core.Models.CpanelStorage
{
    public class CpanelStorageInfoViewModel
    {
        public string total { get; set; }
        public string used { get; set; }
        public string available { get; set; }
        public string used_percentage { get; set; }
    }
    public class ProjectWiseCpanelStorageInfoViewModel
    {
        public string projectname { get; set; }
        public string used { get; set; }
    }
    public class DeserializeProjectWiseCpanelStorageInfoViewModel
    {
        public string projectname { get; set; }
        public string used { get; set; }
    }
}
