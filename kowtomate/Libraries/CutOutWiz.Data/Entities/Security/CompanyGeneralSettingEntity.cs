namespace CutOutWiz.Data.Entities.Security
{
	public class CompanyGeneralSettingEntity
	{
		public int Id { get; set; }
		public bool AutoAssignOrderToTeam { get; set; }
		public bool AutoDistributeToEditor { get; set; }
		public bool AutoQcPass { get; set; }
		public bool AutoOperationPass { get; set; }
		public bool AllowPartialDelivery { get; set; }
		public bool EnableFtpOrderPlacement { get; set; }
		public bool EnableOrderDeliveryToFtp { get; set; }
		public bool AllowSingleOrderFromFTP { get; set; }
		public int CompanyId { get; set; }
		public int AllowMaxNumOfItemsPerOrder { get; set; }
		public bool EnableSingleOrderPlacement { get; set; }
		public bool IsIbrProcessedEnabled { get; set; }
		public bool AllowAutoUploadFromEditorPc { get; set; }
		public bool AllowAutoUploadFromQcPc { get; set; }
		public bool AllowAutoDownloadToEditorPc { get; set; }
		public bool AllowClientWiseImageProcessing { get; set; }
		public bool AllowNotifyOpsOnImageArrivalFTP { get; set; }
		public bool CheckUploadCompletedFlagOnFile { get; set; }
		public bool CheckEmailForUploadCompletedConfirmation { get; set; }
		public short FtpOrderPlacedAppNo { get; set; }

		public bool IsBatchRootFolderNameAddWithOrder { get; set; }
		public bool isFtpFolderPreviousStructureWiseStayInFtp { get; set; }
		public bool AllowExtraFile { get; set; }
		public bool IsSameNameImageExistOnSameFolder { get; set; }
		public short DeliveryType { get; set; }
		public short OrderPlaceBatchMoveType { get; set; }
		public bool RemoveFacilityNameFromOutputRootFolderPath { get; set; }

		public bool CheckUploadCompletedFlagOnBatchName { get; set; }
		public string CompletedFlagKeyNameOnBatch { get; set; }
		public bool IsOrderCreatedThenFileMove { get; set; }
		public bool IsFtpIdleTimeChecking { get; set; }
		public int FtpIdleTime { get; set; }
		public string FtpFileMovedPathAfterOrderCreated { get; set; }
		public bool IsSendClientHotkey { get; set; }
		public string HotkeyFlagFileName { get; set; }
		public bool IsZipParentFolderSave { get; set; }
		public string HotKeyFileName { get; set; }
		public bool AllowSingleOrderForRootAllFolderAndFiles { get; set; }

		public bool IsOrderPlacedEmailSentToCompany { get; set; }
		public bool IsOrderPlacedEmailSentToCompanyAllUsers { get; set; }
	}
}
