using CutOutWiz.Services.Models.FtpModels;
using CutOutWiz.Services.Models.Security;
using Dapper;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.StorageService
{
    public class FTPClient
    {
        private FtpClient _client;
        List<string> returnRemotePaths = new List<string>();
        private FtpConfig _config;
        private CompanyGeneralSettingModel _generalSetting;
        public FTPClient(FtpCredentailsModel storageInfo,CompanyGeneralSettingModel companyGeneralSetting ,FtpConfig? ftpConfig = null)
        {
            _client = new FtpClient(storageInfo.Host, storageInfo.UserName, storageInfo.Password,storageInfo.Port??21);
            _client.Encoding = System.Text.Encoding.UTF8;  // Ensure UTF-8 encoding
            _client.Config.EncryptionMode = ftpConfig.EncryptionMode;
            _config = ftpConfig;
            _generalSetting = companyGeneralSetting;
        }
        public async Task Connect()
        {
            if(_config.EncryptionMode == FtpEncryptionMode.None)
            {
                _client.Connect();
            }
            else
            {
                _client.AutoConnect();
            }

        }

        public async Task Disconnect()
        {
            _client.Disconnect();
        }
        /// <summary>
        /// List of File Paths with recursively
        /// </summary>
        /// <param name="remoteDirPath">Received Ftp Base Root Path</param>
        /// <returns>List of File Paths</returns>
        public async Task<List<string>> ReadallFtpFilePathWithRecursively(string remoteDirPath, string skipFolderPath)
        {
            var ListOfPaths = await ListDirectoryFilePathsWithRecursive(remoteDirPath, skipFolderPath);
            var removeDirectoryNameFromList = ListOfPaths.Where(x=>x.Contains(".")).ToList();
            return removeDirectoryNameFromList;
        }

        /// <summary>
        /// List of File Paths with recursively
        /// </summary>
        /// <param name="remoteDirPath">Received Ftp Base Root Path</param>
        /// <returns>List of File Paths</returns>
        public async Task<List<string>> ReadallFtpFilePathWithoutRecursively(string remoteDirPath)
        {
            var ListOfPaths = await ListDirectoryFilePathsWithoutRecursive(remoteDirPath);
            var removeDirectoryNameFromList = ListOfPaths.Where(x => x.Contains(".")).ToList();
            return removeDirectoryNameFromList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteDirPath"></param>
        /// <returns></returns>
        private async Task<List<string>> ListDirectoryFilePathsWithoutRecursive(string remoteDirPath)
        {
            await Task.Yield();
            _client.SetWorkingDirectory(remoteDirPath);
            var listing = _client.GetListing(remoteDirPath, FtpListOption.NoPath);

            foreach (var item in listing)
            {
                string remotePath = Path.Combine(remoteDirPath, item.Name).Replace("\\", "/");
                if (item.Type == FtpObjectType.File)
                {
                    if (_generalSetting.IsFtpIdleTimeChecking && item.FullName.Contains(".zip"))
                    {
                        int? initftpIdleTime = _generalSetting.FtpIdleTime;
                        var checkTheFileModifiedBeforeProvidedFtpIdletime = item.Modified < DateTime.UtcNow.AddMinutes(-initftpIdleTime ?? 1);

                        if (checkTheFileModifiedBeforeProvidedFtpIdletime)
                        {
                            returnRemotePaths.Add(remotePath);
                        }
                    }
                    else
                    {
                        returnRemotePaths.Add(remotePath);
                    }
                }
                else if (item.Type == FtpObjectType.Directory)
                {
                    returnRemotePaths.Add(remotePath);
                    //await ListDirectoryFilePaths(remotePath);  // Recursive call
                }
            }

            return returnRemotePaths;
        }

        private async Task<List<string>> ListDirectoryFilePathsWithRecursive(string remoteDirPath, string skipFolderPath)
        {
            _client.SetWorkingDirectory(remoteDirPath);
            var listing = _client.GetListing(remoteDirPath, FtpListOption.NoPath);
            foreach (var item in listing)
            {
                string remotePath = Path.Combine(remoteDirPath, item.Name).Replace("\\", "/");
                if (item.Type == FtpObjectType.File)
                {
                    if (_generalSetting.IsFtpIdleTimeChecking && item.FullName.Contains(".zip"))
                    {
                        int? initftpIdleTime = _generalSetting.FtpIdleTime;
                        var checkTheFileModifiedBeforeProvidedFtpIdletime = item.Modified < DateTime.UtcNow.AddMinutes(-initftpIdleTime ?? 1);

                        if (checkTheFileModifiedBeforeProvidedFtpIdletime)
                        {
                            returnRemotePaths.Add(remotePath);
                            // Optionally, print/log the file path for debugging
                            // Console.WriteLine($"File: {remotePath} was modified more than {ftpIdleTime} minutes ago.");
                        }
                    }
                    else
                    {
                        returnRemotePaths.Add(remotePath);
                    }
                    //Console.WriteLine($"File: {remotePath}");
                }
                else if (item.Type == FtpObjectType.Directory)
                {
                    //Console.WriteLine($"Directory: {remotePath}");

                    if (string.IsNullOrEmpty(skipFolderPath) || !item.Name.Contains(skipFolderPath))
                    {
                        await ListDirectoryFilePathsWithRecursive(remotePath, skipFolderPath);  // Recursive call
                    }
                }
            }
            return returnRemotePaths;
        }
    }

}
