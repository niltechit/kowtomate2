namespace CutOutWiz.Data.Entities.Common
{
	public class CountryEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Code { get; set; }
		public string ObjectId { get; set; }
		public DateTime CreatedDate { get; set; }
		public int CreatedByContactId { get; set; }
		public DateTime? UpdatedDate { get; set; }
		public int? UpdatedByContactId { get; set; }
	}
}
