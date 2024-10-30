using CutOutWiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.AutomationAppServices.UploadFromQcPcAutomation
{
    public interface IUploadFromQcPcService
    {
        Task<Response<bool>> UploadImageFromQcPc(int consoleAppId);
    }
}
