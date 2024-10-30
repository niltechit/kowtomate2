using CutOutWiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.AutomationAppServices.OrderPlaceAutomation
{
    public interface IOrderPlaceService
    {
        Task<Response<bool>> PlaceNewOrderFromClientStorage(int consoleAppId, bool isInternalStorageOrderPlace);
    }
}
