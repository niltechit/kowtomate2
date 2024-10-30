using CutOutWiz.Services.Models.FileUpload;
using FtpSharpLib.Core;
using FtpSharpLib.Core.Utility;
using System.Text;

namespace CutOutWiz.Services.StorageService
{
    public class FtpSharpLibraryService : IFtpSharpLibraryService
    {
        public FtpClient CreateAsyncFtpClient(FileUploadModel model)
        {
            var host = model.FtpUrl.Split(':');
            var config = new FtpConfig
            {
                Encoding = Encoding.UTF8,
                BufferSize = 20000,
            };
            if (host.Length == 3)
            {
                return new FtpClient($"{host[0]}:{host[1]}", model.userName, model.password, Convert.ToInt32(host[2]), config);
            }
            else
            {
                return new FtpClient(model.FtpUrl, model.userName, model.password,null,config);
            }
        }
    }
}
