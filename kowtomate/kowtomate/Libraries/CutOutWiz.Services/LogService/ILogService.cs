using CutOutWiz.Data.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.LogService
{
    public interface ILogService
    {
        void Log(string message);
        LogModel GetLogsByDate(DateTime date);
    }
}
