using CutOutWiz.Data;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.DataService
{
    public class CompanyService:ICompanyService
    {
        private readonly ISqlDataAccess _db;

        public CompanyService(ISqlDataAccess db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Company>> GetAllCompany()
        {
            return await _db.LoadDataUsingProcedure<Company, dynamic>(storedProcedure: "dbo.spCompany_GetAll", new { });
        }
        public async Task<IEnumerable<Company>> GetFolderByFolderName(string folderName)
        {
            return await _db.LoadDataUsingProcedure<Company, dynamic>(storedProcedure: "dbo.spCompanyByFolderName", new {FolderName =folderName });
        }

        public async Task InsertCompany(Company company)
        {
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.sp_Company_Insert", new
                {
                    company.Name,
                    company.Email,
                    company.Phone,
                    company.FileRootFolderPath,
                    company.Status,
                    company.CreatedByContactId,
                    company.CreatedDateUtc
                });
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
           
        }

        public async Task<Company> GetCompanyById(int CompanyId)
        {
            var result = await _db.LoadDataUsingProcedure<Company, dynamic>(storedProcedure: "dbo.sp_GetCompanyById", new { CompanyId = CompanyId });
            return result.FirstOrDefault();
        }

        public async Task<int> UpdateCompany(Company company)
        {
            try
            {
                var response = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.sp_Company_Update", new
                {
                    CompanyId = company.Id,
                    Name = company.Name,
                    Email = company.Email,
                    Phone = company.Phone,
                    FileRootFolderPath = company.FileRootFolderPath

                });
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }

        }


        public Task DeleteCompany(int companyId) =>
            _db.SaveDataUsingProcedure(storedProcedure: "dbo.spCompany_Delete", new { CompanyId = companyId });
    }
}
