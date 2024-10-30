using CutOutWiz.Core;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Data;

namespace CutOutWiz.Services.Security
{
    public class ModuleService:IModuleService
    {
        private readonly ISqlDataAccess _db;

        public ModuleService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Modules
        /// </summary>
        /// <returns></returns>
        public async Task<List<ModuleModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<ModuleModel, dynamic>(storedProcedure: "dbo.SP_Security_Module_GetAll", new { });
        }

        /// <summary>
        /// Get All Modules with details
        /// </summary>
        /// <returns></returns>
        public async Task<List<ModuleModel>> GetAllWithDetails()
        {
            return await _db.LoadDataUsingProcedure<ModuleModel, dynamic>(storedProcedure: "dbo.SP_Security_Module_GetAllWithDetails", new { });
        }
        /// <summary>
        /// Get module by module Id
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <returns></returns>
        public async Task<ModuleModel> GetById(int moduleId)
        {
            var result = await _db.LoadDataUsingProcedure<ModuleModel, dynamic>(storedProcedure: "dbo.SP_Security_Module_GetById", new { ModuleId = moduleId });
            return result.FirstOrDefault();
        }
        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <returns></returns>
        public async Task<ModuleModel> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<ModuleModel, dynamic>(storedProcedure: "dbo.SP_Security_Module_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }
        /// <summary>
        /// Insert module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(ModuleModel module)
        {
            var response = new Response<int>();

            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.SP_Security_Module_Insert", new
                {
                    module.Name,
                    module.Icon,
                    module.Status,
                    module.CreatedByContactId,
                    module.ObjectId
                });

                module.Id = (short)newId;
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
        /// Update Module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(ModuleModel module)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_Module_Update", new
                {
                    module.Id,
                    module.Name,
                    module.Icon,
                    module.DisplayOrder,
                    module.Status,
                    module.UpdatedByContactId
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
        /// Delete Module by id
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_Module_Delete", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }


        #region Module Permissions
        /// <summary>
        /// Get Permissions by Module object Id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<List<string>> GetPermissionsByModuleObjectId(string moduleObjectId)
        {
            var query = "SELECT PermissionObjectId FROM [dbo].[Security_ModulePermission] p WITH(NOLOCK) WHERE p.ModuleObjectId = @ModuleObjectId";

            return await _db.LoadDataUsingQuery<string, dynamic>(query, new { ModuleObjectId = moduleObjectId });
        }

        /// <summary>
        /// Insert module permissions by module object id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<Response<bool>> ModulePermissionInsertOrUpdateByModuleObjectId(string moduleObjectId, List<string> permissionObjectIds, int updatedByContactId)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_ModulePermission_InsertOrUpdateByModuleObjectId", new
                {
                    ModuleObjectId = moduleObjectId,
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

        /// <summary>
        /// Get All Modules with details
        /// </summary>
        /// <returns></returns>

        public async Task<List<ModulePermissionViewModel>> GetModuleAllPermissions()
        {
            return await _db.LoadDataUsingProcedure<ModulePermissionViewModel, dynamic>(storedProcedure: "dbo.SP_Security_ModulePermission_GetAll", new { });
        }

        public async Task<List<ModulePermissionViewModel>> GetModuleAllPermissions(byte companyType)
        {
            return await _db.LoadDataUsingProcedure<ModulePermissionViewModel, dynamic>(storedProcedure: "dbo.SP_Security_ModulePermission_GetAll", new { CompanyType = companyType });
        }

        public List<TreeNode> GetTreeNodes(List<ModulePermissionViewModel> permissions)
        {
            var parentNodes = permissions.GroupBy(g => g.ModuleObjectId).Select(s => s.First()).ToList();

            var nodes = parentNodes.Select(n => new TreeNode { NodeType= "Module", Id = n.ModuleObjectId, Name = n.ModuleName }).ToList();

            foreach(var parent in nodes)
            {
                parent.ChildNodes = permissions.Where(f => f.ModuleObjectId == parent.Id).OrderBy(o=>o.DisplayOrder).Select(n => new TreeNode { NodeType = "Permission", Id = n.PermissionObjectId,
                    Name = n.DisplayName }).ToList();
            }

            return nodes;
        }

        #endregion
    }
}
