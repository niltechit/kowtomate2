using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.StorageService
{
    public interface ILocalFileService
    {
        Task<bool> UploadAsync(string path,MemoryStream memoryStream);
        Task DeleteFiles(string FolderName);
    }
}
