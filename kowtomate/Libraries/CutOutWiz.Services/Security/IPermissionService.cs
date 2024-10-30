using CutOutWiz.Core;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Security
{
    public interface IPermissionService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<PermissionModel>> GetAll();
        Task<List<PermissionListModel>> GetAllWithDetails();
        /// <summary>
        /// Get All Permissions By Permission Ids
        /// </summary>
        /// <returns></returns>
        Task<PermissionListModel> GetDetailsByPermisisonId(string permissionObjectId);
        Task<PermissionModel> GetById(int permissionId);
        Task<PermissionModel> GetByObjectId(string objectId);
        Task<Response<int>> Insert(PermissionModel permission);
        Task<Response<bool>> Update(PermissionModel permission);

        /// <summary>
        ///  Get All Permissions by User Id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<List<PermissionModel>> GetAllByUserId(string userObjectId);

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
        Task<List<PermissionModel>> GetAllPermissionByRoleObjectId(string roleObjectId);

	}
}
