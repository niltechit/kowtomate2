using CutOutWiz.Core;
using CutOutWiz.Services.Models.HR;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Data;

namespace CutOutWiz.Services.HR
{
    public class EmployeeLeaveService : IEmployeeLeaveService
    {
        private readonly ISqlDataAccess _db;

        public EmployeeLeaveService(ISqlDataAccess db)
        {
            _db = db;
        }
        public Task<Response<bool>> Delete(string objectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<EmployeeLeaveModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<EmployeeLeaveModel, dynamic>(storedProcedure: "dbo.sp_hr_employeeLeave_getAll", new { });
        }

        public async Task<List<EmployeeLeaveViewModel>> GetAll<EmployeeLeaveViewModel>() where EmployeeLeaveViewModel : class
        {
            return await _db.LoadDataUsingProcedure<EmployeeLeaveViewModel, dynamic>(storedProcedure: "dbo.sp_hr_employeeLeave_getAll", new { });
        }

        public async Task<EmployeeLeaveModel> GetById(int id)
        {
            var result =  await _db.LoadDataUsingProcedure<EmployeeLeaveModel, dynamic>(storedProcedure: "dbo.sp_EmployeeLeave_GetById", new {id=id });
            return result.FirstOrDefault();
        }

        public Task<EmployeeLeaveModel> GetByObjectId(string objectId)
        {
            throw new NotImplementedException();
        }

        public async Task<Response<int>> Insert(EmployeeLeaveModel entity)
        {
            var response = new Core.Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureWithGeneric<EmployeeLeaveModel>(storedProcedure: "dbo.sp_employee_leave_insert", entity,"default");

                entity.Id = newId;
                response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<bool>> Update(EmployeeLeaveModel entity)
        {
            var response = new Core.Response<bool>();
            try
            {
                var newId = await _db.UpdateDataUsingProcedureWithGeneric<EmployeeLeaveModel>(storedProcedure: "dbo.sp_employee_leave_update", entity, "default");

                response.Result = newId;
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
