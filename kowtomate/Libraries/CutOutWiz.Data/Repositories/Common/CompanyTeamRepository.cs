
using CutOutWiz.Core;
using CutOutWiz.Data.DbAccess;
using CutOutWiz.Data.DTOs.Common;
using CutOutWiz.Data.Entities.Common;
using CutOutWiz.Data.Models.Common;

namespace CutOutWiz.Data.Repositories.Common
{
    public class CompanyTeamRepository : ICompanyTeamRepository
    {
        private readonly ISqlDataAccess _db;

        public CompanyTeamRepository(ISqlDataAccess db)
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
        /// Get company by company Id
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public async Task<CompanyEntity> GetById(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<CompanyEntity, dynamic>(storedProcedure: "dbo.SP_Common_Company_GetById", new { CompanyId = companyId });
            return result.FirstOrDefault();
        }

        public async Task<CompanyTeamEntity> GetTeamByCompanyId(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<CompanyTeamEntity, dynamic>(storedProcedure: "dbo.SP_Common_CompanyTeam_GetTeamByCompanyId", new { CompanyId = companyId });
            return result.FirstOrDefault();
        }

        public async Task<List<CompanyTeamEntity>> GetByCompanyId(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<CompanyTeamEntity, dynamic>(storedProcedure: "dbo.SP_Common_CompanyTeam_GetByCompanyId", new { companyId = companyId });
            return result;
        }

        public async Task<List<CompanyTeamEntity>> GetCompanyTeamByCompanyId(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<CompanyTeamEntity, dynamic>(storedProcedure: "dbo.SP_Common_CompanyTeam_GetByCompanyId", new { CompanyId = companyId });
            return result;
        }

        /// <summary>
        /// Insert company
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(List<CompanyTeamEntity> companyTeams)
        {
            var count = 0;
            var response = new Core.Response<int>();
            try
            {
                foreach (var companyTeam in companyTeams)
                {
                    if (count <= 0)
                    {
                        await Delete(companyTeam.CompanyId);
                    }
                    count++;
                    var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Common_CompanyTeam_Insert", new
                    {
                        companyTeam.CompanyId,
                        companyTeam.TeamId,
                        companyTeam.Status,
                        companyTeam.CreatedByContactId,
                        companyTeam.ObjectId

                    });
                }
                //companyTeam.Id = newId;
                //response.Result = newId;
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
        public async Task<Response<bool>> Update(CompanyTeamEntity company)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_CompanyTeam_Update", new
                {
                    company.TeamId,
                    company.CompanyId,
                    company.UpdatedByContactId,

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
        public async Task<Response<bool>> Delete(int companyId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Common_CompanyTeam_Delete", new { CompanyId = companyId });
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
    }
}
