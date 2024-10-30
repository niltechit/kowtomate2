using CutOutWiz.Core.Management;
using CutOutWiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Services.DbAccess;

namespace CutOutWiz.Services.Management
{
    public interface IManageTeamMemberChangelogService
    { 
        Task<Response<int>> Insert(List<ManageTeamMemberChangeLogModel> manageTeamMemberChangelog);
        Task<Response<int>> Update(ManageTeamMemberChangeLogModel manageTeamMemberChangelog);
    }
}
