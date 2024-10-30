using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.OrderAssignedImageEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.ClientOrders
{
	public interface IOrderFileAssignService
	{
        Task<OrderAssignedImageEditorModel> GetById(int contactId);
    }
}
