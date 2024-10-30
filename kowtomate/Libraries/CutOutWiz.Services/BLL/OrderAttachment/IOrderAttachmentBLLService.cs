using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.FtpModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.BLL.OrderAttachment
{
	public interface IOrderAttachmentBLLService
	{
		bool IsTxtOrPdfFile(string filePath);
		Task AddOrderAttachment(List<string> attachmentSourchPath, ClientOrderModel order, CompanyModel company, FtpCredentailsModel sourceFtpCredential, FtpCredentailsModel destinationFtpCredentail, bool isLocalFile=false);

    }
}
