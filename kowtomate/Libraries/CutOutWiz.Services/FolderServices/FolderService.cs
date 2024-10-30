
namespace CutOutWiz.Services.FolderServices
{
	public class FolderService : IFolderService
	{
		public async Task CreateFolder(string folderName)
		{
			string folderPath = folderName;
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
		}
		
	}
}
