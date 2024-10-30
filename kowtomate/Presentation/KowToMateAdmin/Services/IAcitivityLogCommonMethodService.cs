using KowToMateAdmin.Models;
using KowToMateAdmin.Models.Security;

namespace KowToMateAdmin.Services
{
	public interface IAcitivityLogCommonMethodService
	{
		Task InsertErrorActivityLog(int orderId, string methodName, string errorMessage, byte category, LoginUserInfoViewModel loginUser, string razorPage = null);
		Task InsertErrorActivityLog(CommonActivityLogViewModel model);
	}
}
