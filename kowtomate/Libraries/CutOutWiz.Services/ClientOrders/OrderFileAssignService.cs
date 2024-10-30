using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.OrderAssignedImageEditors;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.ClientOrders
{
	public class OrderFileAssignService : IOrderFileAssignService
    {
		private readonly ISqlDataAccess _db;

		public OrderFileAssignService(ISqlDataAccess db)
		{
			_db = db;
		}


        public async Task<OrderAssignedImageEditorModel> GetById(int imageId)
		{
            var result = await _db.LoadDataUsingProcedure<OrderAssignedImageEditorModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderAssignedItem_InfoByContactId", new { ImageId = imageId });
            return result.FirstOrDefault();
        }
	}
}
