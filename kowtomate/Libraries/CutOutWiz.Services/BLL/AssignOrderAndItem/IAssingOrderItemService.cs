using CutOutWiz.Services.Models.ClientOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.BLL.AssignOrderAndItem
{
	public interface IAssingOrderItemService
	{
		Task AssignOrderItemToEditor(List<ClientOrderItemModel> clientOrderItems, long assingContactId);
	}
}
