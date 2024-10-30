using CutOutWiz.Core;

namespace CutOutWiz.Services.AutomationAppServices.DeleteFilesFromNasAutomation
{
    public interface IDeleteFilesFromNasService
    {
        Task<Response<bool>> DeleteInprogressFileFromNas();
    }
}
