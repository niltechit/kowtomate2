using CutOutWiz.Data;
using CutOutWiz.Data.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Security
{
    public interface IRoleService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<Role>> GetAll(string companyObjectId);
        Task<Role> GetById(int roleId);
        Task<Role> GetByObjectId(string objectId);
        Task<Response<int>> Insert(Role role);
        Task<Response<bool>> Update(Role role);

        #region Role Permissions
        /// <summary>
        /// Get Permissions by Role object Id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<List<string>> GetPermissionsByRoleObjectId(string roleObjectId);

        /// <summary>
        /// Insert role permissions by role object id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<Response<bool>> RolePermissionInsertOrUpdateByRoleObjectId(string roleObjectId, List<string> permissionObjectIds, int updatedByContactId);
        #endregion
    }
}
