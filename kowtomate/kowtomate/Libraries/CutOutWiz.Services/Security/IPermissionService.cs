using CutOutWiz.Data;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Services.Security
{
    public interface IPermissionService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<Permission>> GetAll();
        Task<List<PermissionListModel>> GetAllWithDetails();
        Task<Permission> GetById(int permissionId);
        Task<Permission> GetByObjectId(string objectId);
        Task<Response<int>> Insert(Permission permission);
        Task<Response<bool>> Update(Permission permission);

        /// <summary>
        ///  Get All Permissions by User Id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<List<Permission>> GetAllByUserId(string userObjectId);

        #region Menu Permissions
        ///// <summary>
        ///// Insert menu permissions by menu id
        ///// </summary>
        ///// <param name="permission"></param>
        ///// <returns></returns>
        //Task<Response<bool>> MenuPermissionInsertOrUpdateByPermissionid(string menuObjectId, List<string> menuObjectIds, int updatedByContactId);        
        #endregion

        #region Module Permissions
        /// <summary>
        /// Get Modules by Permission object Id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<List<string>> GetModulesByPermissionObjectId(string permissionObjectId);

        /// <summary>
        /// Insert module permissions by permission id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<Response<bool>> ModulePermissionInsertOrUpdateByPermissionObjectId(string permissionObjectId, List<string> moduleObjectIds, int updatedByContactId);
        #endregion

        #region Company TYpe Permissions
        /// <summary>
        /// Get Modules by Permission object Id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<List<string>> GetCompanyTypesByPermissionObjectId(string permissionObjectId);

        /// <summary>
        /// Insert module permissions by permission id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<Response<bool>> CompanyTypePermissionInsertOrUpdateByPermissionObjectId(string permissionObjectId, List<string> companyTypeIds, int updatedByContactId);
        #endregion
    }
}
