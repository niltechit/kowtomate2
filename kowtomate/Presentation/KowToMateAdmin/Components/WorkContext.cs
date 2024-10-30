using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Managers.Common;
using KowToMateAdmin.Models.Security;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace KowToMateAdmin
{
    public class WorkContext : IWorkContext
    {
        //private readonly I
        private readonly AuthenticationStateProvider _auth;
        private readonly ICompanyManager _companyService;
        private readonly IMemoryCache _memoryCache;
        private IConfiguration _configuration;

        public WorkContext(IConfiguration configuration,
            AuthenticationStateProvider auth,
            ICompanyManager companyService,
            IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _auth = auth;
            _companyService = companyService;
            _memoryCache = memoryCache;
        }

        public string AdminBaseUrl
        {
            get
            {
                return _configuration["GeneralSettings:AdminBaseUrl"];
            }
        }

        public LoginUserInfoViewModel LoginUserInfo
        {
            get { 
                return GetLoginUserInfo().Result; 
            }
        }

        private async Task<LoginUserInfoViewModel> GetLoginUserInfo()
        {
            var loginUserInfo = new LoginUserInfoViewModel();

            var authState = await _auth.GetAuthenticationStateAsync();

            var userClaims = authState.User.Claims;

            loginUserInfo.UserId = Convert.ToInt32(userClaims.FirstOrDefault(f => f.Type == ClaimTypesConstants.UserId).Value);
            loginUserInfo.UserObjectId = userClaims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier).Value;
            loginUserInfo.ContactId = Convert.ToInt32(userClaims.FirstOrDefault(f => f.Type == ClaimTypesConstants.ContactId).Value);
            loginUserInfo.CompanyId = Convert.ToInt32(userClaims.FirstOrDefault(f => f.Type == ClaimTypesConstants.CompanyId).Value);
            loginUserInfo.CompanyObjectId = userClaims.FirstOrDefault(f => f.Type == ClaimTypesConstants.CompanyObjectId).Value;
            loginUserInfo.FirstName = userClaims.FirstOrDefault(f => f.Type == ClaimTypes.Surname).Value;
            loginUserInfo.FullName = userClaims.FirstOrDefault(f => f.Type == ClaimTypesConstants.FullName).Value;
            loginUserInfo.CompanyType = Convert.ToInt32(userClaims.FirstOrDefault(f => f.Type == ClaimTypesConstants.CompanyType).Value);
            loginUserInfo.TeamId = Convert.ToInt32(userClaims.FirstOrDefault(f => f.Type == ClaimTypesConstants.TeamId)?.Value);
            loginUserInfo.Username = userClaims.FirstOrDefault(f => f.Type == ClaimTypesConstants.Username)?.Value;
            if (!string.IsNullOrWhiteSpace(userClaims.FirstOrDefault(f => f.Type == ClaimTypesConstants.RoleObjectId).Value))
            {
                loginUserInfo.RoleObjectId = userClaims.FirstOrDefault(f => f.Type == ClaimTypesConstants.RoleObjectId).Value;
            }

			//loginUserInfo.RoleName = userClaims.FirstOrDefault(f => f.Type == ClaimTypes.Role).Value;

			//set connection string 
			//loginUserInfo.ConnectionString = "Server=DESKTOP-RTBQ2VM;Database=A2ErpCustomer0001;Integrated Security = true; MultipleActiveResultSets=true;"; //await GetConnectionString(loginUserInfo.CompanyObjectId, true);
			return loginUserInfo;
        }



        //private async Task<string> GetConnectionString(string companyObjectId, bool isCache = true)
        //{
        //    var connString = string.Empty;


        //    if (isCache)
        //    {
        //        connString = _memoryCache.Get<string>(key: $"con-str-{companyObjectId}");

        //        if (string.IsNullOrWhiteSpace(connString))
        //        {
        //            var connResponse = await _companyService.GetConnectionStringByCompanyObjectId(companyObjectId);
        //            connString = connResponse.Result;

        //             _memoryCache.Set<string>(key: $"con-str-{companyObjectId}", connString, TimeSpan.FromMinutes(value: 120));
        //         }
        //    }
        //    else
        //    {
        //        var connResponse = await _companyService.GetConnectionStringByCompanyObjectId(companyObjectId);
        //        connString = connResponse.Result;
        //    }

        //    return connString;
        //}

        //public LoginUserCompanyDetailViewModel()
        //{

        //}
    }
}
