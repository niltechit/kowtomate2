using CutOutWiz.Core;
using CutOutWiz.Services.Models.ClientCategoryServices;
using CutOutWiz.Services.Models.ClientOrders;
using CutOutWiz.Services.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.ClientOrders
{
	public interface IClientOrderItemService
	{
        Task<Response<long>> Insert(ClientOrderItemModel clientOrderItem, long orderId);
        /// <summary>
        /// Update Client Order Item After Retouched.ai processed completed.
        /// </summary>
        /// <param name="clientOrderItem">clientOrderItem</param>
        /// <returns>Client Order Item</returns>
        Task<Response<ClientOrderItemModel>> UpdateAfterRetouchedAiProcessed(ClientOrderItemModel clientOrderItem);
        Task<List<ClientOrderItemModel>> GetAllOrderItemByOrderId(long orderId);
        Task<List<ClientOrderItemModel>> GetAllOrderAssignedItemByOrderId(long orderId,int contactId);
        Task<List<ClientOrderItemModel>> GetAllOrderAssignedItemByOrderIdContactIdTeamId(long orderId, int contactId, int teamId);
        Task<ClientOrderItemModel> GetById(long orderItemId);
        Task<ClientOrderItemListModel> GetOrderItemById(long orderItemId);

        Task<List<ClientOrderItemModel>> GetItemById(long orderItemId);
        Task<ClientOrderItemModel> GetByIdOrderId(long orderId);
        Task<List<ClientOrderItemModel>> GetItemByOrderIdAndReadToCheckStatus(ClientOrderItemModel orderItem);
        Task<Response<bool>> Delete(string Id);
        Task<Response<bool>> DeleteList(List<ClientOrderItemModel> files, FileServerViewModel model, ClientOrderModel order);
        Task<Response<bool>> UpdateClientOrderItemTeamId(long orderId, int newTeamId, List<string> clientOrderItemIds);
        Task<Response<bool>> UpdateEitorItemInfo(ClientOrderItemModel OrderFile);
        Task<Response<bool>> UpdateItemByQC(ClientOrderItemModel OrderFile);
        Task<Response<bool>> UpdateItemQCByReplaceFile(ClientOrderItemModel OrderFile);
        Task<Response<bool>> UpdateItemByQCForReject(ClientOrderItemModel OrderFile);
        Task<Response<bool>> UpdateOrderItemListCategory(List<ClientOrderItemModel> orderItems, OrderWiseCategoryModel orderWiseCategory);
        Task<Response<bool>> UpdateOrderItemCategory(ClientOrderItemModel orderItem, OrderItemWiseCategoryModel orderItemWiseCategory);
        bool CheckingFile(ClientOrderItemModel OrderFile);
        Task<ClientOrderItemModel> GetByFileByOrderIdAndFileName(ClientOrderItemModel clientOrderItem);
        Task<ClientOrderItemModel> GetByFileByOrderIdAndFileNameAndPath(ClientOrderItemModel clientOrderItem);
        Task<ClientOrderItemModel> GetByFileByOrderIdAndFullFileNameAndPath(ClientOrderItemModel clientOrderItem);
        Task<ClientOrderItemModel> GetByFileByOrderIdAndFileNameAndPathWithWorkFileGroup(ClientOrderItemModel clientOrderItem);
        Task<ClientOrderItemModel> GetByIdAndFileName(long OrderId, string FileName, string partialPath);
        Task<List<ClientOrderItemModel>> GetOrderAllItemByOrderIdAndStatus(long orderId, byte status);
        Task<List<ClientOrderItemModel>> GetEqualAndGreaterItemByStatus(long orderId, byte status);
        Task<ClientOrderItemStatus> GetOrderItemMinStatusByOrderId(long orderId);
        Task DeleteFileFromFtp(long FileId);
        Task DeleteFileFromFtpByID(long FileId);
        Task UpdateItemFile(ClientOrderItemModel ItemFile);
        Task<ClientOrderItemModel> GetByFileByOrderIdFileNamePathAndFileId(ClientOrderItemModel clientOrderItem);
        Task<List<ClientOrderItemModel>> GetByFileByOrderIdAndFileNameList(ClientOrderItemModel clientOrderItem);
		Task<Response<bool>> UpdateClientOrderItemStatus(ClientOrderItemModel OrderItem);
		Task<Response<bool>> UpdateClientOrderItemStatusByClientOrderId(long clientOrderId, byte status, byte externalStatus);
        Task<List<ClientOrderItemModel>> GetByFileByOrderIdAndFileNameWithoutExtension(ClientOrderItemModel clientOrderItem);

		Task<List<ClientOrderItemModel>> GetByClientOrderIdAndFileGroup(long orderId, int fileGroup);
        Task<List<ClientOrderItemModel>> GetAllOrderItemByOrderIdForClientDownLoad(long orderId);
        Task<List<ClientOrderItemModel>> GetFileListByOrderIdAndFileName(ClientOrderItemModel clientOrderItem);
        Task<List<ClientOrderItemModel>> GetClientOrderItemByEditorContactIdAndOrderId(int contactId, long orderId);
        Task<List<ClientOrderItemModel>> GetOrderItemByStatus(string query);
		Task<Response<bool>> UpdateClientOrderItemCompletedPathById(ClientOrderItemModel OrderItem);

        Task<List<ClientOrderItemModel>> GetAllByOrderId(long orderId);
        Task<ClientOrderItemCount> GetTotalPrductionOngoingItem(int contactId);
        Task<List<ClientOrderItemModel>> GetAllAssignOrderItemByContactIdAndTeamId(int contactId, int teamId);

        Task<List<ClientOrderItemModel>> GetDistributedClientOrderItemByEditorContactIdAndOrderId(int contactId, long orderId);
        Task<ClientOrderItemArrivalTime> GetOrderItemMinArrivalTimeByOrderId(long orderId);
        Task<ClientOrderItemModel> GetByImageNameAndAllowClientProcessingEnableCompany(string fileNameWithoutExtension);
        Task<List<ClientOrderItemListModel>> GetOrderItemsByFilterWithPaging(ClientOrderFilter filter);
        Task<Response<bool>> UpdateClientOrderItemListModelStatus(ClientOrderItemListModel OrderItem);

        Task<List<ClientOrderItemModel>> GetOrderItemsForSendingToEditorPc(int companyId);
        Task<ClientOrderItemModel> GetItemByImageNameAndCompanyId(string fileName, int companyId);
        Task<bool> UpdateOrderItemStatus(ClientOrderItemModel orderItem, InternalOrderItemStatus status);
        /// <summary>
        /// Get Order Files Where Retouched AI is not processed. 
        /// </summary>
        /// <param name="companyId">companyId</param>
        /// <returns>Client Order Item Files</returns>
        Task<List<ClientOrderItemModel>> GetOrderItemForRetouchedAiProcessing(int companyId);
        /// <summary>
        /// Check files retouched.ai processed or not? 
        /// </summary>
        /// <param name="Id"> ClientOrderItemid </param>
        /// <returns>True or False</returns>
        Task<bool> IsRetouchedProcessed(long Id);

        Task<Response<bool>> UpdateOrderItemExpectedDeliveryDate(long clientOrderId, DateTime? expectedDeliveryDate);
        Task<bool> CheckClientOrderItemFile(int CompanyId, string InternalFileInputPath,string FileName, string? CreatedDate);
        Task<Response<bool>> UpdateOrderItemExpectedDeliveryDateByClientOrderId(long ClientOrderId, DateTime? expectedDeliveryDate);
        Task<List<ClientOrderItemModel>> GetAllOrderItemByOrderIdForClient(long orderId);
        Task<Response<bool>> ApprovedOrderItemListCategory(List<ClientOrderItemModel> orderItems, OrderWiseCategoryModel orderWiseCategory);
        Task<ClientOrderCategorySetStatus> GetOrderItemMinCategorySetStatusByOrderId(long orderId);


    }
}
