using CutOutWiz.Core;
using CutOutWiz.Core.Management;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Data;

namespace CutOutWiz.Services.Security
{
	public class SecurityLoginHistoryService : ISecurityLoginHistoryService
	{
		private readonly ISqlDataAccess _db;

		public SecurityLoginHistoryService(ISqlDataAccess db)
		{
			_db = db;
		}

		public async Task<List<SecurityLoginHistoryViewModel>> GetAll()
		{
            var result= await _db.LoadDataUsingProcedure<SecurityLoginHistoryViewModel, dynamic>(storedProcedure: "dbo.SP_Security_GetSecurityLoginHistories", new { });
			return result.ToList();
        }

		public async Task<Response<int>> Insert(SecurityLoginHistoryModel securityLoginHistory)
		{
			var response = new Response<int>();

			try
			{
				var newId = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.SP_Security_InsertSecurityLoginHistory", new
				{
					securityLoginHistory.UserId,
					securityLoginHistory.Username,
					securityLoginHistory.ActionTime,
					securityLoginHistory.ActionType,
					securityLoginHistory.IPAddress,
					securityLoginHistory.DeviceInfo,
					securityLoginHistory.Status

				});

				securityLoginHistory.Id = (short)newId;
				response.Result = securityLoginHistory.Id;
				response.IsSuccess = true;
				response.Message = StandardDataAccessMessages.SuccessMessaage;

			}
			catch (Exception ex)
			{
				response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
			}

			return response;

		}

        public async Task<Response<int>> Update(SecurityLoginHistoryModel securityLoginHistory)
        {
            var response = new Response<int>();

            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.SP_Security_UpdateSecurityLoginHistoryById", new
                {
                    securityLoginHistory.Id,
                    securityLoginHistory.UserId,
                    securityLoginHistory.Username,
                    securityLoginHistory.ActionTime,
                    securityLoginHistory.ActionType,
                    securityLoginHistory.IPAddress,
                    securityLoginHistory.DeviceInfo,
                    securityLoginHistory.Status

                });

				response.Result = securityLoginHistory.Id;
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
