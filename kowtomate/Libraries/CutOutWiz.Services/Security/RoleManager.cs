using CutOutWiz.Core;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.MapperHelper;
using CutOutWiz.Data.Repositories.Security;
using CutOutWiz.Data.Security;
using CutOutWiz.Data.Models.Security;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CutOutWiz.Services.Security
{
    public class RoleManager : IRoleManager
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapperHelperService _mapperHelperService;

        public RoleManager(IRoleRepository roleRepository,
            IMapperHelperService mapperHelperService)
        {
            _mapperHelperService = mapperHelperService;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleModel>> GetAll(string companyObjectId)
        {
            var entities = await _roleRepository.GetAll(companyObjectId);
            return await _mapperHelperService.MapToListAsync<RoleEntity, RoleModel>(entities);
        }

        /// <summary>
        /// Get role by role Id
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        public async Task<RoleModel> GetById(int roleId)
        {
            var entity = await _roleRepository.GetById(roleId);
            return await _mapperHelperService.MapToSingleAsync<RoleEntity, RoleModel>(entity);
        }

        public async Task<RoleModel> GetByName(string roleName)
        {
            var entity = await _roleRepository.GetByName(roleName);
            return await _mapperHelperService.MapToSingleAsync<RoleEntity, RoleModel>(entity);
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<RoleModel> GetByObjectId(string objectId)
        {
            var entity = await _roleRepository.GetByObjectId(objectId);
            return await _mapperHelperService.MapToSingleAsync<RoleEntity, RoleModel>(entity);
        }

        /// <summary>
        /// Insert role
        /// </summary>
        /// <param name="roleModel"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(RoleModel roleModel)
		{
			var validateResponse = ValidateRole(roleModel);

            if (!validateResponse.IsSuccess)
            {
                return validateResponse;
            }

			var roleEntity = await _mapperHelperService.MapToSingleAsync<RoleModel, RoleEntity>(roleModel);

			return await _roleRepository.Insert(roleEntity);
		}


		private Response<int> ValidateRole(RoleModel roleModel)
		{
            var validateResponse = new Response<int>();

			//Null  validation
			if (roleModel != null)
			{
				validateResponse.Message = "Role should not be empty.";
				return validateResponse;
			}

			//Null  validation
			if (string.IsNullOrEmpty(roleModel.Name))
			{
				validateResponse.Message = "Role Name Required.";
				return validateResponse;
			}

			validateResponse.IsSuccess = true;

            return validateResponse;
		}

		/// <summary>
		/// Update RoleModel
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public async Task<Response<bool>> Update(RoleModel roleModel)
        {
			var mainResponse = new Response<bool>();

			var validateResponse = ValidateRole(roleModel);

			if (!validateResponse.IsSuccess)
			{
                mainResponse.Message = validateResponse.Message;
				return mainResponse;
			}

			var roleEntity = await _mapperHelperService.MapToSingleAsync<RoleModel, RoleEntity>(roleModel);
            return await _roleRepository.Update(roleEntity);
        }

        /// <summary>
        /// Delete RoleModel by id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            return await _roleRepository.Delete(objectId);
        }
        /// <summary>
        /// Get All UserRoles
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserRoleModel>> GetAllUserRole(string roleObjectId)
        {
            var entities = await _roleRepository.GetAllUserRole(roleObjectId);
			return await _mapperHelperService.MapToListAsync<UserRoleEntity, UserRoleModel>(entities);
		}

        public async Task<List<string>> GetUserRoleByUserObjectId(string objectId)
        {
            return await _roleRepository.GetUserRoleByUserObjectId(objectId);
        }

        #region RoleModel Permissions
        /// <summary>
        /// Get Permissions by RoleModel object Id
        /// </summary>
        /// <param name="roleObjectId"></param>
        /// <returns></returns>
        public async Task<List<string>> GetPermissionsByRoleObjectId(string roleObjectId)
        {
            return await _roleRepository.GetPermissionsByRoleObjectId(roleObjectId);
        }

        /// <summary>
        /// Insert role permissions by role object id
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<Response<bool>> RolePermissionInsertOrUpdateByRoleObjectId(string roleObjectId, List<string> permissionObjectIds, int updatedByContactId)
        {
            return await _roleRepository.RolePermissionInsertOrUpdateByRoleObjectId(roleObjectId, permissionObjectIds, updatedByContactId);
        }
		#endregion

		public async Task<List<RoleModel>> GetAllUserRoleByContactObjectId(string contactObjectId)
		{
            var roleEntities = await _roleRepository.GetAllUserRoleByContactObjectId(contactObjectId);
            return await _mapperHelperService.MapToListAsync<RoleEntity, RoleModel>(roleEntities);
        }
	}
}
