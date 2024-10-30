using CutOutWiz.Services.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.ClientOrders
{
    public class FtpFilePathService : IFtpFilePathService
    {
        /// <summary>
        /// fileCurrentStatusWiseLocation : Raw, In Progress, Completed
        /// </summary>
        /// <param name="fileCurrentStatusWiseLocation"></param>
        /// <returns></returns>
        public string GetFtpRootFolderPathUptoOrderNumber(string companyCode, DateTime currentDateTime,
            string orderNumber, string fileCurrentStatusWiseLocation)
        {
            return $"{companyCode.Trim()}\\{currentDateTime.ToString("yyyy")}\\{currentDateTime.ToString("MMMM")}\\{currentDateTime.ToString("dd.MM.yyyy")}\\{fileCurrentStatusWiseLocation}\\{orderNumber}\\";
        }

		public string GetFtpOthersFullBrowserPathFromRawPath(string inputPath, string fileCurrentStatusWiseLocation, string orderNumber)
		{
			if (string.IsNullOrWhiteSpace(inputPath))
			{
				return "";
			}

            return inputPath.Replace($"/Raw/{orderNumber}/", $"/{fileCurrentStatusWiseLocation}/{orderNumber}/");
		}

		/// <summary>
		/// Get Full File Path
		/// </summary>
		/// <param name="fileCurrentStatusWiseLocation"></param>
		/// <returns></returns>
		public string GetFtpFullFilePath(string subdirectory, string filename, string subFolder = null)
        {
            if (!string.IsNullOrWhiteSpace(subFolder))
            {
				return $"{subFolder}{subdirectory}{filename}";
			}
            else
            { 
                return $"{subdirectory}{filename}";
			}
		}

        public string GetFtpFileDisplayInUIPath(string physicalFptDirectoryBrowsePath)
        {
            if (string.IsNullOrWhiteSpace(physicalFptDirectoryBrowsePath))
            {
                return "";
            }

            return physicalFptDirectoryBrowsePath.Replace("\\", "/");
        }

        public string FptFilePhysicalDirectoryBrowsePath(string displayInUIPath)
        {
            if (string.IsNullOrWhiteSpace(displayInUIPath))
            {
                return "";
            }

            return displayInUIPath.Replace("/", "\\");
        }

		public async Task<string> GetFileNameWithoutExtension(string path)
		{
            var fileName = "";

			if (!string.IsNullOrEmpty(path))
            {
				 fileName = Path.GetFileNameWithoutExtension(path);
			}
			return fileName;
		}
        /// <summary>
        /// If Folder Exist Return Path
        /// Or Folder is not exist Firstly Create the folder and return path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
		public async Task<string> ExistsFolderDelete(string folderPath)
		{
			int tryCount = 0;
			var result = "";
			while (tryCount <= 3)
			{
				try
				{
					if (Directory.Exists(folderPath))
					{
						DirectoryInfo directory = new DirectoryInfo(folderPath);
						directory.Delete(true);
						result = folderPath;
						break;
					}
					else
					{
						break;
					}
				}
				catch (Exception ex)
				{
					Thread.Sleep(1000);
					tryCount++;
				}
			}
			return result;
		}
		
	}
}
