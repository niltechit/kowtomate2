
namespace CutOutWiz.Services.Models.OrderAssignedImageEditors
{
    public class OrderAssignedImageEditorModel
    {
		public int Id { get; set; }
		public long OrderId { get; set; }
		public int AssignContactId { get; set; }
		public int AssignByContactId { get; set; }
		public long Order_ImageId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
		public DateTime? AssignDate { get; set; }
		public string ObjectId { get; set; }
	}
}
