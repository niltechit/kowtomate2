using CutOutWiz.Core;
using CutOutWiz.Services.Models.Email;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Data;

namespace CutOutWiz.Services.Email
{
    public class ArchiveQueueEmailService : IArchiveQueueEmailService
    {
        private readonly ISqlDataAccess _db;
        public ArchiveQueueEmailService(ISqlDataAccess db)
        {
            _db = db;
        }

        public async Task<Response<int>> AddAchiveEmail(ArchiveQueueEmailModel archiveEmail)
        {
            var response = new Response<int>();
            try
            {
               
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Email_Archive_Insert", new
                {
                    FromEmail = archiveEmail.FromEmail,
                    FromEmailName = archiveEmail.FromEmailName,
                    ToEmail = archiveEmail.ToEmail,
                    ToEmailName = archiveEmail.ToEmailName,
                    Subject = archiveEmail.Subject,
                    Body = archiveEmail.Body,
                    CcEmail = archiveEmail.CcEmail,
                    BccEmail = archiveEmail.BccEmail,
                    AttachmentFilePath = archiveEmail.AttachmentFilePath,
                    AttachmentFileName = archiveEmail.AttachedFileName,
                    Status = archiveEmail.Status,
                    CreatedByContactId = archiveEmail.CreatedByContactId,
                    ObjectId = archiveEmail.ObjectId
                });

                if (newId == 0)
                {
                    response.Message = StandardDataAccessMessages.ErrorOnAddMessaage;
                    return response;
                }
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

        public async Task<Response<int>> AddQueueEmail(ArchiveQueueEmailModel queueEmail)
        {
            var response = new Response<int>();
            try
            {

                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Email_Queue_Insert", new
                {
                    FromEmail = queueEmail.FromEmail,
                    FromEmailName = queueEmail.FromEmailName,
                    ToEmail = queueEmail.ToEmail,
                    ToEmailName = queueEmail.ToEmailName,
                    Subject = queueEmail.Subject,
                    Body = queueEmail.Body,
                    CcEmail = queueEmail.CcEmail,
                    BccEmail = queueEmail.BccEmail,
                    AttachmentFilePath = queueEmail.AttachmentFilePath,
                    AttachmentFileName = queueEmail.AttachedFileName,
                    Status = queueEmail.Status,
                    CreatedByContactId = queueEmail.CreatedByContactId,
                    ObjectId = queueEmail.ObjectId
                });

                if (newId == 0)
                {
                    response.Message = StandardDataAccessMessages.ErrorOnAddMessaage;
                    return response;
                }
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
