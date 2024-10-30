using CutOutWiz.Services.Models.Security;
using CutOutWiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Security
{
	public interface ISecurityLoginHistoryService
	{
		Task<Response<int>> Insert(SecurityLoginHistoryModel securityLoginHistory);
		Task<Response<int>> Update(SecurityLoginHistoryModel securityLoginHistory);
		Task<List<SecurityLoginHistoryViewModel>> GetAll();
	}
}
