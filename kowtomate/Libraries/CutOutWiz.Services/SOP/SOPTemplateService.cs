using CutOutWiz.Core;
using CutOutWiz.Services.Models.SOP;
using CutOutWiz.Services.DbAccess;
using Mailjet.Client.Resources;
using CutOutWiz.Data;

namespace CutOutWiz.Services.SOP
{
    public class OrderSOPTemplateService:IOrderSOPTemplateService
    {
        private readonly ISqlDataAccess _db;
        public OrderSOPTemplateService(ISqlDataAccess db)
        {
            _db = db;
        }

        #region SOP Template
        /// <summary>
        /// Get All Templates
        /// </summary>
        /// <returns></returns>
        public async Task<List<SOPTemplateModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<SOPTemplateModel, dynamic>(storedProcedure: "dbo.SP_SOP_Template_GetAll", new { });
        }
        public async Task<List<SOPTemplateModel>> GetAllById(int templateId)
        {
            var result = await _db.LoadDataUsingProcedure<SOPTemplateModel, dynamic>(storedProcedure: "dbo.SP_SOP_Template_GetById", new { TemplateId = templateId });
            return result;
        }
        public async Task<List<SOPTemplateModel>> GetAllByCompany(int companyId)
        {
            return await _db.LoadDataUsingProcedure<SOPTemplateModel, dynamic>(storedProcedure: "dbo.[SP_SOP_Template_GetAllByCompanyId]", new { companyId = companyId });
        }
        public async Task<List<SOPTemplateModel>> GetAllPendingSopByCompany(int companyId)
        {
            return await _db.LoadDataUsingProcedure<SOPTemplateModel, dynamic>(storedProcedure: "dbo.SP_SOP_Template_GetAllPendingSop", new { companyId = companyId });
        }
        /// <summary>
        /// Get template by template Id
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<SOPTemplateModel> GetById(int templateId)
        {
            var result = await _db.LoadDataUsingProcedure<SOPTemplateModel, dynamic>(storedProcedure: "dbo.SP_SOP_Template_GetById", new { TemplateId = templateId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<SOPTemplateModel> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<SOPTemplateModel, dynamic>(storedProcedure: "dbo.SP_SOP_Template_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }
        public async Task<SOPTemplateViewModel> GetByObjectID(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<SOPTemplateViewModel, dynamic>(storedProcedure: "dbo.SP_SOP_Template_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }
        /// <summary>
        /// Insert template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(SOPTemplateModel template)
        {
            var response = new Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_SOP_Template_Insert", new
                {
                    template.Name,
                    template.CompanyId,
                    template.FileServerId,
                    template.Version,
                    template.ParentTemplateId,
                    template.Instruction,
                    template.UnitPrice,
                    template.InstructionModifiedByContactId,
                    template.Status,
                    template.IsDeleted,
                    template.CreatedByContactId,
                    template.ObjectId,
                });

                if (newId == 0)
                {
                    response.Message = StandardDataAccessMessages.ErrorOnAddMessaage;
                    return response;
                }

                //Add services
                if (template.SopTemplateServiceList != null && template.SopTemplateServiceList.Any())
                {
                    foreach (var service in template.SopTemplateServiceList)
                    {
                        service.SOPTemplateId = newId;
                        await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_SOP_TemplateService_Insert", new
                        {
                            service.SOPTemplateId,
                            service.SOPStandardServiceId,
                            service.Status,
                            service.CreatedByContactId,
                            service.IsDeleted,
                            service.ObjectId
                        });
                    }
                }

                //Add File
                if (template.SopTemplateFileList != null && template.SopTemplateFileList.Any())
                {
                    await SopFileInsert(template.SopTemplateFileList, newId);
                }


                //template.Id = newId;
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
        /// Update Template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(SOPTemplateModel template)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_SOP_Template_Update", new
                {
                    template.Id,
                    template.Name,
                    template.CompanyId,
                    template.FileServerId,
                    template.Version,
                    template.ParentTemplateId,
                    template.Instruction,
                    template.UnitPrice,
                    template.InstructionModifiedByContactId,
                    template.Status,
                    template.IsDeleted,
                    template.UpdatedByContactId
                });

                //Add services
                if (template.SopTemplateServiceList != null && template.SopTemplateServiceList.Any())
                {
                    await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_SOP_TemplateService_Delete", new { SopTemplateId = template.Id });
                    foreach (var service in template.SopTemplateServiceList)
                    {
                        service.SOPTemplateId = template.Id;
                        await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_SOP_TemplateService_Insert", new
                        {
                            service.SOPTemplateId,
                            service.SOPStandardServiceId,
                            service.Status,
                            service.CreatedByContactId,
                            service.IsDeleted,
                            service.ObjectId
                        });
                    }
                }

                //Add File
                if (template.SopTemplateFileList != null && template.SopTemplateFileList.Any())
                {
                    await SopFileInsert(template.SopTemplateFileList, template.Id);
                }

                template.Id = template.Id;
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
        /// Delete Template by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_SOP_Template_DeleteByObjectId", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        #endregion Sop Template

        #region Sop Template File
        private async Task<bool> SopFileInsert(List<SOPTemplateFile> SopTemplateFileList, int sopTemplateId)
        {
            var newFileId = 0;
            foreach (var file in SopTemplateFileList)
            {
                try
                {


                    file.SOPTemplateId = sopTemplateId;
                    newFileId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_SOP_TemplateFile_Insert", new
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
                }
                catch
                {

                }
            }

            return newFileId > 0;

        }

        public async Task<List<SOPTemplateFile>> GetSopTemplateFilesBySopTemplateId(int SOPTemplateId)
        {
            return await _db.LoadDataUsingProcedure<SOPTemplateFile, dynamic>(storedProcedure: "dbo.[SP_TemplateFile_GetBySOPTemplateId]", new { SOPTemplateId = SOPTemplateId });
        }
        public async Task<List<SOPTemplateFile>> GetSopTemplateFilesByTemplateId(int SOPTemplateId)
        {
            return await _db.LoadDataUsingProcedure<SOPTemplateFile, dynamic>(storedProcedure: "dbo.[SP_TemplateFile_GetSOPTemplateByTemplateId]", new { SOPTemplateId = SOPTemplateId });
        }
        public async Task<Response<bool>> UpdateTemplateFile(string objectId)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.[SP_SOP_Template_UpdateIsDeleteByObjectId]", new { ObjectId = objectId });

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }
            return response;
        }

        public async Task<SOPTemplateFile> GetSopTemplateFilesById(int fileId)
        {
            var result = await _db.LoadDataUsingProcedure<SOPTemplateFile, dynamic>(storedProcedure: "dbo.SP_SOPTemplateFile_GetById", new { FileId = fileId });
            return result.FirstOrDefault();
        }

        #endregion
    }
}
