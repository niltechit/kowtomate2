using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Helper
{
	public class TimeConverter
	{
		public string MinuteToHour(int minute)
		{
			var receiveMinute = 0;
			var receiveHour = 0;
			var receiveDay = 0;
			var receiveMonth = 0;
			var receiveYear = 0;
			var result = "";

			if (minute>60)
			{
				decimal ConvertedreceiveHour =Convert.ToDecimal(minute / 60);
				decimal ConvertedreceiveHour2 =Convert.ToDecimal(minute % 60);
				receiveHour = Convert.ToInt32(ConvertedreceiveHour);
				string formattedReceiveHour = ConvertedreceiveHour.ToString("00");
				string formattedReceiveHour2 = ConvertedreceiveHour2.ToString("00");
				result = $"{formattedReceiveHour}:{formattedReceiveHour2} Hours";
			}
			if (receiveHour>24)
			{
				decimal ConvertedreceiveHour = Convert.ToDecimal((minute / 60)/24);
				decimal ConvertedreceiveHour2 = Convert.ToDecimal((minute % 60)/24);
				receiveHour = Convert.ToInt32(ConvertedreceiveHour);
				string formattedReceiveHour = ConvertedreceiveHour.ToString("00");
				string formattedReceiveHour2 = ConvertedreceiveHour2.ToString("00");
				result = $"{formattedReceiveHour}:{formattedReceiveHour2} Day";
			}

			return result;
		}
	}
}
