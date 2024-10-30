using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.StorageService;
using FluentFTP;

namespace CutOutWiz.Services.FtpFileDeleteServices
{
    public class FtpFileDeleteService : IFtpFileDeleteService
    {

        private readonly IFluentFtpService _fluentFtpService;

        public FtpFileDeleteService(IFluentFtpService fluentFtpService)
        {
            _fluentFtpService = fluentFtpService;
        }

        public async Task DeleteAllFileBeforeLastTwoDays()
        {

            //string host = "eu-storage1.kowtransfer.com";
            //string username = "devtestkow@eu-storage1.kowtransfer.com";
            //string password = "K@w$ev#~T#s!2@2*$";

            string host = "192.168.1.9";
            string username = "clientktm";
            string password = "6BNa?o=)";



            FtpConfig ftpConfig = await _fluentFtpService.GetFluentFtpConfig();

            using (var sourceClient = new AsyncFtpClient(host,
            username, password, 21, ftpConfig))
                {
                sourceClient.Config.EncryptionMode = FtpEncryptionMode.Auto;
                sourceClient.Config.ValidateAnyCertificate = true;
                await sourceClient.AutoConnect();

                try
                {
                    //, FtpListOption.Recursive

                    FtpListItem[] ftpRootListItems = await sourceClient.GetListing("/KTM");

                    foreach (var rootFolder in ftpRootListItems)
                    {
                        FtpListItem[] ftpListItems = await sourceClient.GetListing(rootFolder.FullName);

                        foreach (var ftpItem in ftpListItems)
                        {
                            FtpListItem[] folderItems = await sourceClient.GetListing(ftpItem.FullName, FtpListOption.Recursive);

                            DateTime currentDate = DateTime.Now;
                            
                            DateTime twoDaysAgo = currentDate.AddDays(-2);

                            // Filter files that were last modified 2 days ago
                            var filesToDelete = folderItems
                                .Where(item => item.Type == FtpObjectType.File && item.Modified < twoDaysAgo)
                                .ToList();
                            
                            SemaphoreSlim deletePhore = new SemaphoreSlim(30);
                            var deleteTasks = new List<Task>();
                            // Delete the files
                            foreach (var fileToDelete in filesToDelete)
                            {
                                Console.WriteLine(fileToDelete.FullName);

                                await deletePhore.WaitAsync();

                                deleteTasks.Add(Task.Run(async () =>
                                {
                                    try
                                    {
                                        await sourceClient.DeleteFile(fileToDelete.FullName);
                                    }
                                    catch (Exception)
                                    {
                                        throw;
                                    }
                                    finally
                                    {
                                        deletePhore.Release();
                                    }

                                }));

                            }
                            await Task.WhenAll(deleteTasks);
                        }
                    }

                    await sourceClient.Disconnect();
                }
                catch (Exception ex)
                {

                    throw;
                }

               
            }
            }
        }
}
