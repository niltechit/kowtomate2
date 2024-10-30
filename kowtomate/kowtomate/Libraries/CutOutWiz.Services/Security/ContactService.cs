using CutOutWiz.Data.Security;
using CutOutWiz.Services.DbAccess;

namespace CutOutWiz.Services.Security
{
    public class ContactService : IContactService
    {
        private readonly ISqlDataAccess _db;

        public ContactService(ISqlDataAccess db)
        {
            _db = db;
        }
        public async Task<List<Contact>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<Contact, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetAll", new { });
        }

        /// <summary>
        /// Get All with Details
        /// </summary>
        /// <returns></returns>
        public async Task<List<ContactListModel>> GetListWithDetails()
        {
            return await _db.LoadDataUsingProcedure<ContactListModel, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetListWithDetails", new { });
        }

        public async Task<Data.Response<int>> Insert(Contact contact)
        {
            var response = new Data.Response<int>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_Contact_Insert", new
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
                    contact.ObjectId
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

        public async Task<Contact> GetById(int ContactId)
        {
            var result = await _db.LoadDataUsingProcedure<Data.Security.Contact, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetById", new { ContactId = ContactId });
            return result.FirstOrDefault();
        }

        public async Task<Contact> GetByObjectId(string ObjectId)
        {
            var result = await _db.LoadDataUsingProcedure<Contact, dynamic>(storedProcedure: "dbo.SP_Security_Contact_GetByObjectId", new { ObjectId = ObjectId });
            return result.FirstOrDefault();
        }

        public async Task<Data.Response<int>> Update(Contact contact)
        {
            var response = new Data.Response<int>();

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
                    contact.UpdatedByContactId
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

        public async Task<Data.Response<int>> Delete(string objectId)
        {
            var response = new Data.Response<int>();
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

    }
}
