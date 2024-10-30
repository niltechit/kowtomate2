using CutOutWiz.Core;
using CutOutWiz.Services.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Logs
{
    public interface ILogServices
    {
        Task<List<ActivityLogModel>> GetAll();
        Task<List<ActivityLogModel>> GetAllByContactObjectId(string contactObjectId);
        Task<Response<int>> Insert(ActivityLogModel activityLog);
        Task<List<ActivityLogModel>> GetByActivityLogFor(byte activityLogFor,int primaryId);
    }
}
