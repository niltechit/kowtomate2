using CutOutWiz.Core;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Security
{
    public interface IModuleService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<ModuleModel>> GetAll();
        /// <summary>
        /// Get All Modules with details
        /// </summary>
        /// <returns></returns>
        Task<List<ModuleModel>> GetAllWithDetails();
        Task<ModuleModel> GetById(int moduleId);
        Task<ModuleModel> GetByObjectId(string objectId);
        Task<Response<int>> Insert(ModuleModel module);
        Task<Response<bool>> Update(ModuleModel module);

        #region Module Permissions
        /// <summary>
        /// Get Permissions by Module object Id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<List<string>> GetPermissionsByModuleObjectId(string moduleObjectId);
        /// <summary>
        /// Insert module permissions by module object id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<Response<bool>> ModulePermissionInsertOrUpdateByModuleObjectId(string moduleObjectId, List<string> permissionObjectIds, int updatedByContactId);
        /// <summary>
        /// Get All Modules with permissions
        /// </summary>
        /// <returns></returns>
        Task<List<ModulePermissionViewModel>> GetModuleAllPermissions();
        Task<List<ModulePermissionViewModel>> GetModuleAllPermissions(byte companyType);
        /// <summary>
        /// Get tree nodes
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        List<TreeNode> GetTreeNodes(List<ModulePermissionViewModel> permissions);

        #endregion
    }
}
