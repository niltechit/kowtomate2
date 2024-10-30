using CutOutWiz.Services.Models.SOP;
using CutOutWiz.Core;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Data;

namespace CutOutWiz.Services.SOP
{
	public class OrderSOPTempleateFileService : IOrderSOPTempleateFileService
    {
        private readonly ISqlDataAccess _db;
        public OrderSOPTempleateFileService(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<Response<int>> Insert(SOPTemplateFile file)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_SOP_TemplateFile_Insert", new
                {
                    file.SOPTemplateId,
                    file.FileName,
                    file.FileType,
                    file.ActualPath,
                    file.ModifiedPath,
                    file.IsDeleted,
                    file.CreatedByContactId,
                    file.ObjectId,
                    file.RootFolderPath,
                    file.ViewPath,
                    file.FileByteString,
                });
                file.Id = newId;
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
    }
}
