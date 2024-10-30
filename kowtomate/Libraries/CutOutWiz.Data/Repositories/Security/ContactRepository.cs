using CutOutWiz.Core;
using CutOutWiz.Data.DbAccess;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Data.Repositories.Security
{
    public class ContactRepository : IContactRepository
    {
        private readonly ISqlDataAccess _db;

        public ContactRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<List<ContactEntity>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<ContactEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetAll", new { });
        }/// <summary>
        /// Getting Employees 
        /// </summary>
        /// <returns></returns>
        public async Task<List<ContactEntity>> GetAllEmployee()
        {
            return await _db.LoadDataUsingProcedure<ContactEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetByEmployee", new { });
        }

        public async Task<List<ContactEntity>> GetAllByTeamId(int teamId)
        {
            return await _db.LoadDataUsingProcedure<ContactEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetAllByTeamId", new {teamId = teamId });
        }
		public async Task<List<ContactEntity>> GetAvailableTeamMembersByTeamId(int teamId)
		{
			return await _db.LoadDataUsingProcedure<ContactEntity, dynamic>(storedProcedure: "dbo.SP_GetAvailableTeamMembersForProductionByTeamId", new { teamId = teamId });
		}

        public async Task<List<ContactEntity>> GetSupportMembersByTeamId(int teamId)
        {
            return await _db.LoadDataUsingProcedure<ContactEntity, dynamic>(storedProcedure: "dbo.SP_GetAvailableSupportMembersForProductionByTeamId", new { teamId = teamId });
        }

        public async Task<List<ContactEntity>> GetTeamMembersWhoSupportAnotherByTeamId(int teamId)
        {
            return await _db.LoadDataUsingProcedure<ContactEntity, dynamic>(storedProcedure: "dbo.SP_GetTeamMembersWhoSupportAnotherTeamByTeamId", new { teamId = teamId });
        }

        //public async Task<List<ContactEntity>> GetAllIsSharedFolderEditorContact(int consoleAppId)
        //{
        //          try
        //          {
        //		var list = await _db.LoadDataUsingProcedure<ContactEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetAllIsSharedFolderEditorContact", new { ConsoleAppId = consoleAppId });
        //              return list;
        //	}
        //          catch (Exception ex)
        //          {
        //              return null;
        //          }
        //}
        public async Task<List<ContactEntity>> GetAllIsSharedFolderEditorContact(int consoleAppId)
        {
            try
            {
                var list = await _db.LoadDataUsingProcedure<ContactEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetAllIsSharedFolderEditorContact", new { ConsoleAppId = consoleAppId });
               // string query = "SELECT distinct sc.* \r\n\t \r\n\t FROM CompanyGeneralSettings cgs\r\n\r\n\tinner join Common_CompanyTeam cct WITH(NOLOCK) on cct.CompanyId = cgs.CompanyId\r\n\tinner join Management_Team mt WITH(NOLOCK) on mt.Id=cct.TeamId\r\n\tinner join Management_TeamMember mtm WITH(NOLOCK) on mtm.TeamId=mt.Id\r\n\tinner join Security_Contact sc WITH(NOLOCK) on sc.Id=mtm.ContactId and sc.isSharedFolderEnable = 1\r\n\tinner join Order_AssignedImageEditor oie WITH(NOLOCK) on sc.Id=oie.AssignContactId\r\n\tinner join Order_ClientOrderItem oci WITH(NOLOCK) on oci.Id=oie.Order_ImageId and  oci.Status IN (7,8,11,12)\r\n\r\n\twhere cgs.FtpOrderPlacedAppNo=@ConsoleApp and cgs.AllowAutoUploadFromEditorPc = 1 ";
                //var list = await _db.LoadDataUsingQuery<ContactEntity,dynamic>(query, new { ConsoleApp = consoleAppId });
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ContactEntity>> GetAllIsSharedFolderQcContact(int consoleAppId)
		{
			return await _db.LoadDataUsingProcedure<ContactEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetAllIsSharedFolderQcContact", new { ConsoleAppId = consoleAppId });
		}

		public async Task<List<ContactListEntity>> GetAllContactByCompanyObjectId(string objectId)
        {
            return await _db.LoadDataUsingProcedure<ContactListEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetAllByCompanyObjectId", new {ObjectId=objectId });
        }

        public async Task<List<ContactEntity>> GetAllContacts()
        {
            return await _db.LoadDataUsingProcedure<ContactEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetAllContacts", new { });
        }


        /// <summary>
        /// Get ContactEntity for dropdown
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<List<ContactForDropdownEntity>> GetContactsForDropdownByCompanyId(int companyId)
        {
            var query = "SELECT Id, (FirstName + ' ' + LastName) FullName FROM Security_Contact WITH(NOLOCK) WHERE CompanyId = @CompanyId";
            return await _db.LoadDataUsingQuery<ContactForDropdownEntity, dynamic>(query, new { CompanyId = companyId });
        }

		public async Task<List<ContactForDropdownEntity>> GetContactsForDropdownByTeamId(int teamId)
		{
			var query = "SELECT sc.Id, (sc.FirstName + ' ' + sc.LastName) FullName FROM Security_Contact sc WITH(NOLOCK) inner join Management_TeamMember mt on mt.ContactId = sc.Id  WHERE mt.TeamId = @TeamId";
			return await _db.LoadDataUsingQuery<ContactForDropdownEntity, dynamic>(query, new { TeamId = teamId });
		}

		/// <summary>
		/// Get All with Details
		/// </summary>
		/// <returns></returns>
		public async Task<List<ContactListEntity>> GetListWithDetails()
        {
            return await _db.LoadDataUsingProcedure<ContactListEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetListWithDetails", new { });
        }
        public async Task<List<ContactListEntity>> GetListWithDetails(int companyId)
        {
            return await _db.LoadDataUsingProcedure<ContactListEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetListWithDetailsForCompanyWise", new {companyId=companyId });
        }
        public async Task<Response<int>> Insert(ContactEntity contact)
        {
            var response = new Response<int>();
            var employeeId = contact.EmployeeId;

            //bool isExist = await isEmployeeIdDublicate(contact.EmployeeId);
            try
            {
                //if (isExist)
                //{
                //    response.Message = "Duplicate EmployeeId is not eligible";
                //}
                //else
                //{

                    var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Security_Contact_Insert", new
                    {
                        contact.CompanyId,
                        contact.FirstName,
                        contact.LastName,
                        contact.DesignationId,
                        contact.Email,
                        contact.Phone,
                        contact.ProfileImageUrl,
                        contact.Status,
                        contact.CreatedByContactId,
                        contact.ObjectId,
                        contact.EmployeeId
                        
                    });
                    response.Result = newId;
                    response.IsSuccess = true;
                    response.Message = StandardDataAccessMessages.SuccessMessaage;
                //}

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }
          
           return response;
        }

        public async Task<ContactEntity> GetById(int ContactId)
        {
            var result = await _db.LoadDataUsingProcedure<Data.Security.ContactEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetById", new { ContactId = ContactId });
            return result.FirstOrDefault();
        }

        public async Task<ContactEntity> GetByObjectId(string ObjectId)
        {
            var result = await _db.LoadDataUsingProcedure<ContactEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetByObjectId", new { ObjectId = ObjectId });
            return result.FirstOrDefault();
        }

        public async Task<List<ContactEntity>> GetByCompanyId(int CompanyId)
        {
            var result = await _db.LoadDataUsingProcedure<ContactEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetByCompanyId", new { CompanyId = CompanyId });
            return result.ToList();
        }

        public async Task<Response<int>> Update(ContactEntity contact)
        {
            var response = new Response<int>();
            try
            {               
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_Contact_Update", new
                {
                    contact.ObjectId,
                    contact.FirstName,
                    contact.LastName,
                    contact.DesignationId,
                    contact.Email,
                    contact.Phone,
                    contact.Status,
                    contact.UpdatedByContactId,
                    contact.EmployeeId
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

        public async Task<Response<int>> Delete(string objectId)
        {
            var response = new Response<int>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_Contact_Delete", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<bool> CheckEmployeeIdExist(string employeeId)
        {
            var query = "select Id from [dbo].[Security_Contact] where employeeId = @employeeId";
            var existingContacts = await _db.LoadDataUsingQuery<int, dynamic>(query, new { EmployeeId = employeeId });

            if (existingContacts != null && existingContacts.Any())
            {
                return true;
            }
            
            return false;
        }

		public async Task UpdateContactDownloadPath(ContactEntity contact)
		{
			var response = new Response<int>();
			try
			{
				// Add Order_ClientOrder
				var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_User_UpdateUserDownloadFolderPath", new
				{
					contact.DownloadFolderPath,
					contact.Id,
                    contact.IsUserActive,
                    contact.IsSharedFolderEnable
				});

			}
			catch (Exception ex)
			{
				response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
			}
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<ContactEntity> GetByEmployeeId(string employeeId)
        {
            try
            { 
                var result = await _db.LoadDataUsingProcedure<ContactEntity, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetByEmployeeId", new { employeeId = employeeId });
                return result.FirstOrDefault();
            }
            catch (Exception ex) 
            {
                return null;
            }
        }
    }
}
