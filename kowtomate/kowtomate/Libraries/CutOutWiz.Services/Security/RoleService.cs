using CutOutWiz.Data;
using CutOutWiz.Data.Security;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Security
{
    public class RoleService : IRoleService
    {
        private readonly ISqlDataAccess _db;

        public RoleService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        public async Task<List<Role>> GetAll(string companyObjectId)
        {
            return await _db.LoadDataUsingProcedure<Role, dynamic>(storedProcedure: "dbo.SP_Security_Role_GetAll", new { CompanyObjectId = companyObjectId });
        }

        /// <summary>
        /// Get role by role Id
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public async Task<Role> GetById(int roleId)
        {
            var result = await _db.LoadDataUsingProcedure<Role, dynamic>(storedProcedure: "dbo.SP_Security_Role_GetById", new { RoleId = roleId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public async Task<Role> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<Role, dynamic>(storedProcedure: "dbo.SP_Security_Role_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(Role role)
        {
            var response = new Response<int>();

            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.SP_Security_Role_Insert", new
                {
                    role.Name,
                    role.Status,
                    role.CreatedByContactId,
                    role.ObjectId,
                    role.CompanyObjectId
                });

                role.Id = (short) newId;
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
        /// Update Role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(Role role)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_Role_Update", new
                {
                    role.Id,
                    role.Name,
                    role.Status,
                    role.UpdatedByContactId
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
        /// Delete Role by id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_Role_Delete", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }


        #region Role Permissions
        /// <summary>
        /// Get Permissions by Role object Id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<List<string>> GetPermissionsByRoleObjectId(string roleObjectId)
        {
            var query = "SELECT PermissionObjectId FROM [dbo].[Security_RolePermission] p WITH(NOLOCK) WHERE p.RoleObjectId = @RoleObjectId";

            return await _db.LoadDataUsingQuery<string, dynamic>(query, new { RoleObjectId = roleObjectId });
        }

        /// <summary>
        /// Insert role permissions by role object id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<Response<bool>> RolePermissionInsertOrUpdateByRoleObjectId(string roleObjectId, List<string> permissionObjectIds, int updatedByContactId)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_RolePermission_InsertOrUpdateByRoleObjectId", new
                {
                    RoleObjectId = roleObjectId,
                    PermissionObjectIds = string.Join(",", permissionObjectIds),
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
