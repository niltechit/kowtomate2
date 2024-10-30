using CutOutWiz.Core;
using CutOutWiz.Data.DbAccess;
using CutOutWiz.Data.Models.Security;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Data.Repositories.Security
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ISqlDataAccess _db;

        public RoleRepository(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleEntity>> GetAll(string companyObjectId)
        {
            return await _db.LoadDataUsingProcedure<RoleEntity, dynamic>(storedProcedure: "dbo.SP_Security_Role_GetAll", new { CompanyObjectId = companyObjectId });
        }

        /// <summary>
        /// Get role by role Id
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public async Task<RoleEntity> GetById(int roleId)
        {
            var result = await _db.LoadDataUsingProcedure<RoleEntity, dynamic>(storedProcedure: "dbo.SP_Security_Role_GetById", new { RoleId = roleId });
            return result.FirstOrDefault();
        }

        public async Task<RoleEntity> GetByName(string roleName)
        {
            var result = await _db.LoadDataUsingProcedure<RoleEntity, dynamic>(storedProcedure: "dbo.SP_Security_Role_GetByRoleName", new { RoleName = roleName });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public async Task<RoleEntity> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<RoleEntity, dynamic>(storedProcedure: "dbo.SP_Security_Role_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(RoleEntity role)
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

                role.Id = (short)newId;
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
        /// Update RoleEntity
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(RoleEntity role)
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
        /// Delete RoleEntity by id
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
        /// <summary>
        /// Get All UserRoles
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserRoleEntity>> GetAllUserRole(string roleObjectId)
        {
            return await _db.LoadDataUsingProcedure<UserRoleEntity, dynamic>(storedProcedure: "dbo.SP_Security_UserRole_GetAllByRoldeObjectId", new { roleObjectId });
        }

        public async Task<List<string>> GetUserRoleByUserObjectId(string objectId)
        {
            List<string> roleNames = new List<string>();
            try
            {
                var roleObjectIdList = await _db.LoadDataUsingProcedure<string, dynamic>(storedProcedure: "dbo.SP_Security_UserRole_GetUserRoleObjectIdByUserObjectId", new { ObjectId = objectId });
                
                foreach (var roleId in roleObjectIdList)
                {
                    var result = await _db.LoadDataUsingProcedure<RoleEntity, dynamic>(storedProcedure: "dbo.SP_Security_Role_GetByObjectId", new { ObjectId = roleId });
                    roleNames.Add(result.FirstOrDefault().Name);
                }
            }
            catch
            {
                return null;
            }
            return roleNames;
        }

        #region RoleEntity Permissions
        /// <summary>
        /// Get Permissions by RoleEntity object Id
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

        public async Task<List<RoleEntity>> GetAllUserRoleByContactObjectId(string contactObjectId)
        {
            return await _db.LoadDataUsingProcedure<RoleEntity, dynamic>(storedProcedure: "dbo.getUserRoleByContactId", new { contactObjectId });
        }

    }
}
