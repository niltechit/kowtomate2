using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core;

namespace CutOutWiz.Services.Managers.Common
{
    public interface IShiftManager
    {
        Task<Response<int>> Insert(ShiftModel shift);
        Task<List<ShiftModel>> GetAll();
    }
}
