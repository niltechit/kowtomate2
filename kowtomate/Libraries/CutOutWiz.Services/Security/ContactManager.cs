using CutOutWiz.Core;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.MapperHelper;
using CutOutWiz.Data.Repositories.Security;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Services.Security
{
    public class ContactManager : IContactManager
    {
        private readonly IMapperHelperService _mapperHelperService;
        private readonly IContactRepository _contactRepository;

        public ContactManager(IContactRepository contactRepository,
            IMapperHelperService mapperHelperService)
        {
            _contactRepository = contactRepository;
            _mapperHelperService = mapperHelperService;
        }

        public async Task<List<ContactModel>> GetAll()
        {
            var entities = await _contactRepository.GetAll();
            return await _mapperHelperService.MapToListAsync<ContactEntity, ContactModel>(entities);
        }
        
        /// <summary>
        /// Getting Employees 
        /// </summary>
        /// <returns></returns>
        public async Task<List<ContactModel>> GetAllEmployee()
        {
            var entities = await _contactRepository.GetAllEmployee();
            return await _mapperHelperService.MapToListAsync<ContactEntity, ContactModel>(entities);
        }

        public async Task<List<ContactModel>> GetAllByTeamId(int teamId)
        {
            var entities = await _contactRepository.GetAllByTeamId(teamId);
            return await _mapperHelperService.MapToListAsync<ContactEntity, ContactModel>(entities);
        }

		public async Task<List<ContactModel>> GetAvailableTeamMembersByTeamId(int teamId)
		{
            var entities = await _contactRepository.GetAvailableTeamMembersByTeamId(teamId);
            return await _mapperHelperService.MapToListAsync<ContactEntity, ContactModel>(entities);
		}

        public async Task<List<ContactModel>> GetSupportMembersByTeamId(int teamId)
        {
            var entities = await _contactRepository.GetSupportMembersByTeamId(teamId);
            return await _mapperHelperService.MapToListAsync<ContactEntity, ContactModel>(entities);
        }

        public async Task<List<ContactModel>> GetTeamMembersWhoSupportAnotherByTeamId(int teamId)
        {
            var entities = await _contactRepository.GetTeamMembersWhoSupportAnotherByTeamId(teamId);
            return await _mapperHelperService.MapToListAsync<ContactEntity, ContactModel>(entities);
        }

        //public async Task<List<ContactModel>> GetAllIsSharedFolderEditorContact(int consoleAppId)
        //{
        //          try
        //          {
        //		var list = await _db.LoadDataUsingProcedure<ContactModel, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetAllIsSharedFolderEditorContact", new { ConsoleAppId = consoleAppId });
        //              return list;
        //	}
        //          catch (Exception ex)
        //          {
        //              return null;
        //          }
        //}
        public async Task<List<ContactModel>> GetAllIsSharedFolderEditorContact(int consoleAppId)
        {
            var entities = await _contactRepository.GetAllIsSharedFolderEditorContact(consoleAppId);
            return await _mapperHelperService.MapToListAsync<ContactEntity, ContactModel>(entities);
        }

        public async Task<List<ContactModel>> GetAllIsSharedFolderQcContact(int consoleAppId)
		{
            var entities = await _contactRepository.GetAllIsSharedFolderQcContact(consoleAppId);
            return await _mapperHelperService.MapToListAsync<ContactEntity, ContactModel>(entities);
		}

		public async Task<List<ContactListModel>> GetAllContactByCompanyObjectId(string objectId)
        {
			var entities = await _contactRepository.GetAllContactByCompanyObjectId(objectId);
			return await _mapperHelperService.MapToListAsync<ContactListEntity, ContactListModel>(entities);
        }

        public async Task<List<ContactModel>> GetAllContacts()
        {
            var entities = await _contactRepository.GetAllContacts();
            return await _mapperHelperService.MapToListAsync<ContactEntity, ContactModel>(entities);
        }

        /// <summary>
        /// Get ContactModel for dropdown
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<List<ContactForDropdown>> GetContactsForDropdownByCompanyId(int companyId)
        {
            var entities = await _contactRepository.GetContactsForDropdownByCompanyId(companyId);
			return await _mapperHelperService.MapToListAsync<ContactForDropdownEntity, ContactForDropdown>(entities);
		}

		public async Task<List<ContactForDropdown>> GetContactsForDropdownByTeamId(int teamId)
		{
			var entities = await _contactRepository.GetContactsForDropdownByTeamId(teamId);
			return await _mapperHelperService.MapToListAsync<ContactForDropdownEntity, ContactForDropdown>(entities);
		}

		/// <summary>
		/// Get All with Details
		/// </summary>
		/// <returns></returns>
		public async Task<List<ContactListModel>> GetListWithDetails()
        {
            var entities = await _contactRepository.GetListWithDetails();
			return await _mapperHelperService.MapToListAsync<ContactListEntity, ContactListModel>(entities);
		}

        public async Task<List<ContactListModel>> GetListWithDetails(int companyId)
        {
			var entities = await _contactRepository.GetListWithDetails(companyId);
			return await _mapperHelperService.MapToListAsync<ContactListEntity, ContactListModel>(entities);
		}

        public async Task<Response<int>> Insert(ContactModel contactModel)
        {
            var entity = await _mapperHelperService.MapToSingleAsync<ContactModel, ContactEntity>(contactModel);
            return await _contactRepository.Insert(entity);
        }

        public async Task<ContactModel> GetById(int ContactId)
        {
            var entity = await _contactRepository.GetById(ContactId);
            return await _mapperHelperService.MapToSingleAsync<ContactEntity, ContactModel>(entity);
        }

        public async Task<ContactModel> GetByObjectId(string objectId)
        {
            var entity = await _contactRepository.GetByObjectId(objectId);
            return await _mapperHelperService.MapToSingleAsync<ContactEntity, ContactModel>(entity);
        }

        public async Task<List<ContactModel>> GetByCompanyId(int companyId)
        {
            var entities = await _contactRepository.GetByCompanyId(companyId);
            return await _mapperHelperService.MapToListAsync<ContactEntity, ContactModel>(entities);
        }

        public async Task<Core.Response<int>> Update(ContactModel contactModel)
        {
            var entity = await _mapperHelperService.MapToSingleAsync<ContactModel, ContactEntity>(contactModel);
            return await _contactRepository.Update(entity);
        }

        public async Task<Response<int>> Delete(string objectId)
        {
            return await _contactRepository.Delete(objectId);
        }

        public async Task<bool> CheckEmployeeIdExist(string employeeId)
        {
            return await _contactRepository.CheckEmployeeIdExist(employeeId);
        }

		public async Task UpdateContactDownloadPath(ContactModel contactModel)
		{
            var entity = await _mapperHelperService.MapToSingleAsync<ContactModel, ContactEntity>(contactModel);
            await _contactRepository.UpdateContactDownloadPath(entity);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<ContactModel> GetByEmployeeId(string employeeId)
        {
            var entity = await _contactRepository.GetByEmployeeId(employeeId);
            return await _mapperHelperService.MapToSingleAsync<ContactEntity, ContactModel>(entity);
        }
    }
}
