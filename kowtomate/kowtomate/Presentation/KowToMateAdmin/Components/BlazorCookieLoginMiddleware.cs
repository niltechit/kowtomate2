using CutOutWiz.Core.Utilities;
using CutOutWiz.Data.Security;
using CutOutWiz.Services.Common;
using CutOutWiz.Services.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Concurrent;
using System.Security.Claims;

public class BlazorCookieLoginMiddleware
{
    public static IDictionary<Guid, LoginViewModel> Logins { get; private set; }
        = new ConcurrentDictionary<Guid, LoginViewModel>();


    private readonly RequestDelegate _next;

    public BlazorCookieLoginMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, 
        ICustomAuthenticationService authenticationService,
        IPermissionService permissionService,
        ICompanyService companyService)
    {
        if (context.Request.Path == "/authorizelogin" && context.Request.Query.ContainsKey("key"))
        {
            var key = Guid.Parse(context.Request.Query["key"]);
            
            var loginUserInfo = Logins[key];

            //var result = await signInMgr.PasswordSignInAsync(info.Email, info.Password, false, lockoutOnFailure: true);
            var authenticationResponse = await authenticationService.AuthenticateUser(loginUserInfo);

            loginUserInfo.Password = null;

            if (authenticationResponse.IsSuccess)
            {
                //add login
                var user = authenticationResponse.Result;
                var claims = new List<Claim>(); 

                var permissions = await permissionService.GetAllByUserId(user.ObjectId);
                
                if (permissions != null && permissions.Any())
                {
                    foreach(var permission in permissions)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, permission.Value));
                    }
                }

                var company = await companyService.GetById(user.CompanyId);
                //var connString = "";               
                claims.Add(new Claim(ClaimTypes.Name, user.Username));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.ObjectId));
                claims.Add(new Claim(ClaimTypesConstants.UserId, user.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, user.Username));
                claims.Add(new Claim(ClaimTypesConstants.CompanyObjectId, company.ObjectId));
                claims.Add(new Claim(ClaimTypesConstants.CompanyId, company.Id.ToString()));

                if (user.ContactId > 0)
                {
                    claims.Add(new Claim(ClaimTypesConstants.ContactId, user.ContactId.ToString()));
                }

                //if (role.Code != RoleConstants.SystemAdmin)
                //{
                //    var connResponse = await companyService.GetConnectionStringByCompanyObjectId(company.ObjectId);
                //    connString = connResponse.Result;
                //    var contact = await contactService.GetByUserIdAndCompanyId(user.Id, company.Id, connString);

                //    if (contact != null)
                //    {
                //        claims.Add(new Claim(ClaimTypesConstants.ContactId, contact.Id.ToString()));
                //    }
                //}                              

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

                Logins.Remove(key);
                context.Response.Redirect("/");
                return;
            }
            else
            {
                //TODO: Proper error handling
                //context.Response.Redirect("/loginfailed");
                context.Response.Redirect("/");
                return;
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