using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.Models.FileUpload;
using FluentFTP;
using FluentFTP.Helpers;
using Microsoft.JSInterop;
using static CutOutWiz.Core.Utilities.Enums;
using static Google.Apis.Requests.BatchRequest;

namespace CutOutWiz.Services.StorageService
{
    public class FluentFtpService : IFluentFtpService
    {
        public double maxValue { get; set; }
        public double CurrentValue { get; set; }

        public async Task<AsyncFtpClient> CreateFtpClient(FileServerViewModel model)
        {
            await Task.Yield();
            var host = model.Host.Split(':');

            if (host.Length == 3)
            {
                var client = new AsyncFtpClient($"{host[0]}:{host[1]}", model.UserName, model.Password, Convert.ToInt32(host[2]));
                return client;
            }
            else
            {
                var client = new AsyncFtpClient(model.Host, model.UserName, model.Password);
                return client;
            }
        }

        private async Task<AsyncFtpClient> CreateFtpClientUsingFileServerModel(FileUploadModel model)
        {
            await Task.Yield();
            var host = model.FtpUrl.Split(':');

            if (host.Length == 3)
            {
                var client = new AsyncFtpClient($"{host[0]}:{host[1]}", model.userName, model.password, Convert.ToInt32(host[2]));
                return client;
            }
            else
            {
                var client = new AsyncFtpClient(model.FtpUrl, model.userName, model.password);
                return client;
            }
        }

        public async Task<bool> DeleteFile(FileUploadModel model)
        {
            bool result = false;
            //var client = new AsyncFtpClient(model.FtpUrl, model.userName, model.password);
            var client = await CreateFtpClientUsingFileServerModel(model);
            client.Config.EncryptionMode = FtpEncryptionMode.Auto;
            client.Config.ValidateAnyCertificate = true;
            if (client != null && await client.FileExists($"{model.ReturnPath}"))
            {
                await client.DeleteFile(model.ReturnPath);
                result = true;
            }
            return result;
        }

        public async Task<string> UplodFile(FileUploadModel model)
        {
            //var client = new AsyncFtpClient(model.FtpUrl, model.userName, model.password);
            var client = await CreateFtpClientUsingFileServerModel(model);
            var remote = "";
            try
            {
                await client.AutoConnect();
                var remotePath = $"{model.UploadDirectory}{model.fileName}";

                if (model.file != null)
                {
                    FtpStatus status = await client.UploadBytes(model.ImageByte, remotePath, FtpRemoteExists.Overwrite, true);
                    remote = $"{model.UploadDirectory}";
                }
                await client.Disconnect();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            return remote;
        }
        public async Task<Response<bool>> MoveFile(FileUploadModel model)
        {
            var response = new Response<bool>();
            //var client = new AsyncFtpClient(model.FtpUrl, model.userName, model.password);
            int tryCount = 0;
            while (tryCount <= 3)
            {
                using (var client = await CreateFtpClientUsingFileServerModel(model))
                {
                    try
                    {
                        client.Config.EncryptionMode = FtpEncryptionMode.Auto;
                        client.Config.ValidateAnyCertificate = true;
                        await client.AutoConnect();
                        var copyPath = model.UploadDirectory;
                        var savePath = model.ReturnPath;

                        if (!string.IsNullOrWhiteSpace(model.SubFolder))
                        {

                            copyPath = $"{model.SubFolder}/{copyPath}";
                            savePath = $"{model.SubFolder}/{savePath}";
                        }

                        if (!await client.DirectoryExists(savePath))
                        {
                            await client.CreateDirectory(Path.GetDirectoryName(savePath));
                        }
                        copyPath = copyPath.Replace("//", "/");
                        savePath = savePath.Replace("//", "/");

                        var ftpResult = await client.MoveFile(copyPath, savePath, FtpRemoteExists.Overwrite);
                        await client.Disconnect();
                        response.IsSuccess = ftpResult;
                        break;
                    }
                    catch (Exception)
                    {

                        Thread.Sleep(1000);
                        tryCount++;
                    }
                }
            }
            return response;
        }
        public async Task FolderCreateAtApprovedTime(FileUploadModel model)
        {
            //var client = new AsyncFtpClient(model.FtpUrl, model.userName, model.password);
            var client = await CreateFtpClientUsingFileServerModel(model);
            client.Config.EncryptionMode = FtpEncryptionMode.Auto;
            client.Config.ValidateAnyCertificate = true;
            await client.AutoConnect();
            if (!await client.DirectoryExists(model.ReturnPath))
            {
                await client.CreateDirectory(model.ReturnPath);
            }
            await client.Disconnect();
        }

        public async Task<bool> FileExists(FileUploadModel model)
        {
            //var client = new AsyncFtpClient(model.FtpUrl, model.userName, model.password);
            var client = await CreateFtpClientUsingFileServerModel(model);

            bool response = false;
            client.Config.EncryptionMode = FtpEncryptionMode.Auto;
            client.Config.ValidateAnyCertificate = true;
            await client.AutoConnect();


            if (!string.IsNullOrEmpty(model.SubFolder))
            {
                if (!string.IsNullOrEmpty(model.FolderName))
                {
                    if (await client.FileExists($"{model.SubFolder}\\{model.ReturnPath}\\{model.FolderName}\\{model.file.Name}"))
                    {
                        response = true;
                    }
                }
                else if (await client.FileExists($"{model.SubFolder}\\{model.ReturnPath}\\{model.OrderNumber}\\{model.file.Name}"))
                {
                    response = true;
                }
                else
                {
                    response = false;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.FolderName))
                {
                    if (await client.FileExists($"{model.ReturnPath}\\{model.FolderName}\\{model.file.Name}"))
                    {
                        response = true;
                    }
                }
                else if (await client.FileExists($"{model.ReturnPath}\\{model.OrderNumber}\\{model.file.Name}"))
                {
                    response = true;
                }
                else
                {
                    response = false;
                }
            }


            await client.Disconnect();
            return response;
        }
        public async Task<bool> SingleDownload(IJSRuntime js, FileUploadModel model, string WebHostEnvironmentPath = null)
        {
            //timer.StartTimer();
            //var client = new AsyncFtpClient(model.FtpUrl, model.userName, model.password);
            var client = await CreateFtpClientUsingFileServerModel(model);
            await client.AutoConnect();
            var dlpath = $"{WebHostEnvironmentPath}\\TempDownload\\";
            maxValue = model.clientOrderItems.Count();
            var count = 0;
            foreach (var item in model.clientOrderItems)
            {
                var dataSavePath = dlpath + $"\\{model.Contact.FirstName + model.Contact.Id}\\{item.PartialPath}";
                if (!Directory.Exists(dataSavePath))
                {
                    Directory.CreateDirectory(dataSavePath);
                }
                var localPath = $"{dataSavePath}/{item.FileName}";
                await client.DownloadFile(localPath, item.InternalFileInputPath);

                count++;
                CurrentValue = Math.Round((float)((100 / maxValue) * count), 2);
            }

            await client.Disconnect();
            return true;
        }

        public async Task<bool> FolderExists(FileServerViewModel fileServerViewModel, string path)
        {
            bool response = false;
            //var client = new AsyncFtpClient(fileServerViewModel.Host, fileServerViewModel.UserName, fileServerViewModel.Password);
            var client = await CreateFtpClient(fileServerViewModel);
            await client.AutoConnect();
            if (await client.DirectoryExists(path))
            {
                response = true;
            }
            await client.Disconnect();
            return response;

        }

        public async Task<FtpConfig> GetFluentFtpConfig()
        {
            await Task.Yield();
            return new FtpConfig
            {
                ConnectTimeout = 1000 * 60 * 10,
                EncryptionMode = FtpEncryptionMode.Auto,
                ValidateAnyCertificate = true,
                TransferChunkSize = 1024 * 1024 * 10,
                ReadTimeout = 1000 * 60 * 10,
                DataConnectionConnectTimeout = 1000 * 60 * 10,
                LocalFileBufferSize = 1024 * 1024 * 10,  //Change this and try again
                RetryAttempts = 5,
                UploadRateLimit = 0
            };
        }
        /// <summary>
        /// Retrieves the corresponding FtpEncryptionMode based on the provided integer value.
        /// </summary>
        /// <param name="enumValue">An integer representing the desired FtpEncryptionMode.</param>
        /// <returns>
        /// Returns the corresponding FtpEncryptionMode:
        /// - 0: FtpEncryptionMode.None
        /// - 1: FtpEncryptionMode.Implicit
        /// - 3: FtpEncryptionMode.Explicit
        /// If the provided enumValue does not match a valid case, FtpEncryptionMode.Auto is returned by default.
        /// </returns>
        //public FtpEncryptionMode GetFtpEncryptionModeEnumKey(int enumValue)
        //{
        //    switch (enumValue)
        //    {
        //        case 0:
        //            return FtpEncryptionMode.None;
        //        case 1:
        //            return FtpEncryptionMode.Implicit;
        //        case 2:
        //            return FtpEncryptionMode.Explicit;
        //        default:
        //            // If the value is not defined in the enum, return Auto as the default
        //            return FtpEncryptionMode.Auto;
        //    }
        //}
        public static FtpEncryptionMode GetFtpEncryptionModeEnumKey(int enumValue)
        {
            switch (enumValue)
            {
                case 0:
                    return FtpEncryptionMode.None;
                case 1:
                    return FtpEncryptionMode.Implicit;
                case 2:
                    return FtpEncryptionMode.Explicit;
                default:
                    // If the value is not defined in the enum, return Auto as the default
                    return FtpEncryptionMode.Auto;
            }
        }

    }
}
