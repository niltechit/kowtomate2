using CutOutWiz.Data;
using CutOutWiz.Data.Security;
using CutOutWiz.Services.DbAccess;

namespace CutOutWiz.Services.Security
{
    public class PermissionService : IPermissionService
    {
        private readonly ISqlDataAccess _db;

        public PermissionService(ISqlDataAccess db)
        {
            _db = db;
        }

        #region Permision
        /// <summary>
        /// Get All Permissions
        /// </summary>
        /// <returns></returns>
        public async Task<List<Permission>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<Permission, dynamic>(storedProcedure: "dbo.SP_Security_Permission_GetAll", new { });
        }

        /// <summary>
        /// Get All Permissions
        /// </summary>
        /// <returns></returns>
        public async Task<List<PermissionListModel>> GetAllWithDetails()
        {
            return await _db.LoadDataUsingProcedure<PermissionListModel, dynamic>(storedProcedure: "[dbo].[SP_Security_Permission_GetAllWithDetails]", new { });
        }

        /// <summary>
        /// Get permission by permission Id
        /// </summary>
        /// <param name="PermissionId"></param>
        /// <returns></returns>
        public async Task<Permission> GetById(int permissionId)
        {
            var result = await _db.LoadDataUsingProcedure<Permission, dynamic>(storedProcedure: "dbo.SP_Security_Permission_GetById", new { PermissionId = permissionId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="PermissionId"></param>
        /// <returns></returns>
        public async Task<Permission> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<Permission, dynamic>(storedProcedure: "dbo.SP_Security_Permission_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert permission
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(Permission permission)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Security_Permission_Insert", new
                {
                    permission.Name,
                    permission.Value,
                    permission.Status,
                    permission.CreatedByContactId,
                    permission.ObjectId,
                });

                permission.Id = newId;
                response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Update Permission
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(Permission permission)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_Permission_Update", new
                {
                    permission.Id,
                    permission.Name,
                    permission.Value,
                    permission.Status,
                    permission.UpdatedByContactId
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Delete Permission by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_Permission_Delete", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        #endregion

        #region User Permission
        /// <summary>
        ///  Get All Permissions by User Id
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<List<Permission>> GetAllByUserId(string userObjectId)
        {
            return await _db.LoadDataUsingProcedure<Permission, dynamic>(storedProcedure: "dbo.SP_Security_Permission_GetAllByUserId", new { UserObjectId = userObjectId });
        }
        #endregion

        #region Menu Permissions
        ///// <summary>
        ///// Insert menu permissions by menu id
        ///// </summary>
        ///// <param name="permission"></param>
        ///// <returns></returns>
        //public async Task<Response<bool>> MenuPermissionInsertOrUpdateByPermissionid(string permissionObjectId, List<string> menuObjectIds, int updatedByContactId)
        //{
        //    var response = new Response<bool>();
        //    try
        //    {
        //        await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Security_MenuPermission_InsertOrUpdateByPermissionId", new
        //        {
        //            permissionObjectId = permissionObjectId,
        //            MenuObjectIds = menuObjectIds,
        //            UpdatedByContactId = updatedByContactId
        //        });

        //        response.IsSuccess = true;
        //        response.Message = StandardDataAccessMessages.SuccessMessaage;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
        //    }

        //    return response;
        //}
        #endregion

        #region Module Permissions
        /// <summary>
        /// Get Modules by Permission object Id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<List<string>> GetModulesByPermissionObjectId(string permissionObjectId)
        {
            var query = "SELECT ModuleObjectId FROM [dbo].[Security_ModulePermission] p WITH(NOLOCK) WHERE p.PermissionObjectId = @PermissionObjectId";

            return await _db.LoadDataUsingQuery<string, dynamic>(query, new { PermissionObjectId = permissionObjectId });
        }

        /// <summary>
        /// Insert module permissions by permission id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<Response<bool>> ModulePermissionInsertOrUpdateByPermissionObjectId(string permissionObjectId, List<string> moduleObjectIds, int updatedByContactId)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_ModulePermission_InsertOrUpdateByPermissionObjectId", new
                {
                    PermissionObjectId = permissionObjectId,
                    ModuleObjectIds = string.Join(",", moduleObjectIds),
                    UpdatedByContactId = updatedByContactId
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }
        #endregion


        #region Company TYpe Permissions
        /// <summary>
        /// Get Modules by Permission object Id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<List<string>> GetCompanyTypesByPermissionObjectId(string permissionObjectId)
        {
            var query = "SELECT CompanyType FROM [dbo].[Security_CompanyTypePermission] p WITH(NOLOCK) WHERE p.PermissionObjectId = @PermissionObjectId";

            return await _db.LoadDataUsingQuery<string, dynamic>(query, new { PermissionObjectId = permissionObjectId });
        }

        /// <summary>
        /// Insert module permissions by permission id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<Response<bool>> CompanyTypePermissionInsertOrUpdateByPermissionObjectId(string permissionObjectId, List<string> companyTypeIds, int updatedByContactId)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_CompanyTypePermission_InsertOrUpdateByPermissionObjectId", new
                {
                    PermissionObjectId = permissionObjectId,
                    CompanyTypeIds = string.Join(",", companyTypeIds),
                    UpdatedByContactId = updatedByContactId
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }
        #endregion
    }
}
