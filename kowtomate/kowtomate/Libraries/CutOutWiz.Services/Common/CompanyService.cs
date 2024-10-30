using CutOutWiz.Data.Common;
using CutOutWiz.Data.Security;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Common
{
    public class CompanyService : ICompanyService
    {
        private readonly ISqlDataAccess _db;

        public CompanyService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Companys
        /// </summary>
        /// <returns></returns>
        public async Task<List<Company>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<Company, dynamic>(storedProcedure: "dbo.SP_Common_Company_GetAll", new { });
        }

        /// <summary>
        /// Get company by company Id
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public async Task<Company> GetById(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<Company, dynamic>(storedProcedure: "dbo.SP_Common_Company_GetById", new { CompanyId = companyId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public async Task<Company> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<Company, dynamic>(storedProcedure: "dbo.SP_Common_Company_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert company
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<Data.Response<int>> Insert(Company company)
        {
            var response = new Data.Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Common_Company_Insert", new
                {
                    company.Name,
                    company.Code,
                    company.CompanyType,
                    company.Telephone,
                    company.Email,
                    company.Address1,
                    company.Address2,
                    company.City,
                    company.State,
                    company.Zipcode,
                    company.Country,
                    company.Status,
                    company.CreatedByContactId,
                    company.ObjectId
                });

                company.Id = newId;
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
        /// Update Company
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<Data.Response<bool>> Update(Company company)
        {
            var response = new Data.Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_Company_Update", new
                {
                    company.Id,
                    company.Name,
                    company.Code,
                    company.CompanyType,
                    company.Telephone,
                    company.Email,
                    company.Address1,
                    company.Address2,
                    company.City,
                    company.State,
                    company.Zipcode,
                    company.Country,
                    company.Status,
                    company.UpdatedByContactId
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
        /// Delete Company by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Data.Response<bool>> Delete(string objectId)
        {
            var response = new Data.Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_Company_Delete", new { ObjectId = objectId });
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
