using CutOutWiz.Core;
using CutOutWiz.Services.Models.HR;
using CutOutWiz.Services.BaseInterface;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Data;

namespace CutOutWiz.Services.HR
{
	public class EmployeeProfileService : IEmployeeProfileService
	{
		private readonly ISqlDataAccess _db;

		public EmployeeProfileService(ISqlDataAccess db)
		{
			_db = db;
		}
		public async Task<Response<bool>> Delete(string objectId)
		{
			var response = new Core.Response<bool>();

			try
			{
				await _db.SaveDataUsingProcedure(storedProcedure: "dbo.sp_hr_employeeProfile_deleteByid", new { id = objectId });
				response.IsSuccess = true;
				response.Message = StandardDataAccessMessages.SuccessMessaage;
			}
			catch (Exception ex)
			{
				response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
			}

			return response;
		}

		//public async Task<List<EmployeeProfile>> GetAll()
		//{
		//	return await _db.LoadDataUsingProcedure<EmployeeProfile, dynamic>(storedProcedure: "dbo.sp_hr_employeeProfile_getAll", new { });
		//}

        public async Task<List<EmployeeProfile>> GetAll<EmployeeProfile>() where EmployeeProfile : class
        {
            return await _db.LoadDataUsingProcedure<EmployeeProfile, dynamic>(storedProcedure: "dbo.sp_hr_employeeProfile_getAll", new { });
        }

        public async Task<EmployeeProfileModel> GetById(int id)
		{
			var result = await _db.LoadDataUsingProcedure<EmployeeProfileModel, dynamic>(storedProcedure: "dbo.sp_hr_employeeProfile_getById", new { Id = id });
			return result.FirstOrDefault();
		}

		public async Task<EmployeeProfileModel> GetByObjectId(string objectId)
		{
			throw new NotImplementedException();
		}

		public async Task<Response<int>> Insert(EmployeeProfileModel entity)
		{
			var response = new Core.Response<int>();
			try
			{
				var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.sp_hr_employeeProfile_insert", new
				{
					entity.ContactId,
					entity.MonthlySalary,
					entity.YearlyBonus,
					entity.ShiftId,
					entity.DayOffMonday,
					entity.DayOffTuesday,
					entity.DayOffWednesday,
					entity.DayOffThursday,
					entity.DayOffFriday,
					entity.DayOffSaturday,
					entity.DayOffSunday,
					entity.Gender,
					entity.DateOfBirth,
					entity.FullAddress,
					entity.HireDate,
				});

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

		public async Task<Response<bool>> Update(EmployeeProfileModel entity)
		{
			var response = new Core.Response<bool>();

			try
			{
				await _db.SaveDataUsingProcedure(storedProcedure: "dbo.sp_hr_employeeProfile_update", new
				{
					entity.Id,
					entity.ContactId,
					entity.MonthlySalary,
					entity.YearlyBonus,
					entity.ShiftId,
					entity.DayOffMonday,
					entity.DayOffTuesday,
					entity.DayOffWednesday,
					entity.DayOffThursday,
					entity.DayOffFriday,
					entity.DayOffSaturday,
					entity.DayOffSunday,
					entity.Gender,
					entity.DateOfBirth,
					entity.FullAddress,
					entity.HireDate,
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

    }
}
