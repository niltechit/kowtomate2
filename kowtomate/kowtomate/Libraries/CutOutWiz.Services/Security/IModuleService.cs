using CutOutWiz.Data;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Services.Security
{
    public interface IModuleService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<Module>> GetAll();
        /// <summary>
        /// Get All Modules with details
        /// </summary>
        /// <returns></returns>
        Task<List<Module>> GetAllWithDetails();
        Task<Module> GetById(int moduleId);
        Task<Module> GetByObjectId(string objectId);
        Task<Response<int>> Insert(Module module);
        Task<Response<bool>> Update(Module module);

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

        /// <summary>
        /// Get tree nodes
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        List<TreeNode> GetTreeNodes(List<ModulePermissionViewModel> permissions);

        #endregion
    }
}
