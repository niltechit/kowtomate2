using CutOutWiz.Data;
using CutOutWiz.Data.Common;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Common
{
    public class CountryService:ICountryService
    {
        private readonly ISqlDataAccess _db;

        public CountryService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Countrys
        /// </summary>
        /// <returns></returns>
        public async Task<List<Country>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<Country, dynamic>(storedProcedure: "dbo.SP_Common_Country_GetAll", new { });
        }

        /// <summary>
        /// Get country by country Id
        /// </summary>
        /// <param name="CountryId"></param>
        /// <returns></returns>
        public async Task<Country> GetById(int countryId)
        {
            var result = await _db.LoadDataUsingProcedure<Country, dynamic>(storedProcedure: "dbo.SP_Common_Country_GetById", new { CountryId = countryId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="CountryId"></param>
        /// <returns></returns>
        public async Task<Country> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<Country, dynamic>(storedProcedure: "dbo.SP_Common_Country_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert country
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(Country country)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Common_Country_Insert", new
                {
                    country.Name,
                    country.Code,
                    country.CreatedByContactId,
                    country.ObjectId
                });

                country.Id = newId;
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
        /// Update Country
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(Country country)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_Country_Update", new
                {
                    country.Id,
                    country.Name,
                    country.Code,
                    country.UpdatedByContactId
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
        /// Delete Country by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_Country_Delete", new { ObjectId = objectId });
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
