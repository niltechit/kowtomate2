using CutOutWiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.AutomationAppServices.EmailSendAutomation
{
    public interface IAutoEmailSendService
    {
        Task<Response<bool>> NotifyOpsOnImageArrivalFTP();
    }
}
