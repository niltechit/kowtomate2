using CutOutWiz.Core;
using CutOutWiz.Core.Models.CpanelStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.CpanelStorageInfoServices
{
    public interface ICpanelStorageInfoService
    {
        Task<Response<CpanelStorageInfoViewModel>> GetCpanelStorageInfo();
        Task<Response<List<ProjectWiseCpanelStorageInfoViewModel>>> GetCpanelStorageByProjectWise();
    }
}
