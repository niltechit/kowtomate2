using Amazon.S3.Model;
using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.OrderAssignedImageEditors;
using CutOutWiz.Services.OrderTeamServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.BLL.AssignOrderAndItem
{
	public class AssingOrderItemService: IAssingOrderItemService
	{
		private readonly IOrderAssignedImageEditorService _orderAssignedImageEditorService;
		public AssingOrderItemService(IOrderAssignedImageEditorService orderAssignedImageEditorService)
		{
			_orderAssignedImageEditorService = orderAssignedImageEditorService;
		}
		public async Task AssignOrderItemToEditor(List<ClientOrderItemModel> clientOrderItems,long assingContactId)
		{
			List<OrderAssignedImageEditorModel> assignedImages = new List<OrderAssignedImageEditorModel>();
			foreach (var item in clientOrderItems)
			{
				OrderAssignedImageEditorModel supportOrderImage = new OrderAssignedImageEditorModel
				{
					OrderId = item.ClientOrderId,
					AssignByContactId = AutomatedAppConstant.ContactId, 
					AssignContactId =(int)assingContactId,
					Order_ImageId = item.Id,
					ObjectId = Guid.NewGuid().ToString(),
					UpdatedByContactId = AutomatedAppConstant.ContactId, //Dummy,
				};
				assignedImages.Add(supportOrderImage);
			}

			var addResponse = await _orderAssignedImageEditorService.Insert(assignedImages);
		}
	}
}
