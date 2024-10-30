
namespace CutOutWiz.Services.StorageService
{
    public class LocalFileService : ILocalFileService
    {
        public async Task<bool> UploadAsync(string path, MemoryStream memoryStream)
        {
            var result = false;
            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            { 
                memoryStream.WriteTo(fileStream);
				result= true;
			}
           return result;
        }
        public async Task DeleteFiles(string RootFolderPath)
        {
            DirectoryInfo directory = new DirectoryInfo(RootFolderPath);

            foreach (FileInfo file in directory.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception) { }
            }

            foreach (DirectoryInfo directories in directory.GetDirectories())
            {
                try
                {
                    directories.Delete();
                }
                catch (Exception) { } 
            }
        }
    }
}
