
using CutOutWiz.Core;
using CutOutWiz.Data.Entities.Common;

namespace CutOutWiz.Data.Repositories.Common
{
    public interface IShiftRepository
	{
        Task<Response<int>> Insert(ShiftEntity shift);
        Task<List<ShiftEntity>> GetAll();
    }
}
