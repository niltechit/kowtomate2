using CutOutWiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.AutomationAppServices.OrderItemCategorySetByAutomation
{
    public interface ICategorySetService
    {
        Task<int> DetectOrderItemCategory(string filePath, int companyId);
    }
}
