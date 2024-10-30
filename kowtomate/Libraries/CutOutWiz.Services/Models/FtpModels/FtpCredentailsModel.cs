using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.Models.FtpModels
{
    public class FtpCredentailsModel
    {
        public long Id { get; set; } 

		public string RootFolder { get; set; }
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SubFolder { get; set; }
        public int? Port { get; set; }
        public int FtpEncryptionMode { get; set; } = (int)FtpEncryptionModeType.Auto;

        // Method to create log description
        public string GetLogDescription()
        {
            return $@"FTP Details:
                  Id: {Id}
                  Root Folder: {RootFolder}
                  Host: {Host}
                  User Name: {UserName}
                  Sub Folder: {SubFolder}
                  Port: {(Port.HasValue ? Port.ToString() : "")}.";
        }
    }
}
