using CutOutWiz.Data;
using CutOutWiz.Data.HR;
using CutOutWiz.Data.Security;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.HR
{
    public class DesignationService : IDesignationService
    {
        private readonly ISqlDataAccess _db;

        public DesignationService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Designations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Designation>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<Designation, dynamic>(storedProcedure: "dbo.SP_HR_Designation_GetAll", new { });
        }

        /// <summary>
        /// Get designation by designation Id
        /// </summary>
        /// <param name="DesignationId"></param>
        /// <returns></returns>
        public async Task<Designation> GetById(int designationId)
        {
            var result = await _db.LoadDataUsingProcedure<Designation, dynamic>(storedProcedure: "dbo.SP_HR_Designation_GetById", new { DesignationId = designationId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="DesignationId"></param>
        /// <returns></returns>
        public async Task<Designation> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<Designation, dynamic>(storedProcedure: "dbo.SP_HR_Designation_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert designation
        /// </summary>
        /// <param name="designation"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(Designation designation)
        {
            var response = new Data.Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_HR_Designation_Insert", new
                {
                    designation.Name,
                    designation.Status,
                    designation.CreatedByContactId,
                    designation.ObjectId
                });

                designation.Id = newId;
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
        /// Update Designation
        /// </summary>
        /// <param name="designation"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(Designation designation)
        {
            var response = new Data.Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_HR_Designation_Update", new
                {
                    designation.Id,
                    designation.Name,
                    designation.Status,
                    designation.UpdatedByContactId
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
        /// Delete Designation by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Data.Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_HR_Designation_Delete", new { ObjectId = objectId });
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
