using CutOutWiz.Services.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Helper
{
    public interface IFileConvertionService
    {
        /// <summary>
        /// Convert Any type of file to PNG and Return Base64String
        /// </summary>
        /// <param name="fileServer"></param>
        /// <param name="ftpFilePath"></param>
        /// <returns>Base64String of image</returns>
        Task<string> ConvertFileToPNG(FileServerModel fileServer ,string ftpFilePath, string _webHostEnvironment);
    }
}
