using CutOutWiz.Data;
using CutOutWiz.Data.Security;

namespace CutOutWiz.Services.Security
{
    public interface IMenuService
    {
        Task<Response<bool>> Delete(string objectId);
        Task<List<Menu>> GetAll();
        Task<List<MenuListModel>> GetListWithDetails();
        /// <summary>
        /// Get Side Menu by Login User Object Id
        /// </summary>
        /// <returns></returns>
        Task<List<SideMenuListModel>> GetSideMenuByUserObjectId(string UserObjectId);
        Task<Menu> GetById(int menuId);
        Task<Menu> GetByObjectId(string objectId);
        Task<Response<int>> Insert(Menu menu);
        Task<Response<bool>> Update(Menu menu);

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