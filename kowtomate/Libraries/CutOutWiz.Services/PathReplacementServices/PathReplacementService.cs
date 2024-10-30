using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.PathReplacements;
using CutOutWiz.Services.DbAccess;
using System.Globalization;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Data;

namespace CutOutWiz.Services.PathReplacementServices
{
	public class PathReplacementService : IPathReplacementService
	{
		private readonly ISqlDataAccess _db;
		public PathReplacementService(ISqlDataAccess db)
		{
			_db = db;
		}
		/// <summary>
		/// Delete company pathreplacement path company.
		/// </summary>
		/// <param name="Id"></param>
		/// <returns></returns>
		public async Task<Response<bool>> Delete(int Id)
		{
			var response = new Response<bool>();
			try
			{
				// Add Order_ClientOrder
				var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_PathReplacement_Delete", new
				{
					Id = Id,
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
		/// Get all pathreplacement path list by companyid
		/// </summary>
		/// <param name="CompnayId"></param>
		/// <returns></returns>
		public async Task<List<PathReplacementModel>> GetPathReplacements(int CompnayId)
		{
			var result = await _db.LoadDataUsingProcedure<PathReplacementModel, dynamic>(storedProcedure: "dbo.SP_PathReplacement_Gets", new { CompnayId = CompnayId });
			return result;
		}
		/// <summary>
		/// Insert for pathreplacemnet.
		/// </summary>
		/// <param name="pathReplacements"></param>
		/// <returns></returns>
		public async Task<Response<int>> Insert(PathReplacementModel pathReplacements)
		{
			var response = new Response<int>();
			try
			{
				// Add Order_ClientOrder
				var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_PathReplacement_Insert", new
				{
					pathReplacements.ExecutionOrder,
					pathReplacements.Level,
					pathReplacements.NewText,
					pathReplacements.OldText,
					pathReplacements.Type,
					pathReplacements.CompanyId,
					pathReplacements.CreatedDate,
					pathReplacements.UpdatedDate,
					pathReplacements.IsActive,
					pathReplacements.IsDeleted,
					pathReplacements.DateFormat,
				});


				pathReplacements.Id = newId;
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
		/// Update pathreplacement 
		/// </summary>
		/// <param name="pathReplacements"></param>
		/// <returns></returns>
		public async Task<Response<bool>> Update(PathReplacementModel pathReplacements)
		{
			var response = new Response<bool>();
			try
			{
				// Add Order_ClientOrder
				var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_PathReplacement_Update", new
				{
					pathReplacements.Id,
					pathReplacements.ExecutionOrder,
					pathReplacements.Level,
					pathReplacements.NewText,
					pathReplacements.OldText,
					pathReplacements.Type,
					pathReplacements.CompanyId,
					pathReplacements.UpdatedDate,
					pathReplacements.IsActive,
					pathReplacements.IsDeleted,
					pathReplacements.DateFormat,
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
		public async Task<string> Replace(string orderPath, List<PathReplacementModel> pathReplacements, ClientOrderModel order = null)
		{
			var returnPath = orderPath;
			foreach (var path in pathReplacements)
			{
				if (path.Type == (int)PathReplacementType.RemoveOrderNo)
				{
					returnPath = await RemoveOrderNumber(returnPath, order.OrderNumber);

				}
				if (path.Type == (int)PathReplacementType.RemoveOrderDate)
				{
					returnPath = await RemoveOrderDate(returnPath, path);

				}
				if (path.Type == (int)PathReplacementType.ReplacePath)
				{
					returnPath = await ReplacePath(returnPath, path);

				}
				if (path.Type == (int)PathReplacementType.CategoryPath)
				{
					returnPath = await CategoryPath(returnPath, path);
				}
				if (path.Type == (int)PathReplacementType.RemoveString)
				{
					returnPath = await RemoveString(returnPath, path);
				}
				if (path.Type == (int)PathReplacementType.TakeFacilityNameFromPath)
				{
					returnPath = await TakeFacilityName(returnPath, path);
				}
				if (path.Type == (int)PathReplacementType.SubstractDuplicateFacilityNameFromPath)
				{
					returnPath = await SubstractFacilityNameFromPath(returnPath, path);
				}

			}
			return returnPath;
		}
		public async Task<string> RemoveOrderNumber(string path, string orderNumber)
		{
			if (path.Contains(orderNumber, StringComparison.OrdinalIgnoreCase))
			{
				return path.Replace(orderNumber + "/", "");
			}
			else
			{
				return path;
			}
		}

		private async Task<string> RemoveOrderDate(string path, PathReplacementModel pathReplacement)
		{
			//var splitPath = path.Split('/');
			//var orderDateToRemove = pathReplacement.CreatedDate;
			//for (int i = 0; i < splitPath.Length; i++)
			//{
			//    if (splitPath[i].Length == pathReplacement.Level)
			//    {
			//        splitPath[i] = "";
			//        break;
			//    }
			//}
			//var modifiedPath = string.Join("/", splitPath);

			//return modifiedPath;

			var newPath = path;

			string[] segments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

			if (pathReplacement.Level >= 0 && pathReplacement.Level < segments.Length)
			{
				if (DateTime.TryParseExact(segments[pathReplacement.Level], pathReplacement.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
				{
					// Remove the segment at the specified index
					segments[pathReplacement.Level] = "";
                    if (path.StartsWith('/'))
                    {
                        return newPath = "/" + string.Join("/", segments).Replace(@"//", "/");
					}
					else
					{
						return newPath = string.Join("/", segments).Replace(@"//", "/");
					}

				}
			}
			return newPath;
		}

		private async Task<string> ReplacePath(string path, PathReplacementModel pathReplacement)
		{
			if (path.ToLower().Contains(pathReplacement.OldText.ToLower(), StringComparison.OrdinalIgnoreCase))
			{
				string[] segments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

				if (pathReplacement.Level >= 0 && pathReplacement.Level < segments.Length)
				{
					var pat = segments[pathReplacement.Level];
					if (pat.Contains("_"))
					{
						string[] segment = pat.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

						segment[0] = pathReplacement.NewText;

						pat = string.Join("_", segment);
						segments[pathReplacement.Level] = pat;
					}
					else
					{
						segments[pathReplacement.Level] = pathReplacement.NewText;

					}

					if (path.StartsWith("/"))
					{
						path = "/" + string.Join("/", segments).Replace(@"//", "/");

						return path.Replace(@"//", "/");
					}

					return string.Join("/", segments).Replace(@"//", "/");
				}
				return path;
			}
			else
			{
				return path;
			}

		}
		private async Task<string> CategoryPath(string path, PathReplacementModel pathReplacement)
		{
			if (path.ToLower().Contains(pathReplacement.NewText.ToLower(), StringComparison.OrdinalIgnoreCase))
			{
				string[] segments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

				if (pathReplacement.Level >= 0 && pathReplacement.Level < segments.Length)
				{
					var pat = segments[pathReplacement.Level];
					if (pat.Contains(pathReplacement.NewText))
					{
						int startIndex = 0;
						int endIndex = pathReplacement.Level;

						var segnmetn = string.Join("/", segments, startIndex, endIndex - startIndex + 1).Replace(@"//", "/");
						var addFileName = $"/{segnmetn}/{segments.Last()}";
						return addFileName;
					}
				}
				return path;
			}
			else
			{
				return path;
			}
		}

		private async Task<string> RemoveString(string path, PathReplacementModel pathReplacement)
		{
			string[] segments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

			if (pathReplacement.Level >= 0 && pathReplacement.Level < segments.Length)
			{

				segments[pathReplacement.Level] = pathReplacement.NewText;

				if (path.StartsWith("/"))
				{
					path = "/" + string.Join("/", segments).Replace(@"//", "/");

					return path.Replace(@"//", "/");
				}

				path = "/" + string.Join("/", segments).Replace(@"//", "/");

				return path;
			}
			return path;
		}
        private async Task<string> TakeFacilityName(string path, PathReplacementModel pathReplacement)
        {
            string[] segments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (pathReplacement.Level >= 0 && pathReplacement.Level < segments.Length)
            {

                path = "/" + string.Join("/", segments[pathReplacement.Level]).Replace(@"//", "/");

                return path;
            }
            return path;
        }
        private async Task<string> SubstractFacilityNameFromPath(string path, PathReplacementModel pathReplacement)
        {
            string[] segments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (pathReplacement.Level >= 0 && pathReplacement.Level < segments.Length)
            {

                segments[pathReplacement.Level] = pathReplacement.NewText;

                if (path.StartsWith("/"))
                {
                    path = "/" + string.Join("/", segments).Replace(@"//", "/");

                    return path.Replace(@"//", "/");
                }

                path = "/" + string.Join("/", segments).Replace(@"//", "/");

                return path;
            }
            return path;
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path">Provide Full Path</param>
		/// <param name="level">Take Name From Fullpath By Level</param>
		/// <returns></returns>
        public async Task<string> TakeBatchNameFromPath(string path, int level)
        {
            string[] segments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (level >= 0 && level < segments.Length)
            {
                if (path.StartsWith("/"))
                {
                    path = "/" + string.Join("/", segments[level]).Replace(@"//", "/");

                    return path.Replace(@"//", "/");
                }

                path = "/" + string.Join("/", segments).Replace(@"//", "/");

                return path;
            }
            return path;
        }
    }
}
