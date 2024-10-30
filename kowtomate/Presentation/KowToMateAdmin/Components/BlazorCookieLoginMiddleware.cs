//using CutOutWiz.Core.Utilities;
//using KowToMateAdmin.Models.Security;
//using CutOutWiz.Data.Security;
//using CutOutWiz.Services.Common;
//using CutOutWiz.Services.Security;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using System.Collections.Concurrent;
//using System.Net;
//using System.Security.Claims;
//using static CutOutWiz.Core.Utilities.Enums;
//using Newtonsoft.Json;
//using System.Diagnostics;
//using CutOutWiz.Services.Management;

//public class BlazorCookieLoginMiddleware
//{
//    public static IDictionary<Guid, LoginViewModel> Logins { get; private set; } = new ConcurrentDictionary<Guid, LoginViewModel>();
//    public static IDictionary<Guid, LoginUserInfoViewModel> Switch { get; set; } = new ConcurrentDictionary<Guid, LoginUserInfoViewModel>();


//    private readonly RequestDelegate _next;
//    private readonly IHttpContextAccessor _httpContextAccessor;
//    private readonly ISecurityLoginHistoryService _securityLoginHistoryService;




//    private readonly IRoleService _roleManager;
//    private readonly IUserService _userService;
//    private readonly IPermissionService _permissionService;
//    private readonly ICompanyService _companyService;
//    private readonly IContactService _contactManager;
//    private readonly ITeamService _teamService;

//    public BlazorCookieLoginMiddleware(RequestDelegate next,
//        IHttpContextAccessor httpContextAccessor,
//        ISecurityLoginHistoryService securityLoginHistoryService,
//        IRoleService roleService,
//        IUserService userService,
//        IPermissionService permissionService,
//        ICompanyService companyService,
//        IContactService contactService,
//        ITeamService teamService
//        )
//    {
//        _next = next;
//        _httpContextAccessor = httpContextAccessor;
//        _securityLoginHistoryService = securityLoginHistoryService;
//        _roleManager = roleService;
//        _userService = userService;
//        _permissionService = permissionService;
//        _companyService = companyService;
//        _contactManager = contactService;
//        _teamService = teamService;
//    }

//    public async Task Invoke(HttpContext context,
//        ICustomAuthenticationService authenticationService,
//        IPermissionService permissionService,
//        ICompanyService companyService,
//        IContactService contactService,
//        IRoleService roleService,
//        IUserService userService
//        )

//    {
//        if (context.Request.Path == "/authorizelogin" && context.Request.Query.ContainsKey("key"))
//        {
//            var key = Guid.Parse(context.Request.Query["key"]);

//            var loginUserInfo = Logins[key];

//            //var result = await signInMgr.PasswordSignInAsync(info.Email, info.Password, false, lockoutOnFailure: true);
//            var authenticationResponse = await authenticationService.AuthenticateUser(loginUserInfo);

//            loginUserInfo.Password = null;

//            if (authenticationResponse.IsSuccess)
//            {
//                //add login
//                var user = authenticationResponse.Result;
//                var claims = new List<Claim>();

//                //var permissions = await permissionService.GetAllByUserId(user.ObjectId);

//                //if (permissions != null && permissions.Any())
//                //{
//                //	foreach (var permission in permissions)
//                //	{
//                //		claims.Add(new Claim(ClaimTypes.Role, permission.PermissionValue));
//                //	}
//                //}
//                var getUserRole = _roleManager.GetAllUserRoleByContactObjectId(user.ObjectId).Result.FirstOrDefault();

//                var permissions = await _permissionService.GetAllPermissionByRoleObjectId(getUserRole.ObjectId);

//                if (permissions != null && permissions.Any())
//                {
//                    foreach (var permission in permissions)
//                    {
//                        claims.Add(new Claim(ClaimTypes.Role, permission.PermissionValue));
//                    }
//                }

//                string ip = context.Connection?.RemoteIpAddress.ToString();
//                //Update his/her setup setup 
//                //var request = context.Request;
//                //var requesterIpAddress = request.Host;

//                //claims.Add(new Claim(ClaimTypes.Uri, ip));

//                var company = await companyService.GetById(user.CompanyId);

//                var contact = await contactService.GetById(user.ContactId);

//                //var connString = "";               
//                claims.Add(new Claim(ClaimTypes.Name, user.Username));
//                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.ObjectId));
//                claims.Add(new Claim(ClaimTypesConstants.UserId, user.Id.ToString()));
//                claims.Add(new Claim(ClaimTypesConstants.CompanyObjectId, company.ObjectId));
//                claims.Add(new Claim(ClaimTypesConstants.CompanyId, company.Id.ToString()));
//                claims.Add(new Claim(ClaimTypes.Surname, contact?.FirstName));
//                claims.Add(new Claim(ClaimTypesConstants.FullName, contact?.FirstName + " " + contact?.LastName));

//                claims.Add(new Claim(ClaimTypesConstants.CompanyType, Convert.ToString(company.CompanyType)));
//                claims.Add(new Claim(ClaimTypesConstants.RoleObjectId, Convert.ToString(getUserRole.ObjectId)));

//                if (user.ContactId > 0)
//                {
//                    claims.Add(new Claim(ClaimTypesConstants.ContactId, user.ContactId.ToString()));
//                }

//                //if (role.Code != RoleConstants.SystemAdmin)
//                //{
//                //    var connResponse = await companyService.GetConnectionStringByCompanyObjectId(company.ObjectId);
//                //    connString = connResponse.Result;
//                //    var contact = await contactService.GetByUserIdAndCompanyId(user.Id, company.Id, connString);

//                //    if (contact != null)
//                //    {
//                //        claims.Add(new Claim(ClaimTypesConstants.ContactId, contact.Id.ToString()));
//                //    }
//                //}                              

//                var claimsIdentity = new ClaimsIdentity(
//                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

//                var authProperties = new AuthenticationProperties
//                {
//                    IsPersistent = true
//                };

//                await context.SignInAsync(
//                   CookieAuthenticationDefaults.AuthenticationScheme,
//                   new ClaimsPrincipal(claimsIdentity),
//                   authProperties);

//                //Update User Download Path

//                contact.DownloadFolderPath = "//" + ip + "/KD";
//                contact.IsUserActive = true;

//                if (Directory.Exists(contact.DownloadFolderPath))
//                {
//                    contact.IsSharedFolderEnable = true;
//                    await contactService.UpdateContactDownloadPath(contact);
//                }
//                else
//                {
//                    contact.IsSharedFolderEnable = false;
//                    contact.DownloadFolderPath = "";
//                    await contactService.UpdateContactDownloadPath(contact);
//                }

//                if (company.CompanyType != (int)CompanyType.Client)
//                {
//                    // Added Login History
//                    var loginHistory = new SecurityLoginHistory
//                    {
//                        UserId = user.Id,
//                        Username = user.Username,
//                        ActionTime = DateTime.Now,
//                        ActionType = true,
//                        IPAddress = ip,
//                        DeviceInfo = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString(),
//                        Status = true,
//                    };

//                    var saveLoginHistory = await _securityLoginHistoryService.Insert(loginHistory);
//                }
//                Logins.Remove(key);
//                context.Response.Redirect("/");
//                return;
//            }
//            else
//            {
//                //TODO: Proper error handling
//                //context.Response.Redirect("/loginfailed");
//                context.Response.Redirect("/");
//                return;
//            }
//        }

//        else if (context.Request.Path == "/orders" && context.Request.Query.ContainsKey("key"))
//        {
//            var isSignOut = false;
//            var loginUserInfo = new LoginUserInfoViewModel();
//            var key = Guid.Empty; // Initialize with an empty Guid

//            if (!isSignOut)
//            {
//                var keyValue = context.Request.Query["key"];
//                if (!string.IsNullOrEmpty(keyValue) && Guid.TryParse(keyValue, out var parsedKey))
//                {
//                    key = parsedKey;
//                    loginUserInfo = Switch[key];
//                    await context.SignOutAsync();
//                    isSignOut = true;
//                }
//            }
//            var contactId = loginUserInfo.ContactId;
//            var companyId = loginUserInfo.CompanyId;
//            var roleObjectid = loginUserInfo.RoleObjectId;
//            var authenticationResponse = await _userService.GetUserByContactId(contactId);
//            var contact = await _contactManager.GetById(contactId);

//            if (authenticationResponse != null)
//            {
//                //add login
//                var user = authenticationResponse;
//                var claims = new List<Claim>();


//                var company = await _companyService.GetById((int)companyId);

//                var permissions = await _permissionService.GetAllPermissionByRoleObjectId(roleObjectid);

//                var contactTeam = await _teamService.GetByContactId(user.ContactId);

//                if (permissions != null && permissions.Any())
//                {
//                    foreach (var permission in permissions)
//                    {
//                        claims.Add(new Claim(ClaimTypes.Role, permission.PermissionValue));
//                    }
//                }

//                string ip = _httpContextAccessor.HttpContext.Connection?.RemoteIpAddress.ToString();
//                //var connString = "";
//                claims.Add(new Claim(ClaimTypes.Name, user.Username));
//                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.ObjectId));
//                claims.Add(new Claim(ClaimTypesConstants.UserId, user.Id.ToString()));
//                claims.Add(new Claim(ClaimTypesConstants.CompanyObjectId, company.ObjectId));
//                claims.Add(new Claim(ClaimTypesConstants.CompanyId, company.Id.ToString()));
//                claims.Add(new Claim(ClaimTypes.Surname, contact?.FirstName));
//                claims.Add(new Claim(ClaimTypesConstants.FullName, contact?.FirstName + " " + contact?.LastName));

//                claims.Add(new Claim(ClaimTypesConstants.CompanyType, Convert.ToString(company.CompanyType)));
//                claims.Add(new Claim(ClaimTypesConstants.RoleObjectId, Convert.ToString(roleObjectid)));
//                claims.Add(new Claim(ClaimTypesConstants.TeamId, Convert.ToString(contactTeam?.TeamId)));
//                if (user.ContactId > 0)
//                {
//                    claims.Add(new Claim(ClaimTypesConstants.ContactId, user.ContactId.ToString()));
//                }

//                var claimsIdentity = new ClaimsIdentity(
//                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

//                var authProperties = new AuthenticationProperties
//                {
//                    IsPersistent = true
//                };

//                await _httpContextAccessor.HttpContext.SignInAsync(
//                            CookieAuthenticationDefaults.AuthenticationScheme,
//                            new ClaimsPrincipal(claimsIdentity),
//                            authProperties);

//                Switch.Remove(key);
//                context.Response.Redirect("/orders");
//                return;

//            }
//        }

//        else if (context.Request.Path == "/authorizelogout")
//        {
//            await context.SignOutAsync();

//            context.Response.Redirect("/");
//            return;
//        }
//        else
//        {
//            await _next.Invoke(context);
//        }
//    }
//}


using CutOutWiz.Core.Utilities;
using KowToMateAdmin.Models.Security;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Concurrent;
using System.Net;
using System.Security.Claims;
using static CutOutWiz.Core.Utilities.Enums;
using Newtonsoft.Json;
using System.Diagnostics;
using CutOutWiz.Services.Management;
using CutOutWiz.Services.Managers.Common;

public class BlazorCookieLoginMiddleware
{
	public static IDictionary<Guid, LoginViewModel> Logins { get; private set; } = new ConcurrentDictionary<Guid, LoginViewModel>();
	public static IDictionary<Guid, LoginUserInfoViewModel> Switch { get; set; } = new ConcurrentDictionary<Guid, LoginUserInfoViewModel>();

	private readonly RequestDelegate _next;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IServiceScopeFactory _serviceScopeFactory;

	public BlazorCookieLoginMiddleware(RequestDelegate next,
		IHttpContextAccessor httpContextAccessor,
		IServiceScopeFactory serviceScopeFactory)
	{
		_next = next;
		_httpContextAccessor = httpContextAccessor;
		_serviceScopeFactory = serviceScopeFactory;
	}

	public async Task Invoke(HttpContext context)
	{
		if (context.Request.Path == "/authorizelogin" && context.Request.Query.ContainsKey("key"))
		{
			var key = Guid.Parse(context.Request.Query["key"]);

			var loginUserInfo = Logins[key];

			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var authenticationService = scope.ServiceProvider.GetRequiredService<ICustomAuthenticationService>();
				var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
				var companyService = scope.ServiceProvider.GetRequiredService<ICompanyManager>();
				var contactService = scope.ServiceProvider.GetRequiredService<IContactManager>();
				var roleService = scope.ServiceProvider.GetRequiredService<IRoleManager>();
				var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
				var securityLoginHistoryService = scope.ServiceProvider.GetRequiredService<ISecurityLoginHistoryService>();

				var authenticationResponse = await authenticationService.AuthenticateUser(loginUserInfo);

				loginUserInfo.Password = null;

				if (authenticationResponse.IsSuccess)
				{
					//add login
					var user = authenticationResponse.Result;
					var claims = new List<Claim>();

					var getUserRole = roleService.GetAllUserRoleByContactObjectId(user.ObjectId).Result.FirstOrDefault();
					var permissions = await permissionService.GetAllPermissionByRoleObjectId(getUserRole.ObjectId);

					if (permissions != null && permissions.Any())
					{
						foreach (var permission in permissions)
						{
							claims.Add(new Claim(ClaimTypes.Role, permission.PermissionValue));
						}
					}

					string ip = context.Connection?.RemoteIpAddress.ToString();

					var company = await companyService.GetById(user.CompanyId);
					var contact = await contactService.GetById(user.ContactId);

					claims.Add(new Claim(ClaimTypes.Name, user.Username));
					claims.Add(new Claim(ClaimTypes.NameIdentifier, user.ObjectId));
					claims.Add(new Claim(ClaimTypesConstants.UserId, user.Id.ToString()));
					claims.Add(new Claim(ClaimTypesConstants.CompanyObjectId, company.ObjectId));
					claims.Add(new Claim(ClaimTypesConstants.CompanyId, company.Id.ToString()));
					claims.Add(new Claim(ClaimTypes.Surname, contact?.FirstName));
					claims.Add(new Claim(ClaimTypesConstants.FullName, contact?.FirstName + " " + contact?.LastName));

					claims.Add(new Claim(ClaimTypesConstants.CompanyType, Convert.ToString(company.CompanyType)));
					claims.Add(new Claim(ClaimTypesConstants.RoleObjectId, Convert.ToString(getUserRole.ObjectId)));
					claims.Add(new Claim(ClaimTypesConstants.Username, Convert.ToString(user.Username)));

					if (user.ContactId > 0)
					{
						claims.Add(new Claim(ClaimTypesConstants.ContactId, user.ContactId.ToString()));
					}

					var claimsIdentity = new ClaimsIdentity(
						claims, CookieAuthenticationDefaults.AuthenticationScheme);

					var authProperties = new AuthenticationProperties
					{
						IsPersistent = true
					};

					await context.SignInAsync(
					   CookieAuthenticationDefaults.AuthenticationScheme,
					   new ClaimsPrincipal(claimsIdentity),
					   authProperties);

					contact.DownloadFolderPath = "//" + ip + "/KD";
					contact.IsUserActive = true;

					if (Directory.Exists(contact.DownloadFolderPath))
					{
						contact.IsSharedFolderEnable = true;
						await contactService.UpdateContactDownloadPath(contact);
					}
					else
					{
						contact.IsSharedFolderEnable = false;
						contact.DownloadFolderPath = "";
						await contactService.UpdateContactDownloadPath(contact);
					}

					if (company.CompanyType != (int)CompanyType.Client)
					{
						var loginHistory = new SecurityLoginHistoryModel
						{
							UserId = user.Id,
							Username = user.Username,
							ActionTime = DateTime.Now,
							ActionType = true,
							IPAddress = ip,
							DeviceInfo = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString(),
							Status = true,
						};

						await securityLoginHistoryService.Insert(loginHistory);
					}
					Logins.Remove(key);
					context.Response.Redirect("/");
					return;
				}
				else
				{
					context.Response.Redirect("/");
					return;
				}
			}
		}
		else if (context.Request.Path == "/orders" && context.Request.Query.ContainsKey("key"))
		{
			var isSignOut = false;
			var loginUserInfo = new LoginUserInfoViewModel();
			var key = Guid.Empty; // Initialize with an empty Guid

			if (!isSignOut)
			{
				var keyValue = context.Request.Query["key"];
				if (!string.IsNullOrEmpty(keyValue) && Guid.TryParse(keyValue, out var parsedKey))
				{
					key = parsedKey;
					loginUserInfo = Switch[key];
					await context.SignOutAsync();
					isSignOut = true;
				}
			}
			var contactId = loginUserInfo.ContactId;
			var companyId = loginUserInfo.CompanyId;
			var roleObjectid = loginUserInfo.RoleObjectId;

			using (var scope = _serviceScopeFactory.CreateScope())
			{
				var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
				var contactService = scope.ServiceProvider.GetRequiredService<IContactManager>();
				var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
				var companyService = scope.ServiceProvider.GetRequiredService<ICompanyManager>();
				var teamService = scope.ServiceProvider.GetRequiredService<ITeamService>();

				var authenticationResponse = await userService.GetUserByContactId(contactId);
				var contact = await contactService.GetById(contactId);

				if (authenticationResponse != null)
				{
					var user = authenticationResponse;
					var claims = new List<Claim>();

					var company = await companyService.GetById((int)companyId);
					var permissions = await permissionService.GetAllPermissionByRoleObjectId(roleObjectid);
					var contactTeam = await teamService.GetByContactId(user.ContactId);

					if (permissions != null && permissions.Any())
					{
						foreach (var permission in permissions)
						{
							claims.Add(new Claim(ClaimTypes.Role, permission.PermissionValue));
						}
					}

					string ip = _httpContextAccessor.HttpContext.Connection?.RemoteIpAddress.ToString();
					claims.Add(new Claim(ClaimTypes.Name, user.Username));
					claims.Add(new Claim(ClaimTypes.NameIdentifier, user.ObjectId));
					claims.Add(new Claim(ClaimTypesConstants.UserId, user.Id.ToString()));
					claims.Add(new Claim(ClaimTypesConstants.CompanyObjectId, company.ObjectId));
					claims.Add(new Claim(ClaimTypesConstants.CompanyId, company.Id.ToString()));
					claims.Add(new Claim(ClaimTypes.Surname, contact?.FirstName));
					claims.Add(new Claim(ClaimTypesConstants.FullName, contact?.FirstName + " " + contact?.LastName));
					claims.Add(new Claim(ClaimTypesConstants.CompanyType, Convert.ToString(company.CompanyType)));
					claims.Add(new Claim(ClaimTypesConstants.RoleObjectId, Convert.ToString(roleObjectid)));
					claims.Add(new Claim(ClaimTypesConstants.TeamId, Convert.ToString(contactTeam?.TeamId)));
					if (user.ContactId > 0)
					{
						claims.Add(new Claim(ClaimTypesConstants.ContactId, user.ContactId.ToString()));
					}

					var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var authProperties = new AuthenticationProperties { IsPersistent = true };

					await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

					Switch.Remove(key);
					context.Response.Redirect("/orders");
					return;
				}
			}
		}
		else if (context.Request.Path == "/authorizelogout")
		{
			await context.SignOutAsync();
			context.Response.Redirect("/");
			return;
		}
		else
		{
			await _next.Invoke(context);
		}
	}
}
