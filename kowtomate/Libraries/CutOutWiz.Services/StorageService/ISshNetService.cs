using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Services.Models.Security;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.StorageService
{
	public interface ISshNetService
	{
		Task<SftpClient> CreateSshNetConnector(bool isOrderPlace, ClientExternalOrderFTPSetupModel clientOrderSFtp);
        Task RecursiveListFiles(SftpClient client, string remotePath, List<string> filePaths);
		Task<int> RecursiveListFilesMove(SftpClient client, string remotePath, string prefix);
		Task RecursiveCreateDirectories(SftpClient client, string remoteDirectoryPath);
		Task<bool> UploadFileWithRateLimit(SftpClient client, string localPath, string remoteDirectory, int rateLimit);
		Task RecursiveDeleteDiretories(SftpClient client, string directory);
        Task SingleFileMove(ClientExternalOrderFTPSetupModel sourceFtp, string moveFileDestination, CompanyGeneralSettingModel? companyGeneralSetting = null);

    }
}
