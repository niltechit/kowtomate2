using CutOutWiz.Core;
using CutOutWiz.Services.Models.Security;

namespace CutOutWiz.Services.Security
{
    public interface IMenuService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<MenuModel>> GetAll();
        Task<List<MenuListModel>> GetListWithDetails();
        /// <summary>
        /// Get Side Menu by Login User Object Id
        /// </summary>
        /// <returns></returns>
        Task<List<SideMenuListModel>> GetSideMenuByUserObjectId(string UserObjectId);
        Task<MenuModel> GetById(int menuId);
        Task<MenuModel> GetByObjectId(string objectId);
        Task<Response<int>> Insert(MenuModel menu);
        Task<Response<bool>> Update(MenuModel menu);

        #region Menu Permissions
        /// <summary>
        /// Get Permissions by Menu object Id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<List<string>> GetPermissionsByMenuObjectId(string menuObjectId);

        /// <summary>
        /// Insert menu permissions by menu object id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<Response<bool>> MenuPermissionInsertOrUpdateByMenuObjectId(string menuObjectId, List<string> permissionObjectIds, int updatedByContactId);
        #endregion
    }
}