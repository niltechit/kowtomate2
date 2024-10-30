namespace CutOutWiz.Data
{
    public class StandardDataAccessMessages
    {
        public static string SuccessMessaage = "Saved Successfully";
        public static string ErrorOnAddMessaage = "Error on adding";
		public static string RequiredIdForDelete = "Id is required.";

		public static string DeleteMessaage(string record)
        {
            return $"The {record} was deleted.";
        }

        public static string GetSqlErrorMessage(Exception ex, string fieldName = "Record")
        {
            if (ex.Message == null)
            {
                return "";
            }

            if (ex.Message.Contains("insert duplicate key"))
            {
                return $"{fieldName} already exist.";
            }

            if(ex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
            {
                return $"{fieldName} already in used.";
            }

            return ex.Message.Substring(0, Math.Min(500, ex.Message.Length));
        }
    }
}
