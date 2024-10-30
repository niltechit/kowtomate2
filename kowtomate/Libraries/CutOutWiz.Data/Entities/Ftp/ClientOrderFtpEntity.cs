using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Data.Entities.Ftp
{
	public class ClientOrderFtpEntity
	{
		public long Id { get; set; }
		public long ClientCompanyId { get; set; }
		public string Host { get; set; }
		public int? Port { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public bool IsEnable { get; set; } = true;
		public string OutputRootFolder { get; set; }
		public string InputRootFolder { get; set; }

		public bool SentOutputToSeparateFTP { get; set; }
		public bool IsTemporaryDeliveryUploading { get; set; }
		public bool IsDefault { get; set; } = true;
		public string OutputHost { get; set; }
		public string OutputUsername { get; set; }
		public string OutputPassword { get; set; }
		public int OutputPort { get; set; }
		public string OutputFolderName { get; set; }
		public string TemporaryDeliveryUploadFolder { get; set; }

		public short? InputProtocolType { get; set; }
		public short? OutputProtocolType { get; set; }
		public string InputPassPhrase { get; set; }
		public string OutputPassPhrase { get; set; }

		public string InputProtocolTypePuttyKeyPath { get; set; }

		public string OutputProtocolTypePuttyKeyPath { get; set; }

		public double? DeliveryDeadlineInMinute { get; set; }
		public bool IsInternalFtp { get; set; } = false;

		public int FtpEncryptionMode { get; set; } = (int)FtpEncryptionModeType.Auto;
	}
}
