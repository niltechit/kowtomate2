using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.ClaimManagementService
{
	public interface IClaimsService
	{
		Task<List<System.Security.Claims.Claim>> GetClaimsAsync();
	}
}
