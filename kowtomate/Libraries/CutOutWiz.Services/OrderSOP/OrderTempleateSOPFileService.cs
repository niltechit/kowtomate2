using CutOutWiz.Services.Models.SOP;
using CutOutWiz.Core;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutOutWiz.Services.Models.OrderSOP;
using CutOutWiz.Data;

namespace CutOutWiz.Services.OrderSOP
{
	public class OrderTempleateSOPFileService : IOrderTempleateSOPFileService
    {
        private readonly ISqlDataAccess _db;
        public OrderTempleateSOPFileService(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<Response<int>> Insert(OrderSOPTemplateFile file)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_OrderSOP_TemplateFile_Insert", new
                {
                    file.OrderSOPTemplateId,
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
                    file.BaseSOPTemplateFileId,
                    file.BaseTemplateId
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
        public async Task<List<OrderSOPTemplateFile>> GetOrderSopTemplateFilesByOrderSopTemplateId(int SOPTemplateId)
        {
            return await _db.LoadDataUsingProcedure<OrderSOPTemplateFile, dynamic>(storedProcedure: "dbo.SP_OrderSOPTemplateFile_GetBySOPTemplateId", new { SOPTemplateId = SOPTemplateId });
        }
        public async Task<OrderSOPTemplateFile> GetById(int fileId)
        {
            var result = await _db.LoadDataUsingProcedure<OrderSOPTemplateFile, dynamic>(storedProcedure: "dbo.SP_OrderSOPTemplateFile_GetById", new { Id = fileId });
            return result.FirstOrDefault();
        }
        public async Task<OrderSOPTemplateFile> GetByOrderSOPTemplateIdAndFileName(OrderSOPTemplateFile model)
        {
            var result = await _db.LoadDataUsingProcedure<OrderSOPTemplateFile, dynamic>(storedProcedure: "dbo.SP_OrderSOPTemplateFile_GetByOrderSOPTemplateIdAndFileName", new 
            {
				OrderSOPTemplateId = model.OrderSOPTemplateId,
                FileName= model.FileName,
			});
            return result.FirstOrDefault();
        }
        
    }
}
