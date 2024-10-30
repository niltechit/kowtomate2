using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Core
{
	public class GenericServices
	{
        // Testing Purpose : TODO : Zakir
        private async Task<string> CreateTextFileName<T>(List<T> files)
        {
            var text = "";
            var fileName = "";
            var filesArray = files.ToArray();
            for (int i = 0; i < filesArray.ToList().Count; i++)
            {
                fileName += $"{i}) {filesArray[i]}\n";
            }
            text = $"File Names : \n {fileName}\n";
            return text;
        }
        public string GetSizeStringToHumanReadable(long? length)
        {
			long B = 0, KB = 1024, MB = KB * 1024, GB = MB * 1024, TB = GB * 1024;
			double? size = length;
			string suffix = nameof(B);

			if (length >= TB)
			{
				size = Math.Round((double)length / TB, 2);
				suffix = nameof(TB);
			}
			else if (length >= GB)
			{
				size = Math.Round((double)length / GB, 2);
				suffix = nameof(GB);
			}
			else if (length >= MB)
			{
				size = Math.Round((double)length / MB, 2);
				suffix = nameof(MB);
			}
			else if (length >= KB)
			{
				size = Math.Round((double)length / KB, 2);
				suffix = nameof(KB);
			}

			return $"{size} {suffix}";
		}
		/// <summary>
		/// Here provide your constant class and return list of contant.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
        public static IEnumerable<FieldInfo> GetConstants<T>()
        {
            Type type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return fields.Where(f => f.IsLiteral && !f.IsInitOnly);
        }
    }
}
