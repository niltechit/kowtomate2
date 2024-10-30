using CutOutWiz.Core;
using CutOutWiz.Services.Models.HR;

namespace CutOutWiz.Services.HR
{
    public interface ILeaveSubTypeService
    {
        Task<List<LeaveSubTypeModel>> GetAll();
        /// <summary>
        /// Get Sub leave type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<LeaveSubTypeModel> GetById(int id);
        Task<Response<int>> Insert(LeaveSubTypeModel leaveType);
        Task<Response<bool>> Update(LeaveSubTypeModel leaveType);
        Task<Response<bool>> Delete(int id);
        /// <summary>
        /// Get List of Sub leave types by leave type id
        /// </summary>
        /// <param name="leaveTypeId"></param>
        /// <returns></returns>
        Task<Response<List<LeaveSubTypeModel>>> GetSubLeaveTypes(int leaveTypeId);

    }
}
