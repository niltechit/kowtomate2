using CutOutWiz.Core;
using FluentFTP;
using System.Net;
using Microsoft.Extensions.Configuration;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services.FolderServices;
using Microsoft.JSInterop;
using System.IO.Compression;
using FluentFTP.Helpers;
using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.ClientOrders;
using System.IO;
using CutOutWiz.Services.Models.FileUpload;

namespace CutOutWiz.Services.StorageService
{
    public class FtpService : IFtpService
    {
        private readonly string _ftpServer = "storage01.cutoutwiz.com";
        private readonly string _ftpUsername = "testapprovaltool@storage.cutoutwiz.com";
        private string _ftpPassword = "Approval12345";
        private readonly ISqlDataAccess _db;
        private IConfiguration _configuration;
        private IFolderService _folderService;
        DateTimeConfiguration _dateTime = new DateTimeConfiguration();

        public FtpService(IConfiguration configuration, ISqlDataAccess db, IFolderService folderService)
        {
            _configuration = configuration;
            _db = db;
            _folderService = folderService;
        }

        //public Response<List<Node>> ReadDirectoriesAsync(string path)
        //{
        //    var response = new Response<List<Node>>();

        //    try
        //    {
        //        using (FtpClient ftp = new FtpClient(_ftpServer,
        //                new NetworkCredential
        //                {
        //                    UserName = _ftpUsername,
        //                    Password = _ftpPassword
        //                }))
        //        {
        //            FtpListItem[] ftpListItems = ftp.GetListing(path);
        //            List<Node> nodes = new List<Node>();

        //            foreach (var ftpListItem in ftpListItems)
        //            {
        //                if (ftpListItem.Type == FtpFileSystemObjectType.Directory)
        //                {
        //                    var node = new Node(ftpListItem.FullName, ftpListItem.Name, true);

        //                    nodes.Add(node);
        //                }
        //            }
        //            response.Result = nodes;
        //            response.IsSuccess = true;

        //            ftp.Disconnect();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = ex.Message;
        //    }

        //    return response;
        //}

        //public Response<List<DriveImage>> ReadFilesAsync(string path)
        //{
        //    var response = new Response<List<DriveImage>>();

        //    using (FtpClient ftp = new FtpClient(_ftpServer,
        //            new NetworkCredential
        //            {
        //                UserName = _ftpUsername,
        //                Password = _ftpPassword
        //            }))
        //    {
        //        FtpListItem[] ftpListItems = ftp.GetListing(path);
        //        var images = new List<DriveImage>();

        //        foreach (var ftpListItem in ftpListItems)
        //        {
        //            if (ftpListItem.Type == FtpFileSystemObjectType.File)
        //            {
        //                DriveImage node = new DriveImage(path, ftpListItem.Name, ftpListItem.FullName, ftpListItem.Size, DateTime.Now);
        //                //node.Data = GetBase64String(ftpListItem.FullName);
        //                images.Add(node);
        //            }
        //        }

        //        ftp.Disconnect();

        //        response.IsSuccess = true;
        //        response.Result = images;
        //    }

        //    return response;
        //}
        //public Response<string> GetBase64String(string filePath, bool reduceSize = false)
        //{
        //    var methodResponse = new Response<string>();
        //    //FTP Server URL.
        //    string fullPath = $"ftp://{_ftpServer}/{filePath}"; //"ftp://yourserver.com/";

        //    //FTP Folder name. Leave blank if you want to list files from root folder.
        //    //string ftpFolder = "Uploads/";
        //    try
        //    {
        //        //string fileName = "Desert.jpg";
        //        //Create FTP Request.
        //        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fullPath);
        //        request.Method = WebRequestMethods.Ftp.DownloadFile;

        //        //Enter FTP Server credentials.
        //        request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
        //        request.UsePassive = true;
        //        request.UseBinary = true;
        //        request.EnableSsl = false;

        //        //Fetch the Response and read it into a MemoryStream object.
        //        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        //        using (MemoryStream stream = new MemoryStream())
        //        {
        //            response.GetResponseStream().CopyTo(stream);

        //            if (reduceSize)
        //            {
        //                //Rrduce Images
        //                var newImage = GetReducedImage(50, 50, stream);
        //                //var ms = new MemoryStream();
        //                newImage.Save(stream, ImageFormat.Png); ///TODO: need to 
        //               // var byteArray = stream.ToArray(); stream.Close();
        //                //var base64 = "data:image/png;base64," + Convert.ToBase64String(byteArray);
        //            }

        //            string base64String = Convert.ToBase64String(stream.ToArray(), 0, stream.ToArray().Length);
        //            stream.Close();
        //            methodResponse.Result = "data:image/png;base64," + base64String;
        //        }

        //        methodResponse.IsSuccess = true;               
        //    }
        //    catch (WebException ex)
        //    {
        //        methodResponse.Message = ex.Message;
        //        //throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
        //    }

        //    return methodResponse;
        //}

        //private Image GetReducedImage(int width, int height, Stream resourceImage)
        //{
        //    try
        //    {
        //        var image = Image.FromStream(resourceImage);
        //        var thumb = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);

        //        return thumb;
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}
        public FtpClient CreateFtpClient(FileUploadModel model)
        {
			var host = model.FtpUrl.Split(':');

			if (host.Length == 3)
			{
				return new FtpClient($"{host[0]}:{host[1]}", model.userName, model.password, Convert.ToInt32(host[2]));
			}
            else
            {
				return new FtpClient(model.FtpUrl, model.userName, model.password);
			}
			
        }

        public AsyncFtpClient CreateAsyncFtpClient(FileUploadModel model)
        {
            var host = model.FtpUrl.Split(':');
            if (host.Length == 3)
            {
                return new AsyncFtpClient($"{host[0]}:{host[1]}", model.userName, model.password, Convert.ToInt32(host[2]));
            }
            else
            {
                return new AsyncFtpClient(model.FtpUrl, model.userName, model.password);
            }
        }

        //public async Task<string> UploadFile(FileUploadVM model)
        //{
        //    string remote = "";
        //    using (FtpClient ftp = CreateFtpClient(model))
        //    {
        //        ftp.Connect();
        //        using (MemoryStream fs = new MemoryStream())
        //        {
        //            await model.file.OpenReadStream(maxAllowedSize: 1024000000000).CopyToAsync(fs);
        //            //ftp.Connect();
        //            //string remotePat = @"KowToMateERPFiles/" + model.fileName;
        //            var remotePath = $"{model.UploadDirectory}{model.fileName}"; 
        //            await ftp.UploadAsync(fs, remotePath);
        //            remote = $"{model.UploadDirectory}";
        //        }
        //        ftp.Disconnect();
        //    }
        //    return remote;
        //}

        public async Task<string> UploadFile(FileUploadModel model)
        {
            string remote = "";
            try
            {
                using (FtpClient ftp = CreateFtpClient(model))
                {
                    ftp.Connect();
                    var remotePath = $"{model.UploadDirectory}{model.fileName}";

                    if (model.file != null)
                    {
                        //await ftp.UploadAsync(model.file.OpenReadStream(maxAllowedSize: 1024000000000), remotePath);
                        remote = $"{model.UploadDirectory}";
                    }
                    ftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }

            return remote;
        }

        public async Task<string> CheckingFtpFolder(FileUploadModel model)
        {
            var returnPath ="";
            var client = CreateFtpClient(model);
            client.Connect();
            if (client.DirectoryExists(model.ReturnPath))
            {
                if (!string.IsNullOrWhiteSpace(model.BaseFolder))
                {
                    var path = $"{model.ReturnPath}{model.BaseFolder}/";
                    client.CreateDirectory(path);
                    returnPath = path;
                    return returnPath;
                }
                else
                {
                    returnPath = model.ReturnPath;
                }
            }
            else {
                returnPath=model.ReturnPath;
                return returnPath;
            }
            client.Disconnect();
            returnPath = model.ReturnPath;
            return returnPath;
        }
        public async Task<string> CheckingFtpFolderForQC(FileUploadModel model)
        {
            var returnPath = "";
            var client = CreateFtpClient(model);
            client.Connect();
            if (client.DirectoryExists(model.ReturnPath))
            {
                if (!string.IsNullOrWhiteSpace(model.BaseFolder))
                {
                    var path = $"{model.ReturnPath}{model.BaseFolder}/";
                    client.CreateDirectory(path);
                    returnPath = path;
                    return returnPath;
                }
                else
                {
                    returnPath = model.ReturnPath;
                }
            }
            else
            {
                returnPath = model.ReturnPath;
                return returnPath;
            }
            client.Disconnect();
            returnPath = model.ReturnPath;
            return returnPath;
        }
        public async Task<string> CreateFtpFolderForEditor(FileUploadModel model)
        {
            var client = CreateFtpClient(model);
            client.Connect();
            DateTime current = DateTime.Now;
            var year = current.ToString("yyyy");
            var monthName = current.ToString("MMMM");
            var formattedDate = current.ToString("dd.MM.yyyy");

            var remotepath = $"{model.RootDirectory}/{year}/{monthName}/{formattedDate}/";
            if (client.DirectoryExists(remotepath))
            {
                var editorPath = $"{remotepath}In Progress/";
                client.CreateDirectory(editorPath);
                if (client.DirectoryExists(editorPath))
                {
                    var editorFulPath = $"{editorPath}{model.OrderNumber}";
                    client.CreateDirectory(editorFulPath);
                    if (client.DirectoryExists(editorFulPath))
                    {
                        var editorFullPath = $"{editorFulPath}/{model.ContactName}";
                        client.CreateDirectory(editorFullPath);
                        remotepath = editorFullPath;
                        if (client.DirectoryExists(editorFullPath))
                        {
                            var path = $"{editorFullPath}/In Production";
                            var path2 = $"{editorFullPath}/Production Done";
                            var path3 = $"{editorFullPath}/Rejected";
                            client.CreateDirectory(path);
                            client.CreateDirectory(path2);
                            client.CreateDirectory(path3);
                        }
                        
                    }
                }
            }
            return remotepath;
        }
        public async Task<bool> CheckingFtpFolderForEditor(FileUploadModel model)
        {
            var client = CreateFtpClient(model);
            client.Connect();
            var result = false;
            if (client.DirectoryExists(model.ReturnPath))
            {
                result = true;
            }
            return result;
        }

        public async Task<string> CreateFtpFolder(FileUploadModel model)
        {
            var client = CreateFtpClient(model);
            client.Connect();
            var remotePath = $"{model.RootDirectory}\\{_dateTime.Year}\\{_dateTime.Month}\\{_dateTime.Date}\\Raw\\{model.BaseFolder}\\";
            if (!client.DirectoryExists(remotePath))
            {
                client.CreateDirectory(model.RootDirectory);
                var checkPath = $"{model.RootDirectory}/{_dateTime.Year}";
                if (!client.DirectoryExists(remotePath))
                {
                    client.CreateDirectory(checkPath);
                    //client.
                    var month = $"{model.RootDirectory}/{_dateTime.Year}/{_dateTime.Month}";
                    if (!client.DirectoryExists(remotePath))
                    {
                        client.CreateDirectory(month);
                        var folder = $"{model.RootDirectory}/{_dateTime.Year}/{_dateTime.Month}/{_dateTime.Date}";
                        if (!client.DirectoryExists(remotePath))
                        {
                            client.CreateDirectory(folder);
                            var rawPaths = $"{model.RootDirectory}/{_dateTime.Year}/{_dateTime.Month}/{_dateTime.Date}/Raw/{model.BaseFolder}";
                            if (!client.DirectoryExists(remotePath))
                            {
                                client.CreateDirectory(rawPaths);
                                
                                if (!string.IsNullOrEmpty(model.FolderName))
                                {
                                    remotePath = $"{rawPaths}\\{model.FolderName}/";
                                    client.CreateDirectory(remotePath);
                                    var replace = remotePath.Replace("\\", "/");
                                    remotePath = replace;
                                }
                            }
                        }
                    }
                }
                return remotePath;
            }
            else if (!string.IsNullOrEmpty(model.FolderName))
            {
                var path = $"{remotePath}\\{model.FolderName}/";
                if (!client.DirectoryExists(path))
                {
                    client.CreateDirectory(path);
                    remotePath = path.Replace("\\","/");
                    return remotePath;
                }
                else
                {
                    var replaceRemotePath= path.Replace("\\","/");
                    remotePath = replaceRemotePath;
                    client.Disconnect();
                    return remotePath;
                }

            }
            else {
                var replaceRemotePath = remotePath.Replace("\\", "/");
                client.Disconnect();
                return remotePath;
            }
            
        } 
        public async Task<string> CheckingCreateFolderForOrderAttachment(FileUploadModel model)
        {
            var client = CreateFtpClient(model);
            client.Connect();

            var remotePath = $"{model.RootDirectory}/{_dateTime.Year}/{_dateTime.Month}/{_dateTime.Date}/Raw/{model.OrderNumber}/";
            if (!client.DirectoryExists(remotePath))
            {
                client.CreateDirectory(model.RootDirectory);
                var checkPath = $"{model.RootDirectory}/{_dateTime.Year}";
                if (!client.DirectoryExists(remotePath))
                {
                    client.CreateDirectory(checkPath);
                    //client.
                    var month = $"{model.RootDirectory}/{_dateTime.Year}/{_dateTime.Month}";
                    if (!client.DirectoryExists(remotePath))
                    {
                        client.CreateDirectory(month);
                        var folder = $"{model.RootDirectory}/{_dateTime.Year}/{_dateTime.Month}/{_dateTime.Date}";
                        if (!client.DirectoryExists(remotePath))
                        {
                            client.CreateDirectory(folder);
                            var rawPaths = $"{model.RootDirectory}/{_dateTime.Year}/{_dateTime.Month}/{_dateTime.Date}/Raw/{model.OrderNumber}";
                            if (!client.DirectoryExists(remotePath))
                            {
                                client.CreateDirectory(rawPaths);
                                
                                if (!string.IsNullOrEmpty(model.BaseFolder))
                                {
                                    var pathreplace = $"{remotePath}{model.BaseFolder}";
                                    var replace = pathreplace.Replace("\\", "/");
                                    remotePath = $"{rawPaths}/{replace}/";
                                    //var path = $"{rawPaths}/{model.BaseFolder}";
                                    client.CreateDirectory(remotePath);
                                    //return remotePath;
                                }
                            }
                        }
                    }
                }
                client.Disconnect();
                return remotePath;
            }
            else if (!string.IsNullOrEmpty(model.BaseFolder))
            {
                var path = $"{remotePath}{model.BaseFolder}/";
                if (!client.DirectoryExists(path))
                {
                    client.CreateDirectory(path);
                    remotePath = path;
                    client.Disconnect();
                    return remotePath;
                }
                else
                {
                    remotePath = $"{remotePath}{model.BaseFolder}/";
                    client.Disconnect();
                    return remotePath;
                }

            }
            else {
                client.Disconnect();
                return remotePath;
            }
           
        }
        public async Task<FileServerModel> GetById(int Id)
        {
            var result = await _db.LoadDataUsingProcedure<FileServerModel, dynamic>(storedProcedure: "dbo.SP_Common_FileServer_GetById", new { FileServerId = Id });
            return result.FirstOrDefault();
        }
        public async Task<bool> DownloadFile(IJSRuntime js,FileUploadModel model)
        {
            var client = CreateFtpClient(model);
            client.Connect();
            var folder = $"C:\\Download";
            await _folderService.CreateFolder(folder);
            if (model.fileName!=null)
            {
                byte[] bytes;
                using (MemoryStream stream = new MemoryStream())
                {
                    //client.Download(stream, $"{model.DownloadDirectory}/{model.fileName}");
                    bytes = stream.ToArray();
                    await js.InvokeVoidAsync("jsDownloadFile",model.fileName, bytes);
                }
                return true;
            }
            client.Disconnect();
            return false;
        }
        public async Task<bool> MultipleDownload(IJSRuntime js, FileUploadModel model,string dlPath)
        {
            var client = CreateFtpClient(model);
            client.Connect();
            var dataSavePath = dlPath + @"\DownloadOrder";
            client.DownloadDirectory(dataSavePath, $"{model.UploadDirectory}", FtpFolderSyncMode.Mirror);
            client.Disconnect();
            string extractFilePath = Path.Combine(dataSavePath, model.UploadDirectory) ;
            ZipFile.CreateFromDirectory(extractFilePath, dataSavePath + "\\"+model.OrderNumber+".zip");
            string baseUrl = $"{_configuration["AppMainUrl"]}/";
            var dlUri = baseUrl + @"DownloadOrder/" + model.OrderNumber + ".zip";
            await js.InvokeVoidAsync("triggerFileDownload", model.OrderNumber+".zip", dlUri);
            return true;
        }
        public async Task CreateFolderDownloadTime(FileUploadModel model)
        {
            var client = CreateFtpClient(model);
			client.Config.EncryptionMode = FtpEncryptionMode.Auto;
			client.Config.ValidateAnyCertificate = true;
			client.Connect();
            await _dateTime.DateTimeConvert(model.Date);
            var remotepath = "";

			if (!string.IsNullOrEmpty(model.SubFolder))
            {
				remotepath = $"{model.SubFolder}/{model.RootDirectory}/{_dateTime.year}/{_dateTime.month}/{_dateTime.date}/";
			}
            else
            {
				remotepath = $"{model.RootDirectory}/{_dateTime.year}/{_dateTime.month}/{_dateTime.date}/";
			}
          
            if (client.DirectoryExists(remotepath))
            {
                var editorPath = $"{remotepath}In Progress/";
                client.CreateDirectory(editorPath);
                if (client.DirectoryExists(editorPath))
                {
                    var editorFulPath = $"{editorPath}{model.OrderNumber}";
                    client.CreateDirectory(editorFulPath);
                    if (client.DirectoryExists(editorFulPath))
                    {
                        var editorFullPath = $"{editorFulPath}/{model.ContactName}";
                        client.CreateDirectory(editorFullPath);
                        remotepath = editorFullPath;
                        if (client.DirectoryExists(editorFullPath))
                        {
                            var path = $"{editorFullPath}/In Production";
                            var path2 = $"{editorFullPath}/Production Done";
                            var path3 = $"{editorFullPath}/Rejected";
                            client.CreateDirectory(path);
                            client.CreateDirectory(path2);
                            client.CreateDirectory(path3);
                        }
                    }
                }
            }
            //client.MoveFile($"{model.UploadDirectory}/{model.fileName}", $"{remotepath}/In Production/{model.fileName}") ;
            client.Disconnect();
        }
        public async Task FolderCreateAtApprovedTime(FileUploadModel model)
        {
            var client = CreateFtpClient(model);
            client.Connect();
            if (!client.DirectoryExists(model.ReturnPath))
            {
                client.CreateDirectory(model.ReturnPath);
            }
            client.Disconnect();
        }

        public async Task<string> MoveFile(FileUploadModel model)
        {
            var client = CreateFtpClient(model);
			client.Config.EncryptionMode = FtpEncryptionMode.Auto;
			client.Config.ValidateAnyCertificate = true;
			client.Connect();

            await _dateTime.DateTimeConvert(model.Date);
            var remotepath = $"{model.RootDirectory}/{_dateTime.year}/{_dateTime.month}/{_dateTime.date}/";

            if (client.DirectoryExists(remotepath))
            {
                var editorPath = $"{remotepath}Completed/";
                
                if (!client.DirectoryExists(editorPath))
                {
                    client.CreateDirectory(editorPath);
                    remotepath=editorPath;
                }
                remotepath = editorPath;
                //client.CreateDirectory(editorPath);
                var orderFolder = $"{editorPath}{model.OrderNumber}";
                if (!client.DirectoryExists(orderFolder))
                {
                    client.CreateDirectory(orderFolder);
                    remotepath = orderFolder;
                }
                remotepath = orderFolder;
               
            }
            var localPath = $"{model.UploadDirectory}/{model.fileName}";

            var destPath = "";

			if (!string.IsNullOrWhiteSpace(model.SubFolder))
			{
				destPath = $"{model.SubFolder}/{model.UploadDirectory}/{model.fileName}";
			}
            else
            {
				destPath = $"{remotepath}/{model.fileName}";
			}
			
            client.MoveFile(localPath, destPath) ;
            var destinationPath = $"{remotepath}/";
            client.Disconnect();
            return destinationPath;
        }

        public async Task MoveFileForReject(FileUploadModel model)
        {
            var client = CreateFtpClient(model);
			client.Config.EncryptionMode = FtpEncryptionMode.Auto;
			client.Config.ValidateAnyCertificate = true;
			client.Connect();

			_dateTime.DateTimeConvert(model.Date);
            var remotepath = $"{model.RootDirectory}/{_dateTime.year}/{_dateTime.month}/{_dateTime.date}/";
            if (client.DirectoryExists(remotepath))
            {
                var editorPath = $"{remotepath}In Progress/{model.OrderNumber}/{model.ContactName}/Rejected";

                if (!client.DirectoryExists(editorPath))
                {
                    client.CreateDirectory(editorPath);
                    remotepath = editorPath;
                }
                remotepath = editorPath;
               
                var orderFolder = $"{editorPath}/{model.OrderNumber}";
                if (!client.DirectoryExists(orderFolder))
                {
                    client.CreateDirectory(orderFolder);
                    remotepath = orderFolder;
                }
                remotepath = orderFolder;
                
            }
			var destPath = "";

			if (!string.IsNullOrWhiteSpace(model.SubFolder))
			{
				destPath = $"{model.SubFolder}/{remotepath}/{model.fileName}";
			}
			else
			{
				destPath = $"{remotepath}/{model.fileName}";
			}
			var localPath = $"{model.UploadDirectory}/{model.fileName}";
            
            client.MoveFile(localPath, destPath);

            client.Disconnect();
        }
        public async Task<bool> SingleDownload(IJSRuntime js, FileUploadModel model, string dlPath,bool downloadAsZip=false)
        {
            var client = CreateFtpClient(model);
			client.Config.EncryptionMode = FtpEncryptionMode.Auto;
			client.Config.ValidateAnyCertificate = true;
			client.Connect();
            var dataSavePath = "";
			//var dataSavePath = dlPath + $"\\Download\\{model.OrderNumber}{model.DownloadFolderName}";
			if (downloadAsZip)
            {
				dataSavePath = dlPath + $"\\{model.ContactName}\\{model.DownloadFolderName}";
			}
            else
            {
				dataSavePath = dlPath + $"\\{model.DownloadFolderName}";
			}
            //var dataSavePath = dlPath + $"\\{model.ContactName}\\{model.DownloadFolderName}";
           
            if (!Directory.Exists(dataSavePath))
            {
                Directory.CreateDirectory(dataSavePath);
            }


            var localPath = $"{dataSavePath}/{model.fileName}";
            var remotePath = "";
			if (!string.IsNullOrWhiteSpace(model.SubFolder))
			{
				remotePath = $"{model.SubFolder}/{model.UploadDirectory}/{model.fileName}";
			}
            else{
				remotePath = $"{model.UploadDirectory}/{model.fileName}";
			}
			
            client.DownloadFile(localPath, remotePath);
            client.Disconnect();
            return true;
        }
		public async Task<bool> SingleDownloadByQc(IJSRuntime js, FileUploadModel model, string dlPath)
		{
            await Task.Yield();
			var client = CreateFtpClient(model);
			client.Config.EncryptionMode = FtpEncryptionMode.Auto;
			client.Config.ValidateAnyCertificate = true;
			client.Connect();
			//var dataSavePath = dlPath + $"\\Download\\{model.OrderNumber}{model.DownloadFolderName}";
			//var dataSavePath = dlPath + $"\\{model.ContactName}\\{model.DownloadFolderName}";
			//if (!Directory.Exists(dlPath))
			//{
			//	Directory.CreateDirectory(dlPath);
			//}
			var localPath = dlPath;

			var remotePath = "";
			if (!string.IsNullOrWhiteSpace(model.SubFolder))
			{
				remotePath = $"{model.SubFolder}/{model.UploadDirectory}/{model.fileName}";
			}
			else
			{
				remotePath = $"{model.UploadDirectory}/{model.fileName}";
			}
		
			client.DownloadFile(localPath, remotePath);
			client.Disconnect();
			return true;
		}
		public async Task<bool> MultipleFileDownload(List<ClientOrderItemModel> clientOrderItems, FileUploadModel model, string dlPath)
		{
            using(var client = CreateFtpClient(model))
            {
				client.Connect();
				foreach (var orderItem in clientOrderItems)
				{
					model.fileName = orderItem.FileName;
					model.UploadDirectory = Path.GetDirectoryName(orderItem.InternalFileInputPath);//$"{fileInfo.ExteranlFileInputPath}";
					model.DownloadFolderName = orderItem.PartialPath;
					
                    var dataSavePath = dlPath + $"\\{model.ContactName}\\{model.DownloadFolderName}";
					if (!Directory.Exists(dataSavePath))
					{
						Directory.CreateDirectory(dataSavePath);
					}
					var localPath = $"{dataSavePath}/{model.fileName}";
					var remotePath = $"{model.UploadDirectory}/{model.fileName}";
					client.DownloadFile(localPath, remotePath);
				}
				client.Disconnect();
			}
           
			//var dataSavePath = dlPath + $"\\Download\\{model.OrderNumber}{model.DownloadFolderName}";
			return true;
		}

		// Folder and its file Upload 
		//Download without delete folder 
		public async Task<bool> DownloadWithNewFolder(IJSRuntime js, FileUploadModel model, string dlPath)
        {
            var client = CreateFtpClient(model);
            client.Connect();
            //var dataSavePath = dlPath + $"\\Download\\{model.OrderNumber}{model.DownloadFolderName}";
            var dataSavePath = dlPath + $"\\{model.ContactName}\\{model.DownloadFolderName}";
            if (!Directory.Exists(dataSavePath))
            {
                Directory.CreateDirectory(dataSavePath);
            }
            var localPath = $"{dataSavePath}/{model.fileName}";
            var path = Path.GetFullPath(model.fileName);

            var remotePath = $"{model.UploadDirectory}/{model.fileName}";
            client.DownloadFile(localPath, remotePath);
            client.Disconnect();
            return true;
        }
        public async Task<string> UploadFileAndFolder(FileUploadModel model)
        {
            string remote = "";
            using (FtpClient ftp = CreateFtpClient(model))
            {
                ftp.Connect();
                using (MemoryStream fs = new MemoryStream())
                {
                    //await model.file.OpenReadStream(maxAllowedSize: 1024000000000).CopyToAsync(fs);
                    ftp.Connect();
                    //string remotePat = @"KowToMateERPFiles/" + model.fileName;
                    //var remotePath = $"{model.UploadDirectory}{model.fileName}";
                    //await ftp.UploadAsync(fs, remotePath);
                    //var fromFolder = @"D:\Kow To Mate";
                    ftp.UploadDirectory(model.BaseFolder, $"{model.UploadDirectory}", FtpFolderSyncMode.Update);
                    //remote = $"{model.UploadDirectory}";
                }
                //ftp.UploadDirectory(@"C:\website\assets\", @"/public_html/assets", FtpFolderSyncMode.Mirror);
                ftp.Disconnect();
            }
            return remote;
        }
        public async Task<bool> SingleDownloadForAttachment(IJSRuntime js, FileUploadModel model, string dlPath)
        {
            var client = CreateFtpClient(model);
			client.Config.EncryptionMode = FtpEncryptionMode.Auto;
			client.Config.ValidateAnyCertificate = true;
			client.Connect();

            var formattedDate = DateTime.Now.ToString("ddMMyyyyhhmmss");
            //var dataSavePath = dlPath + $"\\Download\\{model.OrderNumber}{model.DownloadFolderName}";
            model.ContactName = $"{model.ContactName}{formattedDate}";
            var dataSavePath = dlPath + $"\\{model.ContactName}\\{model.OrderNumber}";
            if (!Directory.Exists(dataSavePath))
            {
                Directory.CreateDirectory(dataSavePath);
            }
            var localPath = $"{dataSavePath}/{model.fileName}";
            var path = Path.GetFullPath(model.fileName);
			var remotePath = "";

			if (!string.IsNullOrWhiteSpace(model.SubFolder))
			{
				remotePath = $"{model.SubFolder}/{model.UploadDirectory}/{model.fileName}";
			}
			else
			{
				remotePath = $"{model.UploadDirectory}/{model.fileName}";
			}

			
            client.DownloadFile(localPath, remotePath);
            client.Disconnect();
            return true;
        }

        public Response<List<NodeModel>> ReadDirectoriesAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Response<List<DriveImageModel>> ReadFilesAsync(string path)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteDirectory(string directoryPath)
        {
            bool result=false;
            if (Directory.Exists(directoryPath))
            {
                DirectoryInfo directory = new DirectoryInfo(directoryPath);
                directory.Delete(true);
                result = true;
                return result;
            }
            else
            {
                result = false;
            }
            return result;
        }
        // Folder and its file Upload 
    }
}