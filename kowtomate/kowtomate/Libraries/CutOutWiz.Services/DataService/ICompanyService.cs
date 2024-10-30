using CutOutWiz.Data;

namespace CutOutWiz.Services.DataService
{
    public interface ICompanyService
    {
        public Task<IEnumerable<Company>> GetAllCompany();
        public Task InsertCompany(Company company);
        public Task<int> UpdateCompany(Company company);
        public Task<Company> GetCompanyById(int CompanyId);
        public Task DeleteCompany(int companyId);
        Task<IEnumerable<Company>> GetFolderByFolderName(string folderName);
    }
}