using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Models.EmailModels
{
    public class CloudStorageUsesLimitationNotifyOps
    {
        public List<string> EmailAddresses { get; set; }
        public string MemoryUses { get; set; }

    }
}
