using CutOutWiz.Core;
using CutOutWiz.Services.Models.OrderSOP;
using CutOutWiz.Services.Models.SOP;

namespace CutOutWiz.Services.OrderSOP
{
    public interface IOrderTemplateSOPService
    {
        Task<Response<bool>> Delete(OrderSOPTemplateModel orderSOPTemplate);
        Task<List<OrderSOPTemplateModel>> GetAllById(int templateId);
        Task<OrderSOPTemplateModel> GetById(int templateId);
        Task<OrderSOPTemplateModel> GetByIdAndIsDeletedFalse(int templateId);
        Task<Response<int>> Insert(OrderSOPTemplateModel orderSOPtemplate);
        Task<Response<bool>> UpdateSOPTemplateName(OrderSOPTemplateModel orderSOPtemplate);
        Task<Response<bool>> UpdateOrderSOPTemplateInstruction(OrderSOPTemplateModel orderSOPtemplate);
    }
}
