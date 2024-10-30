using CutOutWiz.Core;
using CutOutWiz.Data.Entities.Accounting;

namespace CutOutWiz.Data.Repositories.Accounting
{
    public interface IOverheadCostRepository
	{
        Task<Response<int>> Insert(OverheadCostEntity cost);
        Task<Response<int>> Update(OverheadCostEntity cost);
        Task<Response<int>> Delete(int Id);
        Task<List<OverheadCostListDto>> GetAll();
        Task<OverheadCostEntity> GetById(int id);
    }
}
