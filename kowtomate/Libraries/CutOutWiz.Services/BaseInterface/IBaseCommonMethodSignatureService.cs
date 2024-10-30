using CutOutWiz.Services.Models.HR;
using CutOutWiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.BaseInterface
{
	public interface IBaseCommonMethodSignatureService<T>
	{
		Task<Response<bool>> Delete(string objectId);
		Task<List<T1>> GetAll<T1>() where T1 : class;
        Task<T> GetById(int id);
		Task<T> GetByObjectId(string objectId);
		Task<Response<int>> Insert(T entity);
		Task<Response<bool>> Update(T entity);
	}

}
