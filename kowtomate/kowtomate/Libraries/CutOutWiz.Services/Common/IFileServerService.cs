using CutOutWiz.Data;
using CutOutWiz.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Common
{
    public interface IFileServerService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<FileServer>> GetAll();
        Task<FileServer> GetById(int fileServerId);
        Task<FileServer> GetByObjectId(string objectId);
        Task<Response<int>> Insert(FileServer fileServer);
        Task<Response<bool>> Update(FileServer fileServer);
    }
}
