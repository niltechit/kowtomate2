using CutOutWiz.Core;
using CutOutWiz.Core.Models.Accounting;
using CutOutWiz.Data;
using CutOutWiz.Data.Entities.Accounting;
using CutOutWiz.Data.Repositories.Accounting;
using CutOutWiz.Services.MapperHelper;
using CutOutWiz.Services.Models.Accounting;

namespace CutOutWiz.Services.Managers.Accounting
{
    public class OverheadCostManager : IOverheadCostManager
    {
		private readonly IMapperHelperService _mapperHelperService;
		private readonly IOverheadCostRepository _overheadCostRepository;

		public OverheadCostManager(IOverheadCostRepository overheadCostRepository,
			IMapperHelperService mapperHelperService)
		{
			_overheadCostRepository = overheadCostRepository;
			_mapperHelperService = mapperHelperService;
		}

        public async Task<Response<int>> Delete(int id)
        {
            var response = new Response<int>();
            
            if (id == 0)
            {
                response.Message = StandardDataAccessMessages.RequiredIdForDelete;
			}

            return await _overheadCostRepository.Delete(id);
        }

        public async Task<List<OverheadCostingListViewModel>> GetAll()
        {
			var entities = await _overheadCostRepository.GetAll();
			return await _mapperHelperService.MapToListAsync<OverheadCostListDto, OverheadCostingListViewModel>(entities);
        }

        public async Task<OverheadCostingModel> GetById(int id)
        {
			var entity = await _overheadCostRepository.GetById(id);
			return await _mapperHelperService.MapToSingleAsync<OverheadCostEntity, OverheadCostingModel>(entity);
        }

        public async Task<Response<int>> Insert(OverheadCostingModel overheadCostingModel)
        {
            var response = new Core.Response<int>();
          
            if (overheadCostingModel == null)
            {
                response.Message = "Overhead cost is null"; 
                return response;
            }

			var entity = await _mapperHelperService.MapToSingleAsync<OverheadCostingModel, OverheadCostEntity>(overheadCostingModel);
			return await _overheadCostRepository.Insert(entity); 
        }

        public async Task<Response<int>> Update(OverheadCostingModel overheadCostingModel)
        {
			var response = new Core.Response<int>();

			if (overheadCostingModel == null)
			{
				response.Message = "Overhead cost is null";
				return response;
			}

			var entity = await _mapperHelperService.MapToSingleAsync<OverheadCostingModel, OverheadCostEntity>(overheadCostingModel);
			return await _overheadCostRepository.Update(entity);
		}
    }
}
