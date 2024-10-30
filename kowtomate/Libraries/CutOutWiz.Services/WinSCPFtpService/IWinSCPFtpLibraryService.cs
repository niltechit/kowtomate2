using CutOutWiz.Services.Models.FtpModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.WinSCPFtpService
{
    public interface IWinSCPFtpLibraryService
    {
        Task<List<string>> ListFilesRecursive(FtpCredentailsModel ftp,string remotePath);
    }
}
