using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.AwsModel;
using CutOutWiz.Services.Models.ApprovalTool;
using CutOutWiz.Services.Models.Security;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text;

namespace CutOutWiz.Services.ApprovalTool
{
    public class ApprovalToolCommonService : IApprovalToolCommonService
    {

        public readonly IConfiguration _configuration;

        public ApprovalToolCommonService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Public Methods
        /// <summary>
        /// Get Message for client
        /// </summary>
        /// <param name="images"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public string GetResponseMessage(List<ImageMoveResponseModel> images, string actionType)
        {
            if (images == null || !images.Any())
            {
                return $"Failed to {GetMessageTextForActionType(false, actionType)} Selected File(s).";
            }

            var message = new StringBuilder();
            var successCount = images.Where(f => f.IsSuccess).Count();

            if (successCount > 0)
            {
                message.Append($"{successCount} file(s) successfull {GetMessageTextForActionType(true, actionType)}<br>");
            }

            var failCount = images.Where(f => !f.IsSuccess).Count();

            if (failCount > 0)
            {
                message.Append($"{failCount} file(s) failed to {GetMessageTextForActionType(false, actionType)}");
            }

            return message.ToString();
        }

        public string GetMessageTextForActionType(bool isSuccess, string actionType)
        {
            if (isSuccess)
            {
                switch (actionType)
                {
                    case ApprovalToolActionTypeConstants.Rejected:
                        return "Rejected";
                    case ApprovalToolActionTypeConstants.Accepted:
                        return "Accepted";

                    default:
                        return string.Empty;
                }
            }

            //For False
            switch (actionType)
            {
                case ApprovalToolActionTypeConstants.Rejected:
                    return "Reject";
                case ApprovalToolActionTypeConstants.Accepted:
                    return "Accept";

                default:
                    return string.Empty;
            }
        }

        public string GetFormattedComments(ImageMoveRequestModel model)
        {
            if (model == null || model.SelectedImages == null)
            {
                return string.Empty;
            }

            var currentDate = DateTime.Now;
            var comment = new StringBuilder();

            //Add Date
            comment.Append($"Date: {currentDate.ToShortDateString()} {currentDate.ToShortTimeString()}{Environment.NewLine}");

            //Add Client Comments
            comment.Append($"Images:{Environment.NewLine}");

            var selectedImages = model.SelectedImages.Split('|');
            var noOfImage = 1;

            foreach (var imgPath in selectedImages)
            {
                var path = imgPath.Split('/').Last();
                comment.Append($"{noOfImage}. {path}{Environment.NewLine}");
                noOfImage++;
            }

            //Add Client Comments
            comment.Append($"Comment:{Environment.NewLine}");
            comment.Append($"{model.Comments}{Environment.NewLine}");

            //Add Reference if any file or markup add
            comment.Append($"-----------------------------------------{Environment.NewLine}{Environment.NewLine}");

            return comment.ToString();
        }

        public string GetFileNameFromPath(string filePath)
        {
            return filePath.Split('/').Last();
        }

        public string GetFormattedCommentForSingleImage(string filePath, string comments)
        {
            if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(comments))
            {
                return string.Empty;
            }

            var currentDate = DateTime.Now;
            var comment = new StringBuilder();

            //Add Date
            comment.Append($"Date: {currentDate.ToShortDateString()} {currentDate.ToShortTimeString()}{Environment.NewLine}");

            //Add Client Comments
            comment.Append($"Image: ");

            var path = filePath.Split('/').Last();
            comment.Append($"{path}{Environment.NewLine}");

            //Add Client Comments
            comment.Append($"Comment: ");
            comment.Append($"{comments}{Environment.NewLine}");

            //Add Reference if any file or markup add
            comment.Append($"-----------------------------------------{Environment.NewLine}{Environment.NewLine}");

            return comment.ToString();
        }


        /// <summary>
        /// Get Destination Path from source path
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public string GetDestinationPath(string actionType, string sourcePath)
        {
            if (actionType == ApprovalToolActionTypeConstants.Accepted)
            {
                return sourcePath.Replace($"/{ApprovalToolImageFoldersConstants.ForApproval}/", $"/{ApprovalToolImageFoldersConstants.Completed}/");
            }
            else if (actionType == ApprovalToolActionTypeConstants.Rejected)
            {
                return sourcePath.Replace($"/{ApprovalToolImageFoldersConstants.ForApproval}/", $"/{ApprovalToolImageFoldersConstants.ForRework}/");
            }

            return string.Empty;
        }

        /// <summary>
        /// Get Comment path
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public string GetDestinationIssueFolderPath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return string.Empty;
            }

            var firstPath = filePath.Split('|')[0];

            var imagePath = firstPath.Replace($"/{ApprovalToolImageFoldersConstants.ForApproval}/", $"/{ApprovalToolImageFoldersConstants.ForRework}/");
            var directoryName = string.Empty;

            int rstr = imagePath.LastIndexOf('/');

            if (rstr > 0)
                directoryName = imagePath.Remove(rstr);

            var actualIssueFolderPath = GetActualDirectoryPathWithoutIssueFolder(directoryName);
            return $"{actualIssueFolderPath}_Issues_{DateTime.Now.ToString("dd MMMM yyyy")}/";
        }

        public string GetParentDirectory(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                return string.Empty;
            }

            var directoryName = string.Empty;
            int rstr = fullPath.LastIndexOf('/');
            if (rstr > 0)
                directoryName = fullPath.Remove(rstr);

            return directoryName;
        }

        /// <summary>
        /// Get AWS Credentials
        /// </summary>
        /// <returns></returns>
        public AwsCredentialsModel GetAwsCredentials()
        {
            return new AwsCredentialsModel
            {
                BucketName = _configuration["AwsSettings:BucketName"],
                AccessKey = _configuration["AwsSettings:AccessKey"],
                SecretAccessKey = _configuration["AwsSettings:SecretAccessKey"],
            };
        }

        private DataTable GetFileTrackingType(){
            var fileTrackingType = new DataTable();
            
            fileTrackingType.Columns.Add("CompanyId", typeof(int));
            fileTrackingType.Columns.Add("SourceDrive", typeof(string));
            fileTrackingType.Columns.Add("ParentDirectory", typeof(string));
            fileTrackingType.Columns.Add("BucketName", typeof(string));
            fileTrackingType.Columns.Add("ActionDate", typeof(DateTime));
            fileTrackingType.Columns.Add("ActionType", typeof(string));
            fileTrackingType.Columns.Add("Attachment", typeof(string));
            fileTrackingType.Columns.Add("Comments", typeof(string));
            fileTrackingType.Columns.Add("MarkupImageUrl", typeof(string));
            fileTrackingType.Columns.Add("FileName", typeof(string));
            fileTrackingType.Columns.Add("Status", typeof(int));
            fileTrackingType.Columns.Add("CreatedByContactId", typeof(int));
            fileTrackingType.Columns.Add("CreatedDateUtc", typeof(DateTime));
            fileTrackingType.Columns.Add("Brand", typeof(string));
            fileTrackingType.Columns.Add("Article", typeof(string));
            fileTrackingType.Columns.Add("FileType", typeof(string));

            return fileTrackingType;
        }

        public DataTable AddFileTrackingRecord(ImageMoveRequestModel model, LoginUserInfoModel currentLoggedInUser, AwsCredentialsModel awsSettings)
            {
            var selectedImages = model.SelectedImages.Split('|');

            var firstImgPath = selectedImages[0];
            var finalPath = GetDestinationPath(model.ActionType, firstImgPath);

            var fileTrackingType = GetFileTrackingType();

            foreach (var image in selectedImages)
            {
                var folders = image.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var fileName = folders[folders.Length - 1];

                var client = folders[0];
                var brand = folders[1];
                var date = folders[2];
                var place = folders[3];
                var article = folders[4];
                var name = fileName.Split('.')[0];
                var type = fileName.Split('.')[1];

                FileTrackingModel fileTracking = new FileTrackingModel
                {
                    CompanyId = currentLoggedInUser.CompanyId,
                    SourceDrive = SourceDriveConstants.AmazonS3,
                    ParentDirectory = GetParentDirectory(finalPath),
                    BucketName = awsSettings.BucketName,
                    ActionDate = DateTime.UtcNow,
                    ActionType = model.ActionType,
                    Attachment = "",
                    Comments = model.Comments ?? "",
                    MarkupImageUrl = "",
                    Status = StatusConstants.Active,
                    CreatedByContactId = currentLoggedInUser.ContactId,
                    CreatedDateUtc = DateTime.UtcNow,
                    Brand = brand.ToString(),
                    Article = article.ToString(),
                    FileName = name,
                    FileType = type,
                };

                fileTrackingType.Rows.Add(
                        fileTracking.CompanyId,
                        fileTracking.SourceDrive,
                        fileTracking.ParentDirectory,
                        fileTracking.BucketName,
                        fileTracking.ActionDate,
                        fileTracking.ActionType,
                        fileTracking.Attachment,
                        fileTracking.Comments,
                        fileTracking.MarkupImageUrl,
                        fileTracking.FileName,
                        fileTracking.Status,
                        fileTracking.CreatedByContactId,
                        fileTracking.CreatedDateUtc,
                        fileTracking.Brand,
                        fileTracking.Article,
                        fileTracking.FileType
                    );

            }

            return fileTrackingType;
        }

        public string GetActualDirectoryPathWithoutIssueFolder(string directoryPath)
        {
            //Set 
            int rstr = directoryPath.LastIndexOf("_Issues_");
            if (rstr > 0)
                directoryPath = directoryPath.Remove(rstr);

            return directoryPath;
        }
        #endregion
    }
}
