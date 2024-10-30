using CutOutWiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.AutomationAppServices.MakeOrderPlacingToPlaced
{
    public interface IOrderPlacingToPlacedService
    {
        Task<Response<bool>> MakeOrderStatusOrderPlacingToOrderPlaced(int consoleAppId);
    }
}
