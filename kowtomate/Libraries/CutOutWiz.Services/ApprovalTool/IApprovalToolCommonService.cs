using CutOutWiz.Core;
using CutOutWiz.Services.Models.AwsModel;
using CutOutWiz.Services.Models.ApprovalTool;
using CutOutWiz.Services.Models.Security;
using System.Data;

namespace CutOutWiz.Services.ApprovalTool
{
    public interface IApprovalToolCommonService
    {
        DataTable AddFileTrackingRecord(ImageMoveRequestModel model, LoginUserInfoModel currentLoggedInUser, AwsCredentialsModel awsSettings);
        string GetActualDirectoryPathWithoutIssueFolder(string directoryPath);
        AwsCredentialsModel GetAwsCredentials();
        string GetDestinationIssueFolderPath(string filePath);
        string GetDestinationPath(string actionType, string sourcePath);
        string GetFileNameFromPath(string filePath);
        string GetFormattedCommentForSingleImage(string filePath, string comments);
        string GetFormattedComments(ImageMoveRequestModel model);
        string GetMessageTextForActionType(bool isSuccess, string actionType);
        string GetParentDirectory(string fullPath);
        string GetResponseMessage(List<ImageMoveResponseModel> images, string actionType);
    }
}