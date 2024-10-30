using CutOutWiz.Core;
using CutOutWiz.Services.Models.EmailModels;
using CutOutWiz.Core.Models.CpanelStorage;
using CutOutWiz.Services.Models.Message;
using CutOutWiz.Services.CpanelStorageInfoServices;
using CutOutWiz.Services.Email;
using CutOutWiz.Services.EmailMessage;
using CutOutWiz.Services.MessageService;
using Google.Api.Gax.ResourceNames;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KowToMateAdmin.Services
{
    public class CloudStorageUsesLimitAlertBackgroundService : BackgroundService
    {
        private readonly ILogger<CloudStorageUsesLimitAlertBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private int _cloudStorageCheckPercentage = 96;

        public CloudStorageUsesLimitAlertBackgroundService(
            ILogger<CloudStorageUsesLimitAlertBackgroundService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _cloudStorageCheckPercentage = Convert.ToInt32(_configuration["CpanelStorage:CloudStorageCheckPercentage"]);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var isNotificateionSent = false;
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var cpanelStorageService = scope.ServiceProvider.GetRequiredService<ICpanelStorageInfoService>();
                    var _emailSenderAccountService = scope.ServiceProvider.GetRequiredService<IWorkflowEmailService>();

                    try
                    {
                        CpanelStorageInfoViewModel storageInfo = new CpanelStorageInfoViewModel();
                        var projectWiseResponse = await cpanelStorageService.GetCpanelStorageByProjectWise();
                        var totalCloudMemoryUsesPercentage = await cpanelStorageService.GetCpanelStorageInfo();

                        if (totalCloudMemoryUsesPercentage.Result != null)
                        {
                            storageInfo = totalCloudMemoryUsesPercentage.Result;
                        }
                        if (projectWiseResponse.Result == null)
                        {
                            return;
                        }
                        var memoryUsesPercentage = int.Parse(storageInfo.used_percentage.Replace("%", "").Trim());

                        if (memoryUsesPercentage > _cloudStorageCheckPercentage)
                        {
                            if (projectWiseResponse.Result != null)
                            {
                                List<string> opsEmailList = new List<string>
                                {
                                    "ops@thekowcompany.com",
                                    "mashfiq@thekowcompany.com",
                                    "ashik@thekowcompany.com",
                                    "zakir@thekowcompany.com",
                                    "rakibul@thekowcompany.com",
                                };

                                CloudStorageUsesLimitationNotifyOps cloudStorageUsesLimitationNotifyOps = new CloudStorageUsesLimitationNotifyOps
                                {
                                    EmailAddresses = opsEmailList,
                                    MemoryUses = totalCloudMemoryUsesPercentage.Result.used_percentage,
                                };

                                await _emailSenderAccountService.SendEmailNotificationForCloudStorageLimitation(cloudStorageUsesLimitationNotifyOps);

                                isNotificateionSent = true;
                                _logger.LogInformation("Email sent to recipients.");
                            }
                            else
                            {
                                _logger.LogWarning("No project-wise storage information found.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while executing background task.");
                    }

                    await Task.Delay(TimeSpan.FromHours(2), stoppingToken);

                }
                if (isNotificateionSent)
                {
                    // Delay for 24 Hours
                    await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
                    isNotificateionSent = false;
                }
            }
        }
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("StorageAlertBackgroundService is stopping.");
            return base.StopAsync(stoppingToken);
        }
    }
}
