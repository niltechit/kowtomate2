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
    public class MenuService : IMenuService
    {
        private readonly ISqlDataAccess _db;

        public MenuService(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Get All Menus
        /// </summary>
        /// <returns></returns>
        public async Task<List<MenuModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<MenuModel, dynamic>(storedProcedure: "dbo.SP_Security_Menu_GetAll", new { });
        }

        /// <summary>
        /// Get All Menus
        /// </summary>
        /// <returns></returns>
        public async Task<List<MenuListModel>> GetListWithDetails()
        {
            return await _db.LoadDataUsingProcedure<MenuListModel, dynamic>(storedProcedure: "dbo.SP_Security_Menu_GetListWithDetails", new { });
        }

        /// <summary>
        /// Get Side Menu by Login User Object Id
        /// </summary>
        /// <returns></returns>
        public async Task<List<SideMenuListModel>> GetSideMenuByUserObjectId(string UserObjectId)
        {
            return await _db.LoadDataUsingProcedure<SideMenuListModel, dynamic>(storedProcedure: "dbo.SP_Security_Menu_GetMenusByUserObjectId", new { UserObjectId = UserObjectId });
        }

        /// <summary>
        /// Get menu by menu Id
        /// </summary>
        /// <param name="MenuId"></param>
        /// <returns></returns>
        public async Task<MenuModel> GetById(int menuId)
        {
            var result = await _db.LoadDataUsingProcedure<MenuModel, dynamic>(storedProcedure: "dbo.SP_Security_Menu_GetById", new { MenuId = menuId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="MenuId"></param>
        /// <returns></returns>
        public async Task<MenuModel> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<MenuModel, dynamic>(storedProcedure: "dbo.SP_Security_Menu_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert menu
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(MenuModel menu)
        {
            var response = new Response<int>();
            
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<int, dynamic>(storedProcedure: "dbo.SP_Security_Menu_Insert", new
                {
                    menu.Name,
                    menu.ParentId,
                    menu.Icon,
                    menu.IsLeftMenu,
                    menu.IsTopMenu,
                    menu.IsExternalMenu,
                    menu.MenuUrl,
                    menu.Status,
                    menu.CreatedByContactId,
                    menu.ObjectId,
                    menu.DisplayOrder
                });

                menu.Id = newId;
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
        /// Update Menu
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(MenuModel menu)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_Menu_Update", new
                {
                    menu.Id,
                    menu.Name,
                    menu.ParentId,
                    menu.Icon,
                    menu.IsLeftMenu,
                    menu.IsTopMenu,
                    menu.IsExternalMenu,
                    menu.MenuUrl,
                    menu.DisplayOrder,
                    menu.Status,
                    menu.UpdatedByContactId
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
        /// Delete Menu by id
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_Menu_Delete", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }


        #region Menu Permissions
        /// <summary>
        /// Get Permissions by Menu object Id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<List<string>> GetPermissionsByMenuObjectId(string menuObjectId)
        {
            var query = "SELECT PermissionObjectId FROM [dbo].[Security_MenuPermission] p WITH(NOLOCK) WHERE p.MenuObjectId = @MenuObjectId";

            return await _db.LoadDataUsingQuery<string, dynamic>(query, new { MenuObjectId = menuObjectId });
        }

        /// <summary>
        /// Insert menu permissions by menu object id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<Response<bool>> MenuPermissionInsertOrUpdateByMenuObjectId(string menuObjectId, List<string> permissionObjectIds, int updatedByContactId)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Security_MenuPermission_InsertOrUpdateByMenuObjectId", new
                {
                    MenuObjectId = menuObjectId,
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
