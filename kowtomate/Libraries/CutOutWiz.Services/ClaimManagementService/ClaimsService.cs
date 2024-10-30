using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.ClaimManagementService
{
	public class ClaimsService : IClaimsService
	{
		private readonly AuthenticationStateProvider _authenticationStateProvider;

		public ClaimsService(AuthenticationStateProvider authenticationStateProvider)
		{
			_authenticationStateProvider = authenticationStateProvider;
		}

		public async Task<List<Claim>> GetClaimsAsync()
		{
			var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;

			if (user.Identity.IsAuthenticated)
			{
				return user.Claims.ToList();
			}

			return new List<Claim>();
		}
	}
}
