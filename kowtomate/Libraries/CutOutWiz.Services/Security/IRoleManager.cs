using CutOutWiz.Core;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Security
{
    public interface IRoleManager
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<RoleModel>> GetAll(string companyObjectId);
        Task<RoleModel> GetById(int roleId);
        Task<RoleModel> GetByObjectId(string objectId);
        Task<Response<int>> Insert(RoleModel role);
        Task<Response<bool>> Update(RoleModel role);
        Task<List<UserRoleModel>> GetAllUserRole(string roleObjectId);

        Task<List<string>> GetUserRoleByUserObjectId(string objectId);

        #region RoleModel Permissions
        /// <summary>
        /// Get Permissions by RoleModel object Id
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
		Task<List<RoleModel>> GetAllUserRoleByContactObjectId(string contactObjectId);
        Task<RoleModel> GetByName(string roleName);

    }
}
