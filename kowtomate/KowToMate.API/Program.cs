using CutOutWiz.Services.ApprovalTool;
//using CutOutWiz.Services.DataService;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services.HR;
using CutOutWiz.Services.LogServices;
using CutOutWiz.Services.Managers.Common;
using CutOutWiz.Services.MessageService;
using CutOutWiz.Services.ReportServices;
using CutOutWiz.Services.Security;
using CutOutWiz.Services.StorageService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
//builder.Services.AddSingleton<IFileTrackingService, FileTrackingService>();
builder.Services.AddSingleton<IAwsService, AwsService>();
builder.Services.AddSingleton<IEmailSenderService, EmailSenderService>();
builder.Services.AddSingleton<IApprovalToolCommonService, ApprovalToolCommonService>();
builder.Services.AddSingleton<IReportService, ReportService>();
builder.Services.AddSingleton<ILogService, LogService>();
builder.Services.AddSingleton<IModuleService, ModuleService>();
builder.Services.AddSingleton<IMenuService, MenuService>();
builder.Services.AddSingleton<IPermissionService, PermissionService>();
builder.Services.AddSingleton<IDesignationService, DesignationService>();
builder.Services.AddSingleton<IRoleManager, RoleManager>();
builder.Services.AddSingleton<IContactManager, ContactManager>();
builder.Services.AddSingleton<ICountryManager, CountryManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
