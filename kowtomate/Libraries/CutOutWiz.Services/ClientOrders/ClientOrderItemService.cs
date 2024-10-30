using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using CutOutWiz.Core.OrderTeams;
using CutOutWiz.Services.BLL;
using CutOutWiz.Services.DbAccess;
using CutOutWiz.Services.Models.FileUpload;
using CutOutWiz.Services.StorageService;
using MailKit.Search;
using System.Security.Cryptography.X509Certificates;
using static CutOutWiz.Core.Utilities.Enums;
using static Google.Apis.Requests.BatchRequest;
using CutOutWiz.Data;
using CutOutWiz.Services.Managers.Common;

namespace CutOutWiz.Services.ClientOrders
{
    public class ClientOrderItemService : IClientOrderItemService
    {
        private readonly ISqlDataAccess _db;
        private readonly IFileServerManager _fileServerService;
        private readonly IClientOrderService _clientOrderService;
        private readonly IFluentFtpService _fluentFtpService;
        private readonly ICompanyManager _companyService;
        private readonly IFtpFilePathService _ftpFilePathService;
        private readonly IActivityAppLogService _activityAppLogService;
        public ClientOrderItemService(ISqlDataAccess db, IFileServerManager fileServerService,
            IClientOrderService clientOrderService, IFluentFtpService fluentFtpService, 
            ICompanyManager companyService, IFtpFilePathService ftpFilePathService
            ,IActivityAppLogService activityAppLogService)
        {
            _db = db;
            _fileServerService = fileServerService;
            _clientOrderService = clientOrderService;
            _fluentFtpService = fluentFtpService;
            _companyService = companyService;
            _ftpFilePathService = ftpFilePathService;
            _activityAppLogService = activityAppLogService;
        }

        public async Task<List<ClientOrderItemModel>> GetAllOrderItemByOrderId(long orderId)
        {
            return await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetAllByOrderId", new { OrderId = orderId });
        }
        public async Task<List<ClientOrderItemModel>> GetAllOrderItemByOrderIdForClient(long orderId)
        {
            return await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetAllByOrderIdForClient", new { OrderId = orderId });
        }
        public async Task<List<ClientOrderItemModel>> GetAllAssignOrderItemByContactIdAndTeamId(int contactId, int teamId)
        {
            return await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetAssignOrderItemByContactIdAndTeamId", new { teamId = teamId, contactId = contactId });
        }
        public async Task<List<ClientOrderItemModel>> GetAllByOrderId(long orderId)
        {
            return await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetOrderItemsByOrderId", new { OrderId = orderId });
        }
        public async Task<List<ClientOrderItemModel>> GetAllOrderItemByOrderIdForClientDownLoad(long orderId)
        {
            return await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetAllByOrderIdAndStatus", new { OrderId = orderId });
        }
        public async Task<List<ClientOrderItemModel>> GetAllOrderAssignedItemByOrderIdContactIdTeamId(long orderId, int contactId, int teamId)
        {
            return await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderAssignedItem_GetByOrderIdContactIdTeamId", new { OrderId = orderId, ContactId = contactId, TeamId = teamId });
        }
        public async Task<List<ClientOrderItemModel>> GetAllOrderAssignedItemByOrderId(long orderId, int contactId)
        {
            return await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderAssignedItem_GetByOrderId", new { OrderId = orderId, ContactId = contactId });
        }
        public async Task<List<ClientOrderItemModel>> GetOrderAllItemByOrderIdAndStatus(long orderId, byte status)
        {
            return await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderAllItem_GetByOrderIdAndStatus", new { OrderId = orderId, Status = status });
        }
        public async Task<List<ClientOrderItemModel>> GetEqualAndGreaterItemByStatus(long orderId, byte status)
        {
            return await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetEqualAndGreaterItemByStatus", new { OrderId = orderId, Status = status });
        }

        public async Task<Response<long>> Insert(ClientOrderItemModel clientOrderItem, long orderId)
        {

            var response = new Response<long>();

            try
            {
                clientOrderItem.FileNameWithoutExtension = await _ftpFilePathService.GetFileNameWithoutExtension(clientOrderItem.FileName);
                clientOrderItem.CreatedDate = DateTime.Now;
                clientOrderItem.UpdatedDate = DateTime.Now;
                clientOrderItem.ClientOrderId = orderId;

                if (clientOrderItem.ArrivalTime == null || clientOrderItem.ArrivalTime.Value.Year < 2000)
                {
                    clientOrderItem.ArrivalTime = DateTime.Now;
                }
                var newFileId = await _db.SaveDataUsingProcedureAndReturnId<long, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_insert", new
                {
                    clientOrderItem.CompanyId,
                    clientOrderItem.FileName,
                    clientOrderItem.ClientOrderId,
                    clientOrderItem.PartialPath,
                    clientOrderItem.Status,
                    clientOrderItem.IsDeleted,
                    clientOrderItem.CreatedDate,
                    clientOrderItem.UpdatedDate,
                    clientOrderItem.ObjectId,
                    clientOrderItem.FileSize,
                    clientOrderItem.TeamId,
                    clientOrderItem.FileByteString,
                    clientOrderItem.ExternalStatus,
                    clientOrderItem.InternalFileInputPath,
                    clientOrderItem.ExternalFileOutputPath,
                    clientOrderItem.InternalFileOutputPath,
                    clientOrderItem.FileNameWithoutExtension,
                    clientOrderItem.FileGroup,
                    clientOrderItem.IsExtraOutPutFile,
                    clientOrderItem.ArrivalTime,
                    clientOrderItem.CategoryId,
                    clientOrderItem.CategorySetByContactId,
                    clientOrderItem.CategorySetDate,
                    clientOrderItem.CategoryPrice,
                    clientOrderItem.CategorySetStatus,
                    clientOrderItem.CategoryApprovedByContactId
                });
                clientOrderItem.Id = newFileId;
                response.Result = newFileId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }


            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }
            return response;
        }
        public async Task<ClientOrderItemModel> GetById(long orderItemId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetById", new { Id = orderItemId });
            return result.FirstOrDefault();
        }
        public async Task<ClientOrderItemListModel> GetOrderItemById(long orderItemId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemListModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetById", new { Id = orderItemId });
            return result.FirstOrDefault();
        }
        public async Task<List<ClientOrderItemModel>> GetItemById(long orderItemId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetById", new { Id = orderItemId });
            return result.ToList();
        }
        public async Task<List<ClientOrderItemModel>> GetItemByOrderIdAndReadToCheckStatus(ClientOrderItemModel orderItem)
        {
            List<ClientOrderItemModel> resultList = new List<ClientOrderItemModel>();
            try
            {
                var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrder_GetByOrderIdAndReadyToCheckStatus", new
                {
                    OrderId = orderItem.ClientOrderId,
                    Status = orderItem.ExternalStatuss
                });
                resultList = result.ToList();

            }
            catch (Exception ex)
            {
                resultList = null;
            }
            return resultList;
        }
        public async Task<ClientOrderItemModel> GetByIdAndFileName(long OrderId, string FileName, string partialPath)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetByIdFileName", new { OrderId = OrderId, FileName = FileName, PartialPath = partialPath });
            return result.FirstOrDefault();
        }

        public async Task<ClientOrderItemModel> GetByIdOrderId(long orderId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetByOrderId", new { OrderId = orderId });
            return result.FirstOrDefault();
        }

        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrderItem_DeleteByObjectId", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }
        public async Task<Response<bool>> DeleteList(List<ClientOrderItemModel> files, FileServerViewModel model, ClientOrderModel order)
        {
            var response = new Response<bool>();
            foreach (var file in files)
            {
                try
                {
                    var clientOrderItemDelete = await Delete(file.ObjectId);
                    if (clientOrderItemDelete.IsSuccess)
                    {
                        FileUploadModel fileUploadVM = new FileUploadModel();
                        fileUploadVM.ReturnPath = file.InternalFileInputPath;
                        fileUploadVM.FtpUrl = model.Host;
                        fileUploadVM.userName = model.UserName;
                        fileUploadVM.password = model.Password;
                        fileUploadVM.SubFolder = model.SubFolder;
                        // fileUploadVM.ReturnPath=mod
                        await _fluentFtpService.DeleteFile(fileUploadVM);
                        order.orderItems.RemoveAll(oi => oi.ObjectId == file.ObjectId);
                        response.IsSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
                }
            }
            return response;
        }
        public async Task<Response<bool>> UpdateClientOrderItemTeamId(long orderId, int newTeamId, List<string> clientOrderItemIds)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "SP_Order_ClientOrderItem_UpdateTeamId", new
                {
                    OrderId = orderId,

                    ClientOrderItemIds = string.Join(",", clientOrderItemIds),
                    NewTeamId = newTeamId
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
        public bool CheckingFile(ClientOrderItemModel OrderFile)
        {
            bool result = false;
            try
            {
                _db.SaveDataUsingProcedure(storedProcedure: "SP_Order_ClientOrderItem_CheckingImageName", new
                {
                    OrderId = OrderFile.ClientOrderId,
                    FileName = OrderFile.FileName,
                });
                result = true;
                return result;

            }
            catch (Exception ex)
            {
                result = false;
                return result;
            }
        }

        public async Task<ClientOrderItemModel> GetByFileByOrderIdAndFileName(ClientOrderItemModel clientOrderItem)
        {
            var restult = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_CheckingImageName", new { OrderId = clientOrderItem.ClientOrderId, FileName = clientOrderItem.FileName });
            return restult.FirstOrDefault();
        }

        public async Task<List<ClientOrderItemModel>> GetByFileByOrderIdAndFileNameList(ClientOrderItemModel clientOrderItem)
        {
            return await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_CheckingImageName", new { OrderId = clientOrderItem.ClientOrderId, FileName = clientOrderItem.FileName });
        }

        public async Task<ClientOrderItemModel> GetByFileByOrderIdAndFileNameAndPath(ClientOrderItemModel clientOrderItem)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetItemByCompanyIdAndFileNameAndFilePath", new
            {
                ClientOrderId = clientOrderItem.ClientOrderId,
                FileNameWithoutExtension = clientOrderItem.FileNameWithoutExtension,
                PartialPath = clientOrderItem.PartialPath,
                CompanyId = clientOrderItem.CompanyId,
            });

            if (result == null || !result.Any() || result.FirstOrDefault().Id == 0)
            {
                return null;
            }

            return result.FirstOrDefault();
        }
        public async Task<ClientOrderItemModel> GetByFileByOrderIdAndFullFileNameAndPath(ClientOrderItemModel clientOrderItem)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetItemByCompanyIdAndFullFileNameAndFilePath", new
            {
                ClientOrderId = clientOrderItem.ClientOrderId,
                FileName = clientOrderItem.FileName,
                PartialPath = clientOrderItem.PartialPath,
                CompanyId = clientOrderItem.CompanyId,
            });

            if (result == null || !result.Any() || result.FirstOrDefault().Id == 0)
            {
                return null;
            }

            return result.FirstOrDefault();
        }
		public async Task<ClientOrderItemModel> GetByFileByOrderIdAndFileNameAndPathWithWorkFileGroup(ClientOrderItemModel clientOrderItem)
		{
			var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetItemByCompanyIdAndFileNameAndFilePathAndFileGroup", new
			{
				ClientOrderId = clientOrderItem.ClientOrderId,
				FileNameWithoutExtension = clientOrderItem.FileNameWithoutExtension,
				PartialPath = clientOrderItem.PartialPath,
				CompanyId = clientOrderItem.CompanyId,
			});

			if (result == null || !result.Any() || result.FirstOrDefault().Id == 0)
			{
				return null;
			}

			return result.FirstOrDefault();
		}

		public async Task<List<ClientOrderItemModel>> GetByFileByOrderIdAndFileNameWithoutExtension(ClientOrderItemModel clientOrderItem)
        {
            var restult = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetItemByCompanyIdAndFileName", new
            {
                ClientOrderId = clientOrderItem.ClientOrderId,
                clientOrderItem.FileNameWithoutExtension,
                CompanyId = clientOrderItem.CompanyId,
            });
            return restult;
        }

        public async Task<ClientOrderItemModel> GetByImageNameAndAllowClientProcessingEnableCompany(string fileNameWithoutExtension)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetByImageNameAndAllowClientProcessingEnableCompany", new
            {
                FileNameWithoutExtension = fileNameWithoutExtension,

            });
            return result.FirstOrDefault();
        }

        public async Task<ClientOrderItemModel> GetItemByImageNameAndCompanyId(string fileName, int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetByImageNameAndCompanyId", new
            {
                FileName = fileName,
                companyId = companyId

            });
            if (result.Count>0)
            {
				return result.FirstOrDefault();
			}
            else { return null; }
        }
        public async Task<List<ClientOrderItemModel>> GetFileListByOrderIdAndFileName(ClientOrderItemModel clientOrderItem)
        {
            var restult = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_CheckingImageName", new
            {
                OrderId = clientOrderItem.ClientOrderId,
                FileName = clientOrderItem.FileName
            });
            return restult;
        }
        public async Task<ClientOrderItemModel> GetByFileByOrderIdFileNamePathAndFileId(ClientOrderItemModel clientOrderItem)
        {
            var restult = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetItemByCompanyIdFileNamePartialPathAndFileId", new
            {
                ClientOrderId = clientOrderItem.ClientOrderId,
                FileName = clientOrderItem.FileName,
                PartialPath = clientOrderItem.PartialPath,
                CompanyId = clientOrderItem.CompanyId,
                Id = clientOrderItem.Id,
            });
            return restult.FirstOrDefault();
        }
        public async Task<Response<bool>> UpdateEitorItemInfo(ClientOrderItemModel orderItem)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "SP_Order_ClientOrderItem_UpdateForEditorDoneProduction", new
                {
                    orderItem.Id,
                    orderItem.ProductionDoneFilePath,
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


        public async Task<Response<bool>> UpdateOrderItemListCategory(List<ClientOrderItemModel> orderItems,OrderWiseCategoryModel orderWiseCategory)
        {
            var response = new Response<bool>();

            try
            {
                foreach (var orderItem in orderItems)
                {
                    orderItem.CategorySetStatus = orderWiseCategory.CategorySetStatus;
                    orderItem.CategoryApprovedByContactId = orderWiseCategory.CategorySetByContactId;
                    orderItem.CategoryId = orderWiseCategory.CategoryId;
                    orderItem.CategorySetByContactId = orderWiseCategory.CategorySetByContactId;
                    orderItem.CategorySetDate = orderWiseCategory.CategorySetDate;

                    await _db.SaveDataUsingProcedure(storedProcedure: "SP_Order_ClientOrderItem_UpdateCategory", new
                    {
                        orderItem.Id,
                        orderItem.CategoryId,
                        orderItem.CategorySetByContactId,
                        orderItem.CategorySetDate,
                        orderItem.CategoryPrice,
                        orderItem.TimeInMinute,
                        orderItem.CategorySetStatus,
                        orderItem.CategoryApprovedByContactId
                    });

                    ClientCategoryChangeLogModel clientCategoryChangeLog = new ClientCategoryChangeLogModel
                    {
                        ClientCategoryId = orderWiseCategory.CategoryId,
                        ClientOrderItemId = orderItem.Id,
                        CategorySetByContactId = orderWiseCategory.CategorySetByContactId,
                        CategorySetDate = orderWiseCategory.CategorySetDate,
                    };

                    await _activityAppLogService.ClientCategoryChangeLogInsert(clientCategoryChangeLog);
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

        public async Task<Response<bool>> ApprovedOrderItemListCategory(List<ClientOrderItemModel> orderItems, OrderWiseCategoryModel orderWiseCategory)
        {
            var response = new Response<bool>();

            try
            {
                foreach (var orderItem in orderItems)
                {
                    orderItem.CategorySetStatus = orderWiseCategory.CategorySetStatus;
                    orderItem.CategoryApprovedByContactId = orderWiseCategory.CategoryApprovedByContactId;

                    await _db.SaveDataUsingProcedure(storedProcedure: "SP_Order_ClientOrderItem_ApprovedCategorySetStatus", new
                    {
                        orderItem.Id,
                        orderItem.CategorySetStatus,
                        orderItem.CategoryApprovedByContactId
                    });

                    ClientCategoryChangeLogModel clientCategoryChangeLog = new ClientCategoryChangeLogModel
                    {
                        ClientCategoryId = orderWiseCategory.CategoryId,
                        ClientOrderItemId = orderItem.Id,
                        CategorySetByContactId = orderWiseCategory.CategorySetByContactId,
                        CategorySetDate = orderWiseCategory.CategorySetDate,
                    };

                    await _activityAppLogService.ClientCategoryChangeLogInsert(clientCategoryChangeLog);
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
        public async Task<Response<bool>> UpdateOrderItemCategory(ClientOrderItemModel orderItem, OrderItemWiseCategoryModel orderItemWiseCategory)
        {
            var response = new Response<bool>();

            try
            {

                
                await _db.SaveDataUsingProcedure(storedProcedure: "SP_Order_ClientOrderItem_UpdateCategory", new
                    {
                        orderItem.Id,
                        orderItemWiseCategory.CategoryId,
                        orderItemWiseCategory.CategorySetByContactId,
                        orderItemWiseCategory.CategorySetDate,
                        orderItemWiseCategory.CategoryPrice,
                        orderItem.TimeInMinute,
                        orderItem.CategorySetStatus,
                        orderItem.CategoryApprovedByContactId
                });

                    ClientCategoryChangeLogModel clientCategoryChangeLog = new ClientCategoryChangeLogModel
                    {
                        ClientCategoryId = orderItemWiseCategory.CategoryId,
                        ClientOrderItemId = orderItem.Id,
                        CategorySetByContactId = orderItemWiseCategory.CategorySetByContactId,
                        CategorySetDate = orderItemWiseCategory.CategorySetDate,
                    };

                    await _activityAppLogService.ClientCategoryChangeLogInsert(clientCategoryChangeLog);
               

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<bool>> UpdateItemByQC(ClientOrderItemModel orderItem)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "SP_Order_ClientOrderItem_UpdateForQCCompletedFile", new
                {
                    orderItem.Id,
                    orderItem.InternalFileOutputPath,
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
        public async Task<Response<bool>> UpdateItemQCByReplaceFile(ClientOrderItemModel orderItem)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "SP_Order_ClientOrderItem_UpdateQCByReplaceCompletedFile", new
                {
                    orderItem.Id,
                    orderItem.CompanyId,
                    orderItem.FileName,
                    orderItem.InternalFileOutputPath,
                    orderItem.PartialPath,
                    orderItem.ClientOrderId,
                    orderItem.ProductionDoneFilePath
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
        public async Task<Response<bool>> UpdateItemByQCForReject(ClientOrderItemModel orderItem)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "SP_Order_ClientOrderItem_UpdateForQCRejectedFile", new
                {
                    orderItem.CompanyId,
                    orderItem.FileName,
                    orderItem.PartialPath,
                    orderItem.ClientOrderId,
                    orderItem.ProductionDoneFilePath
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

        public async Task<ClientOrderItemStatus> GetOrderItemMinStatusByOrderId(long orderId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemStatus, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItemsMinStatusByOrderId", new { OrderId = orderId, FileGroup = OrderItemFileGroup.Work });
            return result.FirstOrDefault();
        }

        public async Task<ClientOrderCategorySetStatus> GetOrderItemMinCategorySetStatusByOrderId(long orderId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderCategorySetStatus, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItemsMinCategorySetStatusByOrderId", new { OrderId = orderId, FileGroup = OrderItemFileGroup.Work });
            return result.FirstOrDefault();
        }
        public async Task<ClientOrderItemArrivalTime> GetOrderItemMinArrivalTimeByOrderId(long orderId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemArrivalTime, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItemsMinArrivalTimeByOrderId", new { OrderId = orderId });
            return result.FirstOrDefault();
        }
        public async Task<ClientOrderItemCount> GetTotalPrductionOngoingItem(int contactId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemCount, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetAssignOrderItemByContactId", new { ContactId = contactId });
            return result.FirstOrDefault();
        }

        public async Task DeleteFileFromFtp(long FileId)
        {
            var result = await GetById(FileId);
            var order = await _clientOrderService.GetById((int)result.ClientOrderId);
            var fileServer = await _fileServerService.GetById((int)order.FileServerId);
            FileUploadModel fileUploadVM = new FileUploadModel();
            fileUploadVM.FtpUrl = fileServer.Host;
            fileUploadVM.userName = fileServer.UserName;
            fileUploadVM.password = fileServer.Password;
            fileUploadVM.SubFolder = fileServer.SubFolder;
            fileUploadVM.ReturnPath = result.InternalFileInputPath;

            var deleteResult = await _fluentFtpService.DeleteFile(fileUploadVM);
        }
        public async Task DeleteFileFromFtpByID(long FileId)
        {
            var result = await GetById(FileId);
            var order = await _clientOrderService.GetById((long)result.ClientOrderId);
            var fileServer = await _fileServerService.GetById((int)order.FileServerId);
            FileUploadModel fileUploadVM = new FileUploadModel();
            fileUploadVM.FtpUrl = fileServer.Host;
            fileUploadVM.userName = fileServer.UserName;
            fileUploadVM.password = fileServer.Password;
            var basePath = await GetOrderInfoPath(order);
            fileUploadVM.ReturnPath = $"{basePath}{result.PartialPath}\\{result.FileName}";

            var deleteResult = await _fluentFtpService.DeleteFile(fileUploadVM);
        }

        public async Task<string> GetOrderInfoPath(ClientOrderModel order)
        {
            var current = order.CreatedDate;
            var company = await _companyService.GetById(order.CompanyId);
            var year = current.ToString("yyyy");
            var monthName = current.ToString("MMMM");
            var formattedDate = current.ToString("dd.MM.yyyy");
            var path = $"{company.Code}/{year}/{monthName}/{formattedDate}/Completed";
            return path;
        }

        public async Task UpdateItemFile(ClientOrderItemModel orderItem)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "SP_Order_ClientOrderItem_UpdateByOrderIdCompanyIdObjectId", new
                {
                    orderItem.Id,
                    orderItem.CompanyId,
                    orderItem.FileName,
                    orderItem.FileSize,
                    orderItem.InternalFileInputPath,
                    orderItem.PartialPath,
                    orderItem.Status,
                    orderItem.IsDeleted,
                    orderItem.CreatedDate,
                    orderItem.UpdatedDate,
                    orderItem.UpdatedByContactId,
                    orderItem.ObjectId,
                    orderItem.ExternalStatus,
                    orderItem.ClientOrderId,
                });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

        }

        public async Task<Response<bool>> UpdateClientOrderItemStatus(ClientOrderItemModel OrderItem)
        {
            var response = new Response<bool>();

            try
            {
                Console.WriteLine(OrderItem.Id);
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrderItem_StatusUpdate", new
                {
                    OrderItem.Id,
                    OrderItem.Status,
                    OrderItem.ExternalStatus,
                    OrderItem.FileGroup
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

        public async Task<Response<bool>> UpdateClientOrderItemListModelStatus(ClientOrderItemListModel OrderItem)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrderItem_StatusUpdate", new
                {
                    OrderItem.Id,
                    OrderItem.Status,
                    OrderItem.ExternalStatus,
                    OrderItem.FileGroup
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

        public async Task<Response<bool>> UpdateClientOrderItemStatusByClientOrderId(long clientOrderId, byte status, byte externalStatus)
        {
            var response = new Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrderItem_StatusUpdateByClientOrderId", new
                {
                    ClientOrderId = clientOrderId,
                    Status = status,
                    ExternalStatus = externalStatus,
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

        public async Task<List<ClientOrderItemModel>> GetByClientOrderIdAndFileGroup(long orderId, int fileGroup)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetByClientOrderIdAndFileGroup", new { ClientOrderId = orderId, FileGroup = fileGroup });
            return result;
        }

        public async Task<List<ClientOrderItemModel>> GetClientOrderItemByEditorContactIdAndOrderId(int contactId, long orderId)
        {
            var restult = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetByAssignedContactIdAndOrderId", new
            {
                assignContactId = contactId,
                orderId = orderId

            });
            return restult;
        }

        public async Task<List<ClientOrderItemModel>> GetDistributedClientOrderItemByEditorContactIdAndOrderId(int contactId, long orderId)
        {
            var restult = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_GetDistributedAssignOrderItemByContactIdAndTeamId", new
            {
                assignContactId = contactId,
                orderId = orderId

            });
            return restult;
        }

        public async Task<List<ClientOrderItemModel>> GetOrderItemsForSendingToEditorPc(int companyId)
        {
            string sql = @"
                            SELECT *
                            FROM (
                                SELECT
                                    ci.*,
                                    oa.AssignContactId AS EditorContactId,
                                    con.FirstName AS EditorFirstName,
                                    con.DownloadFolderPath AS EditorDownloadFolderPath,
                                    ROW_NUMBER() OVER (PARTITION BY oa.AssignContactId ORDER BY ci.ArrivalTime) AS RowNumber,
                                    MIN(ci.ArrivalTime) OVER (PARTITION BY oa.AssignContactId) AS MinArrivalTime
                                FROM Order_ClientOrderItem AS ci with(nolock)
                                INNER JOIN Order_AssignedImageEditor AS oa with(nolock) ON oa.Order_ImageId = ci.Id AND oa.IsActive = 1
                                INNER JOIN [dbo].[Security_Contact] AS con WITH (NOLOCK) ON oa.AssignContactId = con.Id AND con.IsUserActive = 1 AND con.isSharedFolderEnable = 1
                                WHERE
                                    ci.CompanyId = @CompanyId
                                    AND ci.Status IN (7, 11)
                            ) AS oc
                            WHERE
                                oc.RowNumber <= 7;";

            var restult = await _db.LoadDataUsingQuery<ClientOrderItemModel, dynamic>(sql, new
            {
                CompanyId = companyId
            });
            return restult;
        }

        /// <summary>
        /// Get files for Retouched.ai processing
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns>ClientOrderItem</returns>
        public async Task<List<ClientOrderItemModel>> GetOrderItemForRetouchedAiProcessing(int companyId)
        {
            var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItems_for_retouchedai", new 
            {
                CompanyId = companyId 
            });
            return result;
        }
        /// <summary>
        /// Check retouched.ai files processed or not
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>True or False</returns>
        public async Task<bool> IsRetouchedProcessed(long Id)
        {
            var outputResult = false;
            try
            {
                var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItems_CheckRetouched_Processed", new
                {
                    Id = Id
                });

                if (result.FirstOrDefault().IbrStatus == 1 && !string.IsNullOrWhiteSpace(result.FirstOrDefault().IbrProcessedImageUrl))
                {
                    outputResult = true;
                }

            }
            catch (Exception ex)
            {
                outputResult = false;
            }
            return outputResult;
        }


        public async Task<List<ClientOrderItemModel>> GetOrderItemByStatus(string query)
        {
            var queryString = String.Format("dbo.SP_Order_ClientOrder_GetOrdersItemStatus '{0}'", query);
            var filteredList = await _db.LoadDataUsingQuery<ClientOrderItemModel, dynamic>(queryString,
                    new
                    {
                    });
            return filteredList;
        }

        public async Task<Response<bool>> UpdateClientOrderItemCompletedPathById(ClientOrderItemModel OrderItem)
        {
            var response = new Response<bool>();

            try
            {
                int rows = await _db.SaveDataUsingProcedureAndReturnNumberOfEffectedRow(storedProcedure: "dbo.SP_Order_ClientOrderItem_UpdateInternalOutputPathById", new { id = OrderItem.Id, internalFileOutputPath = OrderItem.InternalFileOutputPath });
                if (rows > 0)
                {
                    response.IsSuccess = true;
                    response.Message = StandardDataAccessMessages.SuccessMessaage;
                }

            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;

        }

        public async Task<List<ClientOrderItemListModel>> GetOrderItemsByFilterWithPaging(ClientOrderFilter filter)
        {
            List<ClientOrderItemListModel> clientOrderItems = new List<ClientOrderItemListModel>();

            if (string.IsNullOrWhiteSpace(filter.SortColumn) || filter.SortColumn == "o.[OrderPlaceDateOnly]")
                filter.SortColumn = "oi.[Id]";

            if (string.IsNullOrWhiteSpace(filter.SortDirection))
                filter.SortDirection = "DESC";

            try
            {
                var queryString = String.Format("dbo.SP_OrderItem_GetListByFilter_Dev '{0}','{1}',{2},{3},'{4}','{5}','{6}'",
                filter.Where,
                filter.IsCalculateTotal.ToString().ToLower(),
                filter.Skip,
                filter.Top,
                filter.SortColumn,
                filter.SortDirection,
                filter.JoinClouse
                );

                var filteredList = await _db.LoadDataUsingQuery<ClientOrderItemListModel, dynamic>(queryString,
                    new
                    {
                    });

                if (filteredList.Any() && filter.IsCalculateTotal)
                {

                    filter.TotalImageCount = filteredList[0].TotalImageCount;
                }
                if (!filteredList.Any() && filter.IsCalculateTotal)
                {
                    filter.TotalCount = 0;
                    filter.TotalImageCount = 0;
                }

                return filteredList;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return null;

        }

        public async Task<bool> UpdateOrderItemStatus(ClientOrderItemModel orderItem, InternalOrderItemStatus status)
        {
            var statusHasChanged = false;
            if (orderItem != null)
            {
                if (orderItem.Status == (byte)InternalOrderItemStatus.ReworkDistributed)
                {
                    orderItem.Status = (byte)InternalOrderItemStatus.ReworkInProduction;
                    orderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkInProduction));
                    await UpdateClientOrderItemStatus(orderItem);
                    statusHasChanged = true;
                }
                else if (orderItem.Status == (byte)InternalOrderItemStatus.ReworkDone)
                {
                    orderItem.Status = (byte)InternalOrderItemStatus.ReworkQc;
                    orderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkQc));
                    await UpdateClientOrderItemStatus(orderItem);
                    statusHasChanged = true;

                }
                else if (orderItem.Status == (byte)InternalOrderItemStatus.ReworkQc && status == InternalOrderItemStatus.ReworkDistributed)
                {
                    orderItem.Status = (byte)InternalOrderItemStatus.ReworkDistributed;
                    orderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(InternalOrderItemStatus.ReworkDistributed));
                    await UpdateClientOrderItemStatus(orderItem);
                    statusHasChanged = true;

                }
                else
                {
                    if (orderItem.Status < (byte)status)
                    {
                        orderItem.Status = (byte)status;
                        orderItem.ExternalStatus = (byte)(EnumHelper.ExternalOrderItemStatusChange(status));
                        await UpdateClientOrderItemStatus(orderItem);
                        statusHasChanged = true;

                    }

                }

            }
            else
            {
                statusHasChanged = false;
            }
            return statusHasChanged;
        }
        /// <summary>
        /// Update Client Order Item After Retouched.ai processed completed.
        /// </summary>
        /// <param name="clientOrderItem">clientOrderItem</param>
        /// <returns>Client Order Item</returns>
        public async Task<Response<ClientOrderItemModel>> UpdateAfterRetouchedAiProcessed(ClientOrderItemModel clientOrderItem)
        {
            var response = new Response<ClientOrderItemModel>();

            try
            {

                var result = await _db.SaveDataUsingProcedureAndReturnId<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItem_Update_after_IbrProcessed", new
                {
                    clientOrderItem.Id,
                    clientOrderItem.IbrProcessedImageUrl,
                    clientOrderItem.IbrStatus,
                });

                response.Result = clientOrderItem;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }


            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
                response.Result = null;
            }
            return response;
        }
        /// <summary>
        /// Here check client order item. by search Client Order Item InternalInputPath and Created Date and Company Id.
        /// </summary>
        /// <param name="clientOrderItem"></param>
        /// <returns></returns>
        public async Task<bool> CheckClientOrderItemFile(int CompanyId, string InternalFileInputPath,string FileName, string CreatedDate=null)
        {
            var outputResult = false;
            try
            {
                var result = await _db.LoadDataUsingProcedure<ClientOrderItemModel, dynamic>(storedProcedure: "dbo.SP_Order_ClientOrderItems_CheckWithCompanyIdAndFilePath", new
                {
                    CompanyId=CompanyId,
                    InternalFileInputPath= InternalFileInputPath,
                    CreatedDate = CreatedDate,
                });

                if (result.Count>0 && result.Any())
                {
                    outputResult = true;
                }

            }
            catch (Exception ex)
            {
                outputResult = false;
            }
            return outputResult;
        }


        public async Task<Response<bool>> UpdateOrderItemExpectedDeliveryDate(long ClientOrderItemId, DateTime? expectedDeliveryDate)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrderItem_UpdateOrderItemDeadLineDate", new
                {
                    ClientOrderItemId,
                    expectedDeliveryDate
                });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public async Task<Response<bool>> UpdateOrderItemExpectedDeliveryDateByClientOrderId(long ClientOrderId, DateTime? expectedDeliveryDate)
        {
            var response = new Response<bool>();
            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Order_ClientOrderItem_UpdateOrderItemDeadLineDateByOrderId", new
                {
                    ClientOrderId,
                    expectedDeliveryDate
                });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {

            }
            return response;
        }

    }
}
