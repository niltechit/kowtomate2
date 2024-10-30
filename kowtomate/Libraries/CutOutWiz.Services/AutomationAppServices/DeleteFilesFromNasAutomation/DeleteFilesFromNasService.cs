using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.BLL;
using CutOutWiz.Services.ClientOrders;
using CutOutWiz.Services.StorageService;
using DocumentFormat.OpenXml.Drawing.Charts;
using FluentFTP;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.AutomationAppServices.DeleteFilesFromNasAutomation
{
    public class DeleteFilesFromNasService : IDeleteFilesFromNasService
    {
        private readonly IFileServerManager _fileServerService;
        private readonly ICompanyManager _companyService;
        private readonly IFluentFtpService _fluentFtpService;
        private readonly IActivityAppLogService _activityAppLogService;

        public DeleteFilesFromNasService(ICompanyManager companyService, 
            IFileServerManager fileServerService, IFtpFilePathService fpFilePathService,
            IFluentFtpService fluentFtpService,
            IActivityAppLogService activityAppLogService)
        {
            _companyService = companyService;
            _fileServerService = fileServerService;
            _fluentFtpService = fluentFtpService;
            _activityAppLogService = activityAppLogService;
        }

        public async Task<Response<bool>> DeleteInprogressFileFromNas()
        {
            var companies = await _companyService.GetAll();

            if (!companies.Any())
            {
                return new Response<bool>();
            }

            await CompanyWiseInprogressFileDelete(companies);

            throw new NotImplementedException();
        }

        private async Task CompanyWiseInprogressFileDelete(List<CompanyModel> companies)
        {
            foreach (var company in companies)
            {
                int dataRetentionPeriod = 7;
                DateTime fileDeleteStartDate = GetFileDeleteStartDate(dataRetentionPeriod);
                int deletionDays = 7;
                FileServerModel companyFileStoreServer = await _fileServerService.GetById((int)company.FileServerId);
                FileServerViewModel fileServerViewModel = CreateFileServerObject(companyFileStoreServer);
                await DeleteCompanyInprogressFile(company, fileDeleteStartDate, companyFileStoreServer, fileServerViewModel, deletionDays);
            }
        }

        private async Task DeleteCompanyInprogressFile(CompanyModel company, DateTime fileDeleteStartDate, FileServerModel companyServer, FileServerViewModel fileServerViewModel, int deletionDays)
        {
            FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();

            for (int dayCount = 0; dayCount < deletionDays; dayCount++)
            {
                using (var ftp = await _fluentFtpService.CreateFtpClient(fileServerViewModel))
                {
                    try
                    {
                        SetFtpConfigInformation(ftp);
                        await ftp.Connect();
                        var searchDate = fileDeleteStartDate.AddDays(-dayCount);
                        string finalSearchPath = CreateFinalSearchPathForDelete(company, companyServer, searchDate);
                        await ftp.DeleteDirectory(finalSearchPath, FtpListOption.Recursive | FtpListOption.NoPath);
                        await ftp.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        CommonActivityLogViewModel activity = new CommonActivityLogViewModel
                        {
                            CreatedByContactId = AutomatedAppConstant.ContactId,
                            ActivityLogFor = (int)ActivityLogForConstants.Company,
                            PrimaryId = company.Id,
                            ErrorMessage = $"CompanyId: {company.Id}, Company Code: {company.Code}. Server Host: {companyServer.Host}. Exception: {ex.Message}",
                            MethodName = "DeleteCompanyInprogressFile",
                            RazorPage = "DeleteFilesFromNasService",
                            Category = (int)ActivityLogCategory.GeneralException,
                        };
                        await _activityAppLogService.InsertAppErrorActivityLog(activity);
                    }
                }
            }
        }

        private static string CreateFinalSearchPathForDelete(CompanyModel company, FileServerModel companyServer, DateTime searchDate)
        {
            string inprogressFilePath = $"{company.Code.Trim()}/{searchDate.ToString("yyyy")}/{searchDate.ToString("MMMM")}/{searchDate.ToString("dd.MM.yyyy")}/In Progress";
            string finalSearchPath = companyServer.SubFolder + inprogressFilePath;
            return finalSearchPath;
        }

        private static void SetFtpConfigInformation(AsyncFtpClient ftp)
        {
            ftp.Config.EncryptionMode = FtpEncryptionMode.Auto;
            ftp.Config.ValidateAnyCertificate = true;
        }

        private static DateTime GetFileDeleteStartDate(int dataRetentionPeriod)
        {
            DateTime fileDeleteStartDate = DateTime.Now;
            fileDeleteStartDate = fileDeleteStartDate.AddDays(-dataRetentionPeriod);
            return fileDeleteStartDate;
        }

        private static FileServerViewModel CreateFileServerObject(FileServerModel companyServer)
        {        
            FileServerViewModel fileServerViewModel = new();
          
            fileServerViewModel = new FileServerViewModel()
            {
                Host = companyServer.Host,
                UserName = companyServer.UserName,
                Password = companyServer.Password,
                SubFolder = companyServer.SubFolder,
            };

            return fileServerViewModel;
        }
    }
}
