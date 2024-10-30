using CutOutWiz.Services.Models.SOP;
using CutOutWiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services.Models.OrderSOP;

namespace CutOutWiz.Services.OrderSOP
{
	public interface IOrderTempleateSOPFileService
	{
        Task<Response<int>> Insert(OrderSOPTemplateFile file);
        Task<List<OrderSOPTemplateFile>> GetOrderSopTemplateFilesByOrderSopTemplateId(int SOPTemplateId);
        Task<OrderSOPTemplateFile> GetById(int fileId);
		Task<OrderSOPTemplateFile> GetByOrderSOPTemplateIdAndFileName(OrderSOPTemplateFile model);

	}
}
