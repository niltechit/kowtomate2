using CutOutWiz.Core;
using CutOutWiz.Data.DbAccess;
using CutOutWiz.Data.DTOs.Common;
using CutOutWiz.Data.Models.Common;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Data.Repositories.Common
{
    public class CompanyRepository : ICompanyRepository
	{
        private readonly ISqlDataAccess _db;

        public CompanyRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Companys
        /// </summary>
        /// <returns></returns>
        public async Task<List<CompanyEntity>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<CompanyEntity, dynamic>(storedProcedure: "dbo.SP_Common_Company_GetAll", new { });
        }

        /// <summary>
        /// Get All Companys
        /// </summary>
        /// <returns></returns>
        public async Task<List<CompanyEntity>> GetAllClientCompany()
        {
            return await _db.LoadDataUsingProcedure<CompanyEntity, dynamic>(storedProcedure: "dbo.SP_Common_Company_GetAllClientCompany", new { });
        }

        /// <summary>
        /// Get company by company Id
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public async Task<CompanyEntity> GetById(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<CompanyEntity, dynamic>(storedProcedure: "dbo.SP_Common_Company_GetById", new { CompanyId = companyId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get company by company Code
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public async Task<CompanyEntity> GetByCompanyCode(string code)
        {
            var query = $"SELECT * FROM [dbo].[Common_Company] WITH(NOLOCK) WHERE Code = @Code";
            var result = await _db.LoadDataUsingQuery<CompanyEntity, dynamic>(query, new { Code = code });
            return result.FirstOrDefault();
        }
        public async Task<CompanyEntity> GetByCompanyName(string Name)
        {
            var query = $"SELECT * FROM [dbo].[Common_Company] WITH(NOLOCK) WHERE Name = @Name";
            var result = await _db.LoadDataUsingQuery<CompanyEntity, dynamic>(query, new { Name = Name });
            return result.FirstOrDefault();
        }
        public async Task<CompanyEntity> GetByCompanyEmail(string Email)
        {
            var query = $"SELECT * FROM [dbo].[Common_Company] WITH(NOLOCK) WHERE Email = @Email";
            var result = await _db.LoadDataUsingQuery<CompanyEntity, dynamic>(query, new { Email = Email });
            return result.FirstOrDefault();
        }

        public async Task<List<CompanyEntity>> GetCompaniesById(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<CompanyEntity, dynamic>(storedProcedure: "dbo.SP_Common_Company_GetById", new { CompanyId = companyId });
            return result.ToList();
        } 
        public async Task<CompanyEntity> GetCompanyByCode(string code)
        {
            var result = await _db.LoadDataUsingProcedure<CompanyEntity, dynamic>(storedProcedure: "dbo.SP_Common_Company_GetByCode", new { Code = code });
            return result.FirstOrDefault();
        }
        public async Task<RoleEntity> GetRoleByCompanyObjectId(string companyObjectId)
        {
            var result = await _db.LoadDataUsingProcedure<RoleEntity, dynamic>(storedProcedure: "dbo.SP_Common_Company_GetRoleByCompanyObjectId", new { companyObjectId = companyObjectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public async Task<CompanyEntity> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<CompanyEntity, dynamic>(storedProcedure: "dbo.SP_Common_Company_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert company
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(CompanyEntity company)
        {
            var response = new Response<int>();
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
                    company.ObjectId,
                    company.FileServerId,
                    company.DeliveryDeadlineInMinute
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
        public async Task<Response<bool>> Update(CompanyEntity company)
        {
            var response = new Core.Response<bool>();

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
                    company.UpdatedByContactId,
                    company.FileServerId,
                    company.DeliveryDeadlineInMinute
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
        public async Task<Core.Response<bool>> Delete(string objectId)
        {
            var response = new Core.Response<bool>();

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

        /// <summary>
        /// Adding New Company for Signup page
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<int>> SignupInsertCompany(SignUpDto model)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Security_AddNewCompanyRoleAndPermission", new
                {
                    model.CreatedByContactId,
                    model.NewUserContactId,
                    model.BaseCompanyCode,
                    model.NewCompanyObjectId,
                    model.CompanyId,
                    //model.UserOBjectId,
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


        public async Task<List<CompanyEntity>> GetAllClientCompanyByQuery(string query)
        {
            var filteredList = await _db.LoadDataUsingQuery<CompanyEntity, dynamic>(query,
                    new
                    {
                    });
            return filteredList;
        }
    }
} 
