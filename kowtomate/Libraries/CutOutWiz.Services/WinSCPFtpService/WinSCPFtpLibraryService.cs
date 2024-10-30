using CutOutWiz.Services.Models.FtpModels;
using FluentFTP.Helpers;
using System.IO;
using System.Text;
using WinSCP;

namespace CutOutWiz.Services.WinSCPFtpService
{
    public class WinSCPFtpLibraryService : IWinSCPFtpLibraryService
    {
        public async Task<List<string>> ListFilesRecursive(FtpCredentailsModel ftp, string remotePath)
        {
            var response = new List<string>();
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = ftp.Host,
                UserName = ftp.UserName,
                Password = ftp.Password,
            };

            // Add raw settings to ensure ISO-8859-1 encoding
            sessionOptions.AddRawSettings("Charset", "ISO-8859-1");

            using (Session session = new Session())
            {
                try
                {
                    // Enable logging for debugging purposes
                    session.SessionLogPath = "winscp.log";

                    // Connect
                    session.Open(sessionOptions);

                    // List files recursively starting from the remotePath
                    var fileList = ListFilesRecursively(session, "/Automated/Otto/24-04-25_Otto_LAYDOWN_1_4_16_New/");
                    response.AddRange(fileList);
                }
                catch (Exception ex)
                {
                    // Handle exception
                    Console.WriteLine("Error: " + ex.Message);
                    // Optionally, log the exception details or handle it differently based on your application's requirements.
                }
            }
            return response;
        }

        static List<string> ListFilesRecursively(Session session, string remotePath)
        {
            List<string> fileList = new List<string>();

            // List directory
            RemoteDirectoryInfo directoryInfo = session.ListDirectory(remotePath);
            Console.WriteLine(directoryInfo);

            // Process files and directories
            foreach (RemoteFileInfo fileInfo in directoryInfo.Files)
            {
                if (fileInfo.IsDirectory && fileInfo.Name != "." && fileInfo.Name != "..")
                {
                    // It's a directory, recurse into it
                    List<string> subDirectoryFiles = ListFilesRecursively(session, fileInfo.FullName);
                    fileList.AddRange(subDirectoryFiles);
                }
                else if (!fileInfo.IsDirectory)
                {
                    Console.WriteLine(fileInfo.FullName);
                    // It's a file, add its full name to the list
                    fileList.Add(CorrectFilenameEncoding(fileInfo.FullName));
                }
            }

            return fileList;
        }
        // Method to correct the filename encoding
        static string CorrectFilenameEncoding(string incorrectlyEncodedFilename)
        {
            return incorrectlyEncodedFilename.Replace("ß", "ÃŸ");
        }
    }
}
