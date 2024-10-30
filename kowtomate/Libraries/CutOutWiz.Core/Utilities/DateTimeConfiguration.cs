using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Core.Utilities
{
	public class DateTimeConfiguration
	{
		public string currenDateTime { get; set; }= DateTime.Now.ToString("yyyymmddHHmmss");
		public DateTime CurrentDateAndTime { get; set; } = DateTime.Now;
		public string Year { get; set; } = DateTime.Now.ToString("yyyy");
		public string Month { get; set; } = DateTime.Now.ToString("MMMM");
		public string Date { get; set; } = DateTime.Now.ToString("dd.MM.yyyy");

		public string year = "";
        public string month = "";
		public string date = "";

		public async Task DateTimeConvert(DateTime dateTime)
		{
			year = dateTime.ToString("yyyy");
            month = dateTime.ToString("MMMM");
            date = dateTime.ToString("dd.MM.yyyy");
		}
    }
}
