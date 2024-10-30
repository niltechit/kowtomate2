using CutOutWiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.AutomationAppServices.DownloadToEditorAutomation
{
    public interface IDownloadToEditorService
    {
        Task<Response<bool>> RetouchedAiProcessingAndSaveFilePath(int consoleAppId);
        Task<Response<bool>> AutoDownloadDistributedItemToEditorPc(int consoleAppId);
    }
}
