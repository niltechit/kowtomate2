using CutOutWiz.Core;
using CutOutWiz.Core.Message;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services.Models.Message;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Data;

namespace CutOutWiz.Services.InternalMessage
{
    public class InternalMessageService : IInternalMessageService
    {
        private readonly ISqlDataAccess _db;
        private readonly IInternalMessageTemplateService _internalMessageService;
        private readonly IInternalMessageTokenProvider _internalMessageTokenProvider;
        private readonly IInernalMessageTokenizer _inernalMessageTokenizer;
        public InternalMessageService(ISqlDataAccess db,
            IInternalMessageTemplateService internalMessageTemplateService,
            IInternalMessageTokenProvider internalMessageTokenProvider,
            IInernalMessageTokenizer inernalMessageTokenizer

            )
        {
            _db = db;
            _internalMessageService = internalMessageTemplateService;
            _internalMessageTokenProvider = internalMessageTokenProvider;
            _inernalMessageTokenizer = inernalMessageTokenizer;
        }

        /// <summary>
        /// Get All Templates
        /// </summary>
        /// <returns></returns>
        public async Task<List<InternalMessageModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<InternalMessageModel, dynamic>(storedProcedure: "dbo.SP_Internal_Message_GetAll", new { });
        }

        /// <summary>
        /// Get internalMessage by internalMessage Id
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<List<InternalMessageModel>> GetByContactId(int contactId)
        {
            return await _db.LoadDataUsingProcedure<InternalMessageModel, dynamic>(storedProcedure: "dbo.SP_Internal_Message_GetAllByContactId", new { ContactId = contactId });
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<InternalMessageModel> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<InternalMessageModel, dynamic>(storedProcedure: "dbo.SP_Message_Template_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get internalMessage by access code
        /// </summary>
        /// <param name="TemplateId"></param>
        /// <returns></returns>
        public async Task<InternalMessageModel> GetByAccessCode(string accessCode)
        {
            var result = await _db.LoadDataUsingProcedure<InternalMessageModel, dynamic>(storedProcedure: "dbo.SP_Message_Template_GetAccessCode", new { AccessCode = accessCode });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert internalMessage
        /// </summary>
        /// <param name="internalMessage"></param>
        /// <returns></returns>
        /// 
       
        public async Task<Response<int>> Insert(InternalMessageNotification internalMessageNotification)
        {
            var response = new Response<int>();
            try
            {
                var messageTemplate  = await GetActiveInternalMessageTemplate("SOP.InsertSOPInternalMessageForRecipient");
                
                if (internalMessageNotification.MessageType == "Update")
                {
                    messageTemplate = await GetActiveInternalMessageTemplate("SOP.UpdateSOPInternalMessageForRecipient");
                }
                else if (internalMessageNotification.MessageType == "Delete")
                {
                    messageTemplate = await GetActiveInternalMessageTemplate("SOP.DeleteSOPInternalMessageForRecipient");
                }
                else if(internalMessageNotification.MessageType == "PriceUpdateByClient")
                {
                    messageTemplate = await GetActiveInternalMessageTemplate("Sop.PriceConfirmationNotificationForOperation");
                }
                else if (internalMessageNotification.MessageType == "PriceUpdateByOperation")
                {
                    messageTemplate = await GetActiveInternalMessageTemplate("Sop.PriceConfirmationNotificationForClient");
                }
                else if(internalMessageNotification.MessageType == "OrderAdd")
                {
                    messageTemplate = await GetActiveInternalMessageTemplate("Order.OrderAddNotificationForClient");
                }
                else if (internalMessageNotification.MessageType == "OrderUpdate")
                {
                    messageTemplate = await GetActiveInternalMessageTemplate("Order.OrderUpdateNotificationForClient");
                }
                else if (internalMessageNotification.MessageType == "OrderAssignByOps" )
                {
                    messageTemplate = await GetActiveInternalMessageTemplate("Order.OrderAssignToTeamNotificationForOperation");
                }
                else if (internalMessageNotification.MessageType == "OrderAssignByOpsToTeam")
                {
                    messageTemplate = await GetActiveInternalMessageTemplate("Order.OrderAssignToTeamNotificationForTeam");
                }

                if (messageTemplate == null)
                {
                    response.IsSuccess = false;
                    return response; 
                }
                if (internalMessageNotification.Contacts != null)
                {
                    foreach (var contact in internalMessageNotification.Contacts)
                    {
                        InternalMessageModel internalMessage = new InternalMessageModel
                        {
                            SenderContactId = internalMessageNotification.SenderContactId,
                            RecipientContactId = contact.Id,
                            Subject = messageTemplate.Subject,
                            Body = messageTemplate.Body,
                            ObjectId = Guid.NewGuid().ToString()
                        };

                        await SaveInternalNotificationToDb(internalMessageNotification, contact, internalMessage, messageTemplate);
                    }
                }

                else if (internalMessageNotification.Contact!= null)
                {
                    ContactModel contact = internalMessageNotification.Contact;
                    InternalMessageModel internalMessage = new InternalMessageModel
                    {
                        SenderContactId = internalMessageNotification.SenderContactId,
                        RecipientContactId = contact.Id,
                        Subject = messageTemplate.Subject,
                        Body = messageTemplate.Body,
                        ObjectId = Guid.NewGuid().ToString()
                    };
                    await SaveInternalNotificationToDb(internalMessageNotification, contact, internalMessage, messageTemplate);
                }
                
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
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Internal_Message_Update", new { IsDeleted=true,ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }
        protected async Task<InternalMessageTemplate> GetActiveInternalMessageTemplate(string templateName)
        {
            var messageTemplate = await _internalMessageService.GetByAccessCode(templateName);
            return messageTemplate;
        }

        private async Task SaveInternalNotificationToDb(InternalMessageNotification internalMessageNotification,ContactModel contact,InternalMessageModel internalMessage,InternalMessageTemplate messageTemplate)
        {
            internalMessageNotification.Contact = contact;
            var tokens = new List<InternalMessageToken>();

            if (internalMessageNotification.MessageType == "OrderAdd" || internalMessageNotification.MessageType == "OrderUpdate" || internalMessageNotification.MessageType == "OrderAssignByOpsToTeam")
            {
                _internalMessageTokenProvider.AddOrderAddUpdateInternalMessageForClients(tokens, internalMessageNotification);
            }
            else if (internalMessageNotification.MessageType == "OrderAssignByOps")
            {
                _internalMessageTokenProvider.AssignOrderToTeamInternalMessageForOpeartion(tokens, internalMessageNotification);
            }
            else
            {
                _internalMessageTokenProvider.AddSopAddUpdateInternalMessageForClients(tokens, internalMessageNotification);
            }

            internalMessage.Body = _inernalMessageTokenizer.Replace(messageTemplate.Body, tokens, true);

            await _db.SaveDataUsingProcedure(storedProcedure: "SP_Internal_Message_Insert", new
            {
                internalMessage.SenderContactId,
                internalMessage.RecipientContactId,
                internalMessage.Subject,
                internalMessage.Body,
                internalMessage.RootMessageId,
                internalMessage.IsRead,
                internalMessage.IsDeleted,
                internalMessage.Type,
                internalMessage.ObjectId
            });
        }
    }
}
