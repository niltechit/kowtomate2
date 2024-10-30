using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core;
using CutOutWiz.Services.MapperHelper;
using CutOutWiz.Data.Repositories.Common;
using CutOutWiz.Data.Entities.Common;

namespace CutOutWiz.Services.Managers.Common
{
    public class ShiftManager : IShiftManager
    {
        private readonly IMapperHelperService _mapperHelperService;
        private readonly IShiftRepository _shiftRepository;

        public ShiftManager(IShiftRepository shiftRepository,
            IMapperHelperService mapperHelperService)
        {
            _shiftRepository = shiftRepository;
            _mapperHelperService = mapperHelperService;
        }

        public async Task<Response<int>> Insert(ShiftModel shiftModel)
        {
            //Add validation logic here
            var entity = await _mapperHelperService.MapToSingleAsync<ShiftModel, ShiftEntity>(shiftModel);
            return await _shiftRepository.Insert(entity);
        }

        public async Task<List<ShiftModel>> GetAll()
        {
            var entities = await _shiftRepository.GetAll();
            return await _mapperHelperService.MapToListAsync<ShiftEntity, ShiftModel>(entities);
        }
    }
}
