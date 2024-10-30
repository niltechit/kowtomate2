using CutOutWiz.Core;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Security
{
    public interface IContactManager
    {
        Task<Response<int>> Delete(string objectId);
        Task<List<ContactModel>> GetAll();
        /// <summary>
        /// Getting Employee where employee id is not null
        /// </summary>
        /// <returns></returns>
        Task<List<ContactModel>> GetAllEmployee();
        Task<List<ContactModel>> GetAllByTeamId(int teamId);
        Task<List<ContactListModel>> GetAllContactByCompanyObjectId(string objectId);
        Task<List<ContactModel>> GetAllContacts();
        Task<ContactModel> GetById(int ContactId);
        Task<ContactModel> GetByObjectId(string objectId);
        Task<List<ContactModel>> GetByCompanyId(int CompanyId);
        Task<List<ContactListModel>> GetListWithDetails();
        Task<List<ContactListModel>> GetListWithDetails(int companyId);
        Task<Response<int>> Insert(ContactModel contact);
        Task<Response<int>> Update(ContactModel contact);
        Task<bool> CheckEmployeeIdExist(string employeeId);
        /// <summary>
        /// Get ContactModel for dropdown
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<List<ContactForDropdown>> GetContactsForDropdownByCompanyId(int companyId);
        Task<List<ContactForDropdown>> GetContactsForDropdownByTeamId(int teamId);
		Task UpdateContactDownloadPath(ContactModel contact);
        Task<List<ContactModel>> GetAllIsSharedFolderEditorContact(int consoleAppId);
        Task<List<ContactModel>> GetAllIsSharedFolderQcContact(int consoleAppId);
        Task<List<ContactModel>> GetAvailableTeamMembersByTeamId(int teamId);
        Task<List<ContactModel>> GetSupportMembersByTeamId(int teamId);
        Task<List<ContactModel>> GetTeamMembersWhoSupportAnotherByTeamId(int teamId);
        /// <summary>
        /// Get ContactModel by EmployeeId
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        Task<ContactModel> GetByEmployeeId(string employeeId);
    }
}