
using CutOutWiz.Services.Models.FileUpload;
using FtpSharpLib.Core;

namespace CutOutWiz.Services.StorageService
{
    public interface IFtpSharpLibraryService
    {
        FtpClient CreateAsyncFtpClient(FileUploadModel model);
    }
}
