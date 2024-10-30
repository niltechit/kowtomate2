using CutOutWiz.Services.Models.SOP;
using CutOutWiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Services.DbAccess;

namespace CutOutWiz.Services.SOP
{
	public interface IOrderSOPTempleateFileService
	{
        Task<Response<int>> Insert(SOPTemplateFile file);

    }
}
