using CutOutWiz.Services.Common;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services.HR;
using CutOutWiz.Services.Email;
using CutOutWiz.Services.Management;
using CutOutWiz.Services.Security;
using KowToMateAdmin.Data;
using CutOutWiz.Services.SOP;
using CutOutWiz.Services.EmailMessage;
using CutOutWiz.Services.EmailSender;
using CutOutWiz.Services.LogService;
using Radzen;

namespace KowToMateAdmin
{
    public static class RegisterServices
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddScoped<DialogService>();

            builder.Services.AddTransient<IWorkContext, WorkContext>();
            builder.Services.AddTransient<ICustomAuthenticationService, CustomAuthenticationService>();
            builder.Services.AddTransient<ISqlDataAccess, SqlDataAccess>();

            builder.Services.AddTransient<IDesignationService, DesignationService>();
            builder.Services.AddTransient<ICompanyService, CompanyService>();
            builder.Services.AddTransient<ICountryService, CountryService>();
            builder.Services.AddTransient<IContactService, ContactService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IModuleService, ModuleService>();
            builder.Services.AddTransient<IMenuService, MenuService>();
            builder.Services.AddTransient<IPermissionService, PermissionService>();
            builder.Services.AddTransient<IRoleService, RoleService>();
            builder.Services.AddTransient<IFileServerService, FileServerService>();
            builder.Services.AddTransient<IEmailSenderAccountService, EmailSenderAccountService>();
            builder.Services.AddTransient<IEmailTemplateService, EmailTemplateService>();
            builder.Services.AddTransient<ITeamService, TeamService>();
            builder.Services.AddTransient<ITeamRoleService, TeamRoleService>();
            builder.Services.AddTransient<ITeamMemberService, TeamMemberService>();
            builder.Services.AddTransient<ISOPStandardServiceService, SOPStandardServiceService>();
            builder.Services.AddTransient<ISOPTemplateService, SOPTemplateService>();

            builder.Services.AddTransient<ILogService, LogService>();
            builder.Services.AddTransient<IWorkflowEmailService, WorkflowEmailService>();
            builder.Services.AddTransient<IEmailTokenProvider, EmailTokenProvider>();
            builder.Services.AddTransient<IEmailTokenizer, EmailTokenizer>();
            builder.Services.AddTransient<IMailjetEmailService, MailjetEmailService>();            
        }
    }
}
