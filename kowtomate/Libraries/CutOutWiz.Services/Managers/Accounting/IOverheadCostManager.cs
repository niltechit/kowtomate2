using CutOutWiz.Core;
using CutOutWiz.Core.Models.Accounting;
using CutOutWiz.Services.Models.Accounting;

namespace CutOutWiz.Services.Managers.Accounting
{
    public interface IOverheadCostManager
    {
        Task<Response<int>> Insert(OverheadCostingModel cost);
        Task<Response<int>> Update(OverheadCostingModel cost);
        Task<Response<int>> Delete(int Id);
        Task<List<OverheadCostingListViewModel>> GetAll();
        Task<OverheadCostingModel> GetById(int id);
    }
}
