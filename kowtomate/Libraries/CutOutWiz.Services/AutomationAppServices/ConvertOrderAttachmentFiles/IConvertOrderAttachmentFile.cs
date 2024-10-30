using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.AutomationAppServices.ConvertOrderAttachmentFiles
{
    public interface IConvertOrderAttachmentFile
    {
        Task<MemoryStream> ConvertEMLFile(MemoryStream inputStream);
    }
}
