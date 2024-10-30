using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Core
{
	public class FileInfoViewModel
	{
		public string URL { get; set; }
		public string FileName { get; set; }
		public IBrowserFile browserFile { get; set; }
	}
}
