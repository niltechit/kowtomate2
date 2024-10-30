using CutOutWiz.Data;
using System.Data;

namespace CutOutWiz.Services.DataService
{
    public interface IFileTrackingService
    {
        Task<List<FileTracking>> GetAllFileTracking();
        Task<List<FileTracking>> GetFileTrackingByCompanyId(int companyId, string actionType);
        Task<List<FileTrackingDetails>> GetFileTrackingDetails(int companyId);
        Task InsertFileTracking(DataTable fileTrackingType);
        Task InsertFileTrackingDetails(FileTrackingDetails details);
    }
}