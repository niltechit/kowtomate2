using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Security;
using Microsoft.AspNetCore.Components.Forms;

namespace CutOutWiz.Services.Models.FileUpload
{
	public class FileUploadModel
	{
		public string FtpUrl { get; set; }
		public string fileName { get; set; }
		public string FileName { get; set; }
		public string userName { get; set; }
		public string password { get; set; }
		public string UploadDirectory { get; set; }
		public string RootDirectory { get; set; }
		public string DownloadDirectory { get; set; }
		public string CompanyName { get; set; }
		public string FolderName { get; set; }
		public string OrderNumber { get; set; }
		public string BaseFolder { get; set; }
		public string ContactName { get; set; }
		public string DownloadFolderName { get; set; }
		public string ReturnPath { get; set; }
		public DateTime Date { get; set; }
		public IBrowserFile file { get; set; }
		public byte[] ImageByte { get; set; }
		public virtual ClientOrderListModel ClientOrder { get; set; }
		public List<ClientOrderItemModel> clientOrderItems = new List<ClientOrderItemModel>();
		public virtual ContactModel Contact { get; set; }
		public string SubFolder { get; set; }
        public string LocalPath { get; set; }
        public string RemotePath { get; set; }

    }
}
