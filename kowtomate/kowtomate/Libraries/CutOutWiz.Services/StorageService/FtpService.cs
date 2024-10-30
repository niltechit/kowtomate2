using CutOutWiz.Data;
using FluentFTP;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

namespace CutOutWiz.Services.StorageService
{
    public class FtpService
    {
        private readonly string _ftpServer = "storage01.cutoutwiz.com";
        private readonly string _ftpUsername = "testapprovaltool@storage.cutoutwiz.com";
        private string _ftpPassword = "Approval12345";

        public Response<List<Node>> ReadDirectoriesAsync(string path)
        {
            var response = new Response<List<Node>>();

            try
            {
                using (FtpClient ftp = new FtpClient(_ftpServer,
                        new NetworkCredential
                        {
                            UserName = _ftpUsername,
                            Password = _ftpPassword
                        }))
                {
                    FtpListItem[] ftpListItems = ftp.GetListing(path);
                    List<Node> nodes = new List<Node>();

                    foreach (var ftpListItem in ftpListItems)
                    {
                        if (ftpListItem.Type == FtpFileSystemObjectType.Directory)
                        {
                            var node = new Node(ftpListItem.FullName, ftpListItem.Name, true);

                            nodes.Add(node);
                        }
                    }
                    response.Result = nodes;
                    response.IsSuccess = true;

                    ftp.Disconnect();
                }
            }
            catch(Exception ex)
            {
                response.Message = ex.Message;                
            }

            return response;
        }

        public Response<List<DriveImage>> ReadFilesAsync(string path)
        {
            var response = new Response<List<DriveImage>>();

            using (FtpClient ftp = new FtpClient(_ftpServer,
                    new NetworkCredential
                    {
                        UserName = _ftpUsername,
                        Password = _ftpPassword
                    }))
            {
                FtpListItem[] ftpListItems = ftp.GetListing(path);
               var images = new List<DriveImage>();

                foreach (var ftpListItem in ftpListItems)
                {
                    if (ftpListItem.Type == FtpFileSystemObjectType.File)
                    {
                        DriveImage node = new DriveImage(path, ftpListItem.Name, ftpListItem.FullName, ftpListItem.Size, DateTime.Now);
                        //node.Data = GetBase64String(ftpListItem.FullName);
                        images.Add(node);
                    }
                }

                ftp.Disconnect();

                response.IsSuccess = true;
                response.Result = images;
            }

            return response;
        }

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
    }
}
