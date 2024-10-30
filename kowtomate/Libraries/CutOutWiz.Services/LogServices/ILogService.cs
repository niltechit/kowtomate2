using CutOutWiz.Core.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.LogServices
{
    public interface ILogService
    {
        void Log(string message);
        LogModel GetLogsByDate(DateTime date);
    }
}
