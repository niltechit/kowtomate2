using CutOutWiz.Core;
using CutOutWiz.Data;
using CutOutWiz.Data.DbAccess;
using CutOutWiz.Data.Entities.Accounting;

namespace CutOutWiz.Data.Repositories.Accounting
{
    public class OverheadCostRepository : IOverheadCostRepository
	{
        private readonly ISqlDataAccess _db;

        public OverheadCostRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<Response<int>> Delete(int Id)
        {
            var response = new Core.Response<int>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Accounting_OverheadCost_Delete", new { Id = Id });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<List<OverheadCostListDto>> GetAll()
        {
            var result = await _db.LoadDataUsingProcedure<OverheadCostListDto, dynamic>(storedProcedure: "dbo.SP_Accounting_OverheadCost_GetAll", new { });
            return result.ToList();
        }

        public async Task<OverheadCostEntity> GetById(int id)
        {
            var response = new OverheadCostEntity(); 

            try
            {
                var result = await _db.LoadDataUsingProcedure<OverheadCostEntity, dynamic>(storedProcedure: "dbo.SP_Accounting_OverheadCost_GetById", new { id = id });

                response = result.FirstOrDefault();

            }
            catch (Exception ex)
            {
                response = response;
            }

            return response;
        }

        public async Task<Response<int>> Insert(OverheadCostEntity cost)
        {
            var response = new Core.Response<int>();
          
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Accounting_OverheadCost_Insert", new
                {
                    cost.Amount,
                    cost.Month,
                    cost.Year,
                    cost.CreatedByContactId,
                    cost.CreatedDate,

                });
                response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
                

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<int>> Update(OverheadCostEntity cost)
        {
            var response = new Core.Response<int>();

            try
            {
               await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Accounting_OverheadCost_Update", new
                {
                    cost.Id,
                    cost.Amount,
                    cost.Month,
                    cost.Year,
                    cost.UpdatedDate,
                    cost.UpdatedByContactId,

                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;


            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }
    }
}
