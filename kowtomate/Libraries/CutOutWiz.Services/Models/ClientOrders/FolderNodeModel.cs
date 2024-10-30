namespace CutOutWiz.Services.Models.ClientOrders
{
	public class FolderNodeModel
	{
		public long OrderItemId { get; set; }
		public bool IsFolder { get; set; }
		public string FolderName { get; set; }

		public string FullPath { get; set; }
		public string PartialPath { get; set; }
		public string Prefix { get; set; }
		public bool IsSelected { get; set; }
		public byte? Status { get; set; }
		public byte? ExternalStatus { get; set; }
		public string InternalFileInputPath { get; set; }
		
		public string EditorName { get; set; }
		public byte FolderMinStatus { get; set; }
        public int FileGroup { get; set; }
        public string ProductionDoneFilePath { get; set; }
        public string InternalFileOutputPath { get; set; }
		public bool IsFileParentFolder { get; set; }
		public string ParentFolderContainFilePath { get; set; }
    }
}
