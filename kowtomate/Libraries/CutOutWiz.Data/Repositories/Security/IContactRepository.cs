using CutOutWiz.Core;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Data.Repositories.Security
{
    public interface IContactRepository
    {
        Task<Response<int>> Delete(string objectId);
        Task<List<ContactEntity>> GetAll();
        /// <summary>
        /// Getting Employee where employee id is not null
        /// </summary>
        /// <returns></returns>
        Task<List<ContactEntity>> GetAllEmployee();
        Task<List<ContactEntity>> GetAllByTeamId(int teamId);
        Task<List<ContactListEntity>> GetAllContactByCompanyObjectId(string objectId);
        Task<List<ContactEntity>> GetAllContacts();
        Task<ContactEntity> GetById(int ContactId);
        Task<ContactEntity> GetByObjectId(string objectId);
        Task<List<ContactEntity>> GetByCompanyId(int CompanyId);
        Task<List<ContactListEntity>> GetListWithDetails();
        Task<List<ContactListEntity>> GetListWithDetails(int companyId);
        Task<Response<int>> Insert(ContactEntity contact);
        Task<Response<int>> Update(ContactEntity contact);
        Task<bool> CheckEmployeeIdExist(string employeeId);
        /// <summary>
        /// Get ContactEntity for dropdown
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<List<ContactForDropdownEntity>> GetContactsForDropdownByCompanyId(int companyId);
        Task<List<ContactForDropdownEntity>> GetContactsForDropdownByTeamId(int teamId);
		Task UpdateContactDownloadPath(ContactEntity contact);
        Task<List<ContactEntity>> GetAllIsSharedFolderEditorContact(int consoleAppId);
        Task<List<ContactEntity>> GetAllIsSharedFolderQcContact(int consoleAppId);
        Task<List<ContactEntity>> GetAvailableTeamMembersByTeamId(int teamId);
        Task<List<ContactEntity>> GetSupportMembersByTeamId(int teamId);
        Task<List<ContactEntity>> GetTeamMembersWhoSupportAnotherByTeamId(int teamId);
        /// <summary>
        /// Get ContactEntity by EmployeeId
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        Task<ContactEntity> GetByEmployeeId(string employeeId);
    }
}