using CutOutWiz.Data;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Services.Security
{
    public interface IContactService
    {
        Task<Response<int>> Delete(string objectId);
        Task<List<Data.Security.Contact>> GetAll();
        Task<Data.Security.Contact> GetById(int ContactId);
        Task<Data.Security.Contact> GetByObjectId(string objectId);
        Task<List<ContactListModel>> GetListWithDetails();
        Task<Response<int>> Insert(Data.Security.Contact contact);
        Task<Response<int>> Update(Data.Security.Contact contact);
    }
}