using CutOutWiz.Data;
using CutOutWiz.Data.SOP;
using CutOutWiz.Services.DbAccess;

namespace CutOutWiz.Services.SOP
{
    public class SOPStandardServiceService : ISOPStandardServiceService
    {
        private readonly ISqlDataAccess _db;

        public SOPStandardServiceService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All StandardServices
        /// </summary>
        /// <returns></returns>
        public async Task<List<SOPStandardService>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<SOPStandardService, dynamic>(storedProcedure: "dbo.SP_SOP_StandardService_GetAll", new { });
        }

        /// <summary>
        /// Get standardService by standardService Id
        /// </summary>
        /// <param name="StandardServiceId"></param>
        /// <returns></returns>
        public async Task<SOPStandardService> GetById(int standardServiceId)
        {
            var result = await _db.LoadDataUsingProcedure<SOPStandardService, dynamic>(storedProcedure: "dbo.SP_SOP_StandardService_GetById", new { StandardServiceId = standardServiceId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="StandardServiceId"></param>
        /// <returns></returns>
        public async Task<SOPStandardService> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<SOPStandardService, dynamic>(storedProcedure: "dbo.SP_SOP_StandardService_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert standardService
        /// </summary>
        /// <param name="standardService"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(SOPStandardService standardService)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_SOP_StandardService_Insert", new
                {
                    standardService.Name,
                    standardService.SortOrder,
                    standardService.Status,
                    standardService.IsDeleted,
                    standardService.CreatedByContactId,
                    standardService.ObjectId,
                });

                standardService.Id = newId;
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

        /// <summary>
        /// Update StandardService
        /// </summary>
        /// <param name="standardService"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(SOPStandardService standardService)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_SOP_StandardService_Update", new
                {
                    standardService.Id,
                    standardService.Name,
                    standardService.SortOrder,
                    standardService.Status,
                    standardService.IsDeleted,
                    standardService.UpdatedByContactId
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

        /// <summary>
        /// Delete StandardService by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_SOP_StandardService_Delete", new { ObjectId = objectId });
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
