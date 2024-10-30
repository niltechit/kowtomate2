using CutOutWiz.Core;
using CutOutWiz.Services.Models.Email;

namespace CutOutWiz.Services.Email
{
    public interface IArchiveQueueEmailService
    {
        Task<Response<int>> AddAchiveEmail(ArchiveQueueEmailModel archiveQueueEmail);
        Task<Response<int>> AddQueueEmail(ArchiveQueueEmailModel archiveQueueEmail);
    }
}