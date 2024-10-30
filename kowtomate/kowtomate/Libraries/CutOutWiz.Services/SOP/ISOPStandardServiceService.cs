using CutOutWiz.Data;
using CutOutWiz.Data.SOP;

namespace CutOutWiz.Services.SOP
{
    public interface ISOPStandardServiceService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<SOPStandardService>> GetAll();
        Task<SOPStandardService> GetById(int standardServiceId);
        Task<SOPStandardService> GetByObjectId(string objectId);
        Task<Response<int>> Insert(SOPStandardService standardService);
        Task<Response<bool>> Update(SOPStandardService standardService);
    }
}
