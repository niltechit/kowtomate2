using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.WebApiService
{
    public interface IFileViewApiService
    {
        public string ConvertedFileAndGetOutputUrl(string filePath, FileServerModel fileServer, int width, int height);
    }
}
