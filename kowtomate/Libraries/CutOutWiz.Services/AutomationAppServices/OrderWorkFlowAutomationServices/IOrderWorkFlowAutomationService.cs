using CutOutWiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.AutomationAppServices.OrderWorkFlowAutomationServices
{
    public interface IOrderWorkFlowAutomationService
    {
        Task<Response<bool>> AutoOperationPass(int consoleAppId);
        Task<Response<bool>> AutoQcPass(int consoleAppId);
        Task<Response<bool>> AutoAssignToTeam();
        Task<Response<bool>> AutoDistributeToEditor();
    }
}
