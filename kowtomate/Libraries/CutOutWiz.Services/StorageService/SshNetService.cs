using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.BLL;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CutOutWiz.Core.Utilities.Enums;
using static System.Net.WebRequestMethods;

namespace CutOutWiz.Services.StorageService
{
    public class SshNetService : ISshNetService
    {
        private readonly IActivityAppLogService _activityLogCommonMethodService;

        public SshNetService(IActivityAppLogService activityLogCommonMethodService)
        {
            _activityLogCommonMethodService = activityLogCommonMethodService;
        }

        public async Task<SftpClient> CreateSshNetConnector(bool isOrderPlace,ClientExternalOrderFTPSetupModel clientOrderSFtp)
        {
           // await Task.Yield();

            string appRoot = AppDomain.CurrentDomain.BaseDirectory;
           
            if (isOrderPlace || !clientOrderSFtp.SentOutputToSeparateFTP)
            {
                if (string.IsNullOrEmpty(clientOrderSFtp.InputProtocolTypePuttyKeyPath))
                {
                    var sftpClientForOrderPlace = new SftpClient(clientOrderSFtp.Host, clientOrderSFtp.Port??22, clientOrderSFtp.Username, clientOrderSFtp.InputPassPhrase);
                    sftpClientForOrderPlace.OperationTimeout = TimeSpan.FromMinutes(60);
                    sftpClientForOrderPlace.BufferSize = 1024 * 1024 * 10;

                    return sftpClientForOrderPlace;
                }
                
                // IF putty key exist 
                var privateKeyFilePath = Path.Combine(appRoot, "Keys", clientOrderSFtp.InputProtocolTypePuttyKeyPath);
 
                byte[] privateKeyBytes = System.IO.File.ReadAllBytes(privateKeyFilePath);
                var privateKey = new PrivateKeyFile(new System.IO.MemoryStream(privateKeyBytes), clientOrderSFtp.InputPassPhrase);

                var sftpClient = new SftpClient(clientOrderSFtp.Host, clientOrderSFtp.Port??22, clientOrderSFtp.Username, privateKey);
                sftpClient.OperationTimeout = TimeSpan.FromMinutes(60);
                sftpClient.BufferSize = 1024 * 1024 * 10;

                return sftpClient;
                // Replace these variables with your SFTP server details and private key file path.
            }
            else
            {
                if (string.IsNullOrEmpty(clientOrderSFtp.OutputProtocolTypePuttyKeyPath))
                {
                    var sftpClientForOrderDelivery = new SftpClient(clientOrderSFtp.OutputHost, clientOrderSFtp.OutputPort, clientOrderSFtp.OutputUsername, clientOrderSFtp.OutputPassPhrase);
                    sftpClientForOrderDelivery.OperationTimeout = TimeSpan.FromMinutes(60);
                    sftpClientForOrderDelivery.BufferSize = 1024 * 1024 * 10;

                    return sftpClientForOrderDelivery;
                }

                if (string.IsNullOrEmpty(clientOrderSFtp.OutputPassPhrase))
                {
                    var privateKeyFilePath = Path.Combine(appRoot, "Keys", clientOrderSFtp.OutputProtocolTypePuttyKeyPath);

                    byte[] privateKeyBytes = System.IO.File.ReadAllBytes(privateKeyFilePath);
                    var privateKey = new PrivateKeyFile(new System.IO.MemoryStream(privateKeyBytes));

                    var sftpClient = new SftpClient(clientOrderSFtp.OutputHost, clientOrderSFtp.OutputPort, clientOrderSFtp.OutputUsername, privateKey);
                    sftpClient.OperationTimeout = TimeSpan.FromMinutes(60);
                    sftpClient.BufferSize = 1024 * 1024 * 10;
                    return sftpClient;
                }
                else
                {

                    // If putty key exist
                    var privateKeyFilePath = Path.Combine(appRoot, "Keys", clientOrderSFtp.OutputProtocolTypePuttyKeyPath);

                    byte[] privateKeyBytes = System.IO.File.ReadAllBytes(privateKeyFilePath);
                    var privateKey = new PrivateKeyFile(new System.IO.MemoryStream(privateKeyBytes), clientOrderSFtp.OutputPassPhrase);

                    var sftpClient = new SftpClient(clientOrderSFtp.OutputHost, clientOrderSFtp.OutputPort, clientOrderSFtp.OutputUsername, privateKey);
                    sftpClient.OperationTimeout = TimeSpan.FromMinutes(60);
                    sftpClient.BufferSize = 1024 * 1024 * 10;
                    return sftpClient;
                }
                
            }
        }

        public async Task ReadDirectory(SftpClient sftpClient)
        {
            var rootFolder = "/home/storage01/public_html/six/To CutOutWiz";
            var listing = sftpClient.ListDirectory(rootFolder);
            foreach (var item in listing)
            {
                var path = "/home/storage01/public_html/six/To CutOutWiz";
            }
        }


        public async Task RecursiveListFiles(SftpClient client, string remotePath, List<string> filePaths)
        {
            var files = client.ListDirectory(remotePath);

            files = files.Where(x=>x.Name != "_downloaded").ToList();

            foreach (var file in files)
            {
                if (file.IsDirectory && !file.Name.StartsWith("."))
                {
                    await RecursiveListFiles(client, $"{remotePath}/{file.Name}", filePaths);
                }
                else if (file.IsRegularFile)
                {
                    filePaths.Add($"{remotePath}/{file.Name}");
                }
            }
        }

        public async Task<int> RecursiveListFilesMove(SftpClient client, string remotePath, string prefix)
        {
            int moveFileCount = 0;
            var files = client.ListDirectory(remotePath);
            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(20);
            var uploadingTasks = new List<Task>();
            if (files == null || !files.Any())
            {
                return 0;
            }

            foreach (var file in files)
            {

                await semaphoreSlim.WaitAsync();

                uploadingTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            if (file.IsDirectory && !file.Name.StartsWith('.'))
                            {
                                await RecursiveListFilesMove(client, $"{remotePath}/{file.Name}", prefix);
                            }
                            else if (file.IsRegularFile)
                            {
                                //filePaths.Add($"{remotePath}/{file.Name}");
                                try
                                {
                                    var rootFolderName = Path.GetFileName(prefix);
                                    var subFolderPath = file.FullName.Split(rootFolderName)[1];
                                    var finalFolderPath = prefix + Path.GetDirectoryName(subFolderPath);
                                    finalFolderPath = finalFolderPath.Replace("\\", "/");
                                    if (!client.Exists(finalFolderPath))
                                    {
                                        //client.CreateDirectory(finalFolderPath);
                                        await RecursiveCreateDirectories(client, finalFolderPath);

                                    }
                                    string fullFinalPath = finalFolderPath + "/" + file.Name;

                                    if (client.Exists(fullFinalPath))
                                    {
                                        client.Delete(fullFinalPath);
                                    }
                                    if (!client.Exists(fullFinalPath))
                                    {
                                        file.MoveTo(fullFinalPath);
                                        moveFileCount++;
                                    }

                                }
                                catch (Exception ex)
                                {

                                    var loginUser = new LoginUserInfoViewModel
                                    {
                                        ContactId = (int)AutomatedAppEnum.ContactId
                                    };

                                    CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                                    {
                                        //PrimaryId = (int)order.Id,
                                        ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
                                        loginUser = loginUser,
                                        ErrorMessage = ex.InnerException?.ToString(),
                                        MethodName = "MoveOrderToClientFtp",
                                        RazorPage = "SSh Net Service",
                                        Category = (int)ActivityLogCategory.FileMoveOnSFTP,
                                    };
                                    await _activityLogCommonMethodService.InsertAppErrorActivityLog(activity);

                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            var loginUser = new LoginUserInfoViewModel
                            {
                                ContactId = (int)AutomatedAppEnum.ContactId
                            };

                            CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                            {
                                //PrimaryId = (int)order.Id,
                                ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoCompleted,
                                loginUser = loginUser,
                                ErrorMessage = ex.InnerException.ToString(),
                                MethodName = "MoveOrderToClientFtp",
                                RazorPage = "uploadtoclientftp",
                                Category = (int)ActivityLogCategory.FileMoveOnSFTP
                            };
                            await _activityLogCommonMethodService.InsertAppErrorActivityLog(activity);
                        }
                        finally
                        {
                            semaphoreSlim.Release();
                        }

                    }));
            }
            await Task.WhenAll(uploadingTasks);
            return moveFileCount;
        }


        public async Task RecursiveCreateDirectories(SftpClient client, string remoteDirectoryPath)
        {
            await Task.Yield();
            try
            {
                string[] directoryParts = remoteDirectoryPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (directoryParts.Length > 0)
                {
                    string currentPath = "/";
                    foreach (string directoryPart in directoryParts)
                    {
                        currentPath = $"{currentPath}{directoryPart}";

                        if (!client.Exists(currentPath))
                        {
                            client.CreateDirectory(currentPath);
                        }

                        currentPath = $"{currentPath}/";
                    }
                }

            }
            catch (Exception ex)
            {


            }


        }


        public async Task<bool> UploadFileWithRateLimit(SftpClient client, string localPath, string remoteDirectory, int rateLimit)
        {
            bool result = false;
            try
            {
                using (var localFileStream = new FileStream(localPath, FileMode.Open))
                using (var remoteFileStream = client.Create(Path.Combine(remoteDirectory, Path.GetFileName(localPath))))
                {
                    byte[] buffer = new byte[rateLimit];
                    int bytesRead;

                    while ((bytesRead = localFileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        remoteFileStream.Write(buffer, 0, bytesRead);
                        await Task.Delay(1000); // Delay for 1 second to limit the rate
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task RecursiveDeleteDiretories(SftpClient client,string directory)
        {

            foreach (var file in client.ListDirectory(directory))
            {
                if (file.IsDirectory && !file.Name.Equals(".") && !file.Name.Equals(".."))
                {
                    RecursiveDeleteDiretories(client, file.FullName);
                }
                else if (file.IsRegularFile)
                {
                    client.DeleteFile(file.FullName);
                }
            }

            client.DeleteDirectory(directory);

        }


        public async Task SingleFileMove(ClientExternalOrderFTPSetupModel sourceFtp, string moveFileDestination,CompanyGeneralSettingModel? companyGeneralSetting = null)
        {
            SftpClient sftpClient = await CreateSshNetConnector(true, sourceFtp);
            sftpClient.OperationTimeout = TimeSpan.FromMinutes(50);
            sftpClient.Connect();

            var file = sftpClient.Get(moveFileDestination);
            var moveFolderPath = "";
            if (companyGeneralSetting != null)
            {
                moveFolderPath = Path.GetDirectoryName(moveFileDestination) + $"/{companyGeneralSetting.FtpFileMovedPathAfterOrderCreated}/";
            }
            else
            {
                moveFolderPath = Path.GetDirectoryName(moveFileDestination) + "/_downloaded/";
            }

            if (!sftpClient.Exists(moveFolderPath))
            {
               await RecursiveCreateDirectories(sftpClient, moveFolderPath);
            }


            var fileMoveDestinationPath = moveFolderPath + Path.GetFileName(moveFileDestination);

            fileMoveDestinationPath = fileMoveDestinationPath.Replace("\\", "/");
            file.MoveTo(fileMoveDestinationPath);
        }
    }
}
