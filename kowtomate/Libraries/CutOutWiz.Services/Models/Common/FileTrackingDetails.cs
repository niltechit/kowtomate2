using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Core
{
	public class FileTrackingDetails
	{
		public int Id { get; set; }
		public int? FileTrackingId { get; set; }
		public string FileName { get; set; }
		public string FilePathUrl { get; set; }
		public string FileType { get; set; }
		public long? FileSize { get; set; }
		public string FileMarkUp { get; set; }

	}
}
