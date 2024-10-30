namespace CutOutWiz.Services.Models.OrderAndOrderItemStatusChangeLog
{
    public class OrderItemStatusChangeLogModel
    {
		public int Id { get; set; }
		public int OrderFileId { get; set; }
		public byte? OldInternalStatus { get; set; }
		public byte? NewInternalStatus { get; set; }
		public byte? OldExternalStatus { get; set; }
		public string Note { get; set; }
		public int ChangeByContactId { get; set; }
		public DateTime ChangeDate { get; set; }
		public int? TimeDurationInMinutes { get; set; }
		public bool? ClientCanView { get; set; }
		public byte? NewExternalStatus { get; set; }

		public string ChangeByFirstName { get; set; }
		public string ChangeByLastName { get; set; }
		public string EmployeeId { get; set; }
		public string ChangeByFullName => ChangeByFirstName + " " + ChangeByLastName;
	}
}
