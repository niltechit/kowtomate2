using CutOutWiz.Core;
using CutOutWiz.Services.Models.AwsModel;
using Microsoft.AspNetCore.Http;

namespace CutOutWiz.Services.StorageService
{
    public interface IAwsService
    {
        Task<Response<bool>> AddFile(AwsCredentialsModel awsCredentials, IFormFile file, string destination);
        Task<Response<bool>> AddText(AwsCredentialsModel awsCredentials, string destinationKey, string text);
        Task<Response<List<NodeModel>>> GetDirectories(AwsCredentialsModel awsCredentials, string prefix, string delimiter = "/");
        Task<Response<List<DriveImageModel>>> GetFilesByPath(AwsCredentialsModel awsCredentials, string prefix, bool loadRawImages = true);
        Response<string> GetPublicUrl(AwsCredentialsModel awsCredentials, string key);
        Task<Response<bool>> MoveFile(AwsCredentialsModel awsCredentials, string sourceKey, string destinationKey);

        /// <summary>
        /// Delete File from S3
        /// </summary>
        /// <param name="awsCredentials"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Task<Response<bool>> DeleteFile(AwsCredentialsModel awsCredentials, string filePath);

        /// <summary>
        /// Add File
        /// </summary>
        /// <param name="file"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        Task<Response<bool>> AddFile(AwsCredentialsModel awsCredentials, string base64String, string destination);
    }
}