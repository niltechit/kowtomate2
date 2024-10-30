using CutOutWiz.Core;
using CutOutWiz.Data.Models.Security;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Data.Repositories.Security
{
    public interface IRoleRepository
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<RoleEntity>> GetAll(string companyObjectId);
        Task<RoleEntity> GetById(int roleId);
        Task<RoleEntity> GetByObjectId(string objectId);
        Task<Response<int>> Insert(RoleEntity role);
        Task<Response<bool>> Update(RoleEntity role);
        Task<List<UserRoleEntity>> GetAllUserRole(string roleObjectId);

        Task<List<string>> GetUserRoleByUserObjectId(string objectId);

        #region RoleEntity Permissions
        /// <summary>
        /// Get Permissions by RoleEntity object Id
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
		Task<List<RoleEntity>> GetAllUserRoleByContactObjectId(string contactObjectId);
        Task<RoleEntity> GetByName(string roleName);

    }
}
