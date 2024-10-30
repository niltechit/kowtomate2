using System.Text;

namespace CutOutWiz.Core.Utilities
{
    public class StringHelper
    {

        /// <summary>
        /// Convert a Field to title case
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string SeparateWordsFromString(string fieldName)
        {
            if (fieldName == fieldName.ToUpper())
            {
                // If the entire fieldName is already uppercase, return it as is
                return fieldName;
            }

            StringBuilder result = new StringBuilder();

            foreach (char c in fieldName)
            {
                if (char.IsUpper(c))
                {
                    result.Append(' ');
                }

                result.Append(c);
            }

            return result.ToString().Trim();
        }

        public static string GenerateValidFileName(string inputString, int maxLength)
        {
            // Replace invalid characters with underscores
            string sanitizedString = string.Join("_", inputString.Split(Path.GetInvalidFileNameChars()));

            // Ensure the filename is not too long
            if (sanitizedString.Length > maxLength)
            {
                // Trim the string to the specified length
                sanitizedString = sanitizedString.Substring(0, maxLength);
            }

            // Remove leading or trailing underscores
            sanitizedString = sanitizedString.Trim('_');

            // Ensure the filename is not empty
            if (string.IsNullOrEmpty(sanitizedString))
            {
                // Provide a default name if the input is empty
                sanitizedString = "Universal_Perfumes_PO";
            }

            return sanitizedString;
        }

        public static string RGBToHex(string rgb)
        {
            string[] values = rgb.Replace("rgb(", "").Replace(")", "").Split(',');

            int red = int.Parse(values[0].Trim());
            int green = int.Parse(values[1].Trim());
            int blue = int.Parse(values[2].Trim());

            string hexCode = $"#{red:X2}{green:X2}{blue:X2}";
            return hexCode;
        }

        public static string HexToRGB(string hexCode)
        {
            hexCode = hexCode.TrimStart('#');

            int red = int.Parse(hexCode.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int green = int.Parse(hexCode.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int blue = int.Parse(hexCode.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            string rgb = $"rgb({red}, {green}, {blue})";
            return rgb;
        }

        public static string GetColorValueUsingKeyFormString(string input, string key)
        {
            string[] pairs = input.Split(',');
            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split('=');
                if (keyValue.Length == 2 && keyValue[0] == key)
                {
                    return keyValue[1];
                }
            }

            return null;
        }

    }

    
}
