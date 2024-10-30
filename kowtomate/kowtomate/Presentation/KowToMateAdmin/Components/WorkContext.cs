using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Common;
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
        private readonly ICompanyService _companyService;
        private readonly IMemoryCache _memoryCache;
        private IConfiguration _configuration;

        public WorkContext(IConfiguration configuration,
            AuthenticationStateProvider auth,
            ICompanyService companyService,
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
