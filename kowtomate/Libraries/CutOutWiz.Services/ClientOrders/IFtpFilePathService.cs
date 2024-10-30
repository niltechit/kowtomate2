
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.ClientOrders
{
    public interface IFtpFilePathService
    {
        string GetFtpRootFolderPathUptoOrderNumber(string companyCode, DateTime currentDateTime, string orderNumber, string fileCurrentStatusWiseLocation);
        
        string GetFtpOthersFullBrowserPathFromRawPath(string inputPath, string fileCurrentStatusWiseLocation, string orderNumber);

		/// <summary>
		/// Get Full File Path
		/// </summary>
		/// <param name="fileCurrentStatusWiseLocation"></param>
		/// <returns></returns>
		string GetFtpFullFilePath(string subdirectory, string filename, string subFolder = null);

        /// <summary>
        /// Get Full File Path
        /// </summary>
        /// <param name="fileCurrentStatusWiseLocation"></param>
        /// <returns></returns>
        string GetFtpFileDisplayInUIPath(string physicalFptDirectoryBrowsePath);
        string FptFilePhysicalDirectoryBrowsePath(string displayInUIPath);
        Task<string> GetFileNameWithoutExtension(string path);
        Task<string> ExistsFolderDelete(string folderPath);
    }
}