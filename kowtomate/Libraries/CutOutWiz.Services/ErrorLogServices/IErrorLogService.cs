using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.ErrorLogServices
{
    public interface IErrorLogService
    {
        Task LogFtpProcessingError(Exception ex, string methodName, byte category);
        Task LogGeneralError(Exception ex, string methodName, byte category);
    }
}
