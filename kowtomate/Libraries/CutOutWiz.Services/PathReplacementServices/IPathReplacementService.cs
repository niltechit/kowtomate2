using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.PathReplacements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.PathReplacementServices
{
	public interface IPathReplacementService
	{
		
		Task<Response<int>> Insert(PathReplacementModel pathReplacements);
		Task<Response<bool>> Update(PathReplacementModel pathReplacements);
		Task<Response<bool>> Delete(int Id);
		Task<List<PathReplacementModel>> GetPathReplacements(int CompnayId);
		/// <summary>
		/// Replace Path
		/// </summary>
		/// <param name="orderPath"></param>
		/// <param name="pathReplacements"></param>
		/// <param name="order"></param>
		/// <returns></returns>
		Task<string> Replace(string orderPath, List<PathReplacementModel> pathReplacements, ClientOrderModel order=null);
		/// <summary>
		/// Remove Order Number from path
		/// </summary>
		/// <param name="path"></param>
		/// <param name="pathReplacement"></param>
		/// <returns></returns>
		Task<string> RemoveOrderNumber(string path, string orderNumber);
		Task<string> TakeBatchNameFromPath(string path, int level);

    }
}
