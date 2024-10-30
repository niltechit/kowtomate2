using CutOutWiz.Core;
using CutOutWiz.Services.Models.OrderAssignedImageEditors;

namespace CutOutWiz.Services.OrderTeamServices
{
    public interface IOrderAssignedImageEditorService
    {
        Task<Response<int>> Insert(List<OrderAssignedImageEditorModel> orderTeams);
        Task<Response<bool>> Delete(int orderItemId);
		Task<Response<OrderAssignedImageEditorModel>> CheckClientOrderItemFileAssignEditor(OrderAssignedImageEditorModel orderAssignedImageEditor);

	}
}