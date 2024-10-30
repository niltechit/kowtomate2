using CutOutWiz.Core;
using CutOutWiz.Services.Models.HR;

namespace CutOutWiz.Services.HR
{
    public interface ILeaveTypeService
    {
        Task<List<LeaveTypeModel>> GetAll();
        Task<LeaveTypeModel> GetById(int leaveTypeId);
        Task<Response<int>> Insert(LeaveTypeModel leaveType);
        Task<Response<bool>> Update(LeaveTypeModel leaveType);
        Task<Response<bool>> Delete(int id);
    }
}
