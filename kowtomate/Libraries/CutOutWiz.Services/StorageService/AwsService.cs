using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.AwsModel;
using Microsoft.AspNetCore.Http;

namespace CutOutWiz.Services.StorageService
{
    public class AwsService : 
        IAwsService
    {                         
        /// <summary>
        /// Get directories
        /// </summary>
        /// <param name="awsCredentials"></param>
        /// <param name="prefix"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public async Task<Response<List<NodeModel>>> GetDirectories(AwsCredentialsModel awsCredentials, string prefix, string delimiter = "/")
        {
            var methodResponse = new Response<List<NodeModel>>();
            
            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = awsCredentials.BucketName,
                    Prefix = prefix,
                    Delimiter = delimiter
                };

                List<NodeModel> directories = new List<NodeModel>();

                using (var s3 = new AmazonS3Client(awsCredentials.AccessKey, awsCredentials.SecretAccessKey, RegionEndpoint.USEast1))
                {
                    var Responce = await s3.ListObjectsV2Async(request);
                    var CommonPrefixes = Responce.CommonPrefixes.ToList();
                   
                    List<string> DirectoryListWithoutCompleted = new List<string>();

                    foreach (var cm in CommonPrefixes)
                    {
                        DirectoryListWithoutCompleted.Add(cm.TrimEnd('/'));
                    }

                    foreach (var d in DirectoryListWithoutCompleted)
                    {
                        var folderName = d.Replace(prefix, string.Empty);
                        directories.Add(new NodeModel(d, folderName, true));
                    }
                }

                methodResponse.IsSuccess = true;
                methodResponse.Result = directories;
            }
            catch (Exception ex)
            {
                methodResponse.Message = ex.Message;
            }

            return methodResponse;
        }
        /// <summary>
        /// Get Files without depending file extension
        /// </summary>
        /// <param name="awsCredentials"></param>
        /// <param name="prefix"></param>
        /// <param name="loadRawImages"></param>
        /// <returns></returns>
        public async Task<Response<List<DriveImageModel>>> GetFilesByPath(AwsCredentialsModel awsCredentials, string prefix, bool loadRawImages = true)
        {
            var methodResponse = new Response<List<DriveImageModel>>();
            string completePrefix = $"{prefix}/";
            string completePrefixForToProcess = completePrefix.Replace($"/{ApprovalToolImageFoldersConstants.ForApproval}/", $"/{ApprovalToolImageFoldersConstants.ToProcess}/");

            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = awsCredentials.BucketName,
                    Prefix = completePrefix
                };

                ListObjectsV2Request requestForToProcess = new ListObjectsV2Request
                {
                    BucketName = awsCredentials.BucketName,
                    Prefix = completePrefixForToProcess
                };

                List<DriveImageModel> Files = new List<DriveImageModel>();

                using (var s3 = new AmazonS3Client(awsCredentials.AccessKey, awsCredentials.SecretAccessKey, RegionEndpoint.USEast1))
                {
                    var Responce = await s3.ListObjectsV2Async(request);
                    var ResponseForToProcess = await s3.ListObjectsV2Async(requestForToProcess);

                    var imageInfos = Responce.S3Objects; 
                    var imageInfoForToProcess = ResponseForToProcess.S3Objects;

                    foreach (var imageInfo in imageInfos)
                    {
                        var fileName = imageInfo.Key.Replace(completePrefix, string.Empty);
                        if (fileName != "")
                        {
                            foreach (var imgInfoForToProcess in imageInfoForToProcess)
                            {
                                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                                var fileNameForToProcess = imgInfoForToProcess.Key.Replace(completePrefixForToProcess, string.Empty);
                                var fileNameWithoutExtensionForToProcess = Path.GetFileNameWithoutExtension(fileNameForToProcess);

                                if (fileNameWithoutExtension != fileNameWithoutExtensionForToProcess)
                                {
                                    continue;
                                }
                                else
                                {
                                    var prefixSplit = prefix.Split("/");
                                    var fileSize = Convert.ToInt64(imageInfo.Size);
                                    var image = new DriveImageModel(prefix, fileName, imageInfo.Key, fileSize, imageInfo.LastModified);

                                    if (loadRawImages)
                                    {
                                        var rawUrl = imgInfoForToProcess.Key;
                                        //var publicRawUrlResponse = GetPublicUrl(awsCredentials.BucketName,imageInfo.Key, s3);
                                        image.RawImagePath = rawUrl;
                                    }
                                    Files.Add(image);
                                    break;
                                }
                            }
                        } 
                    }
                }

                methodResponse.IsSuccess = true;
                methodResponse.Result = Files;
            }
            catch (Exception ex)
            {
                methodResponse.Message = ex.Message;
            }

            return methodResponse;
        }

        /// <summary>
        /// Get Public Url
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bucketName"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private Response<string> GetPublicUrl(string bucketName, string key, AmazonS3Client client)
        {
            var response = new Response<string>();

            try
            {
                var uploadRequest = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    Expires = DateTime.Now.AddDays(7)
                };

                response.Result = client.GetPreSignedURL(uploadRequest);
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Get Public Url
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bucketName"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public Response<string> GetPublicUrl(AwsCredentialsModel awsCredentials, string key)
        {
            var response = new Response<string>();

            try
            {
                var uploadRequest = new GetPreSignedUrlRequest
                {
                    BucketName = awsCredentials.BucketName,
                    Key = key,
                    Expires = DateTime.Now.AddDays(7)
                };

                using (var s3 = new AmazonS3Client(awsCredentials.AccessKey, awsCredentials.SecretAccessKey, RegionEndpoint.USEast1))
                {
                    response.Result = s3.GetPreSignedURL(uploadRequest);
                }

                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Move File
        /// </summary>
        /// <param name="sourceKey"></param>
        /// <param name="destinationKey"></param>
        /// <returns></returns>
        public async Task<Response<bool>> MoveFile(AwsCredentialsModel awsCredentials, string sourceKey, string destinationKey)
        {
            var response = new Response<bool>();

            try
            {
                using (var s3 = new AmazonS3Client(awsCredentials.AccessKey, awsCredentials.SecretAccessKey, RegionEndpoint.USEast1))
                {
                    await s3.CopyObjectAsync(awsCredentials.BucketName, sourceKey, awsCredentials.BucketName, destinationKey);
                    await s3.DeleteObjectAsync(awsCredentials.BucketName, sourceKey);
                }
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Add File
        /// </summary>
        /// <param name="file"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public async Task<Response<bool>> AddFile(AwsCredentialsModel awsCredentials, IFormFile file, string destination)
        {
            var response = new Response<bool>();

            try
            {
                using (var client = new AmazonS3Client(awsCredentials.AccessKey, awsCredentials.SecretAccessKey, RegionEndpoint.USEast1))
                {
                    using (var newMemoryStream = new MemoryStream())
                    {
                        file.CopyTo(newMemoryStream);

                        var uploadRequest = new TransferUtilityUploadRequest
                        {
                            InputStream = newMemoryStream,
                            Key = file.FileName,
                            BucketName = destination,// $"approvaltool/client 1/Brand 1/to rework/ xyz;
                            CannedACL = S3CannedACL.PublicRead
                        };

                        var fileTransferUtility = new TransferUtility(client);
                        await fileTransferUtility.UploadAsync(uploadRequest);
                    }
                }

                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Add File
        /// </summary>
        /// <param name="file"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public async Task<Response<bool>> AddFile(AwsCredentialsModel awsCredentials, string base64String, string destination)
        {
            var response = new Response<bool>();

            try
            {
                using (var client = new AmazonS3Client(awsCredentials.AccessKey, awsCredentials.SecretAccessKey, RegionEndpoint.USEast1))
                {
                    byte[] bytes = Convert.FromBase64String(base64String);
                    
                    var request = new PutObjectRequest
                    {
                        BucketName = awsCredentials.BucketName,
                        CannedACL = S3CannedACL.PublicRead,
                        Key = destination
                    };
                    using (var ms = new MemoryStream(bytes))
                    {
                        request.InputStream = ms;
                        await client.PutObjectAsync(request);
                    }                   
                }

                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Add Comments as text
        /// </summary>
        /// <param name="destinationKey"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<Response<bool>> AddText(AwsCredentialsModel awsCredentials, string destinationKey, string text)
        {
            var response = new Response<bool>();

            try
            {
                using (var s3 = new AmazonS3Client(awsCredentials.AccessKey, awsCredentials.SecretAccessKey, RegionEndpoint.USEast1))
                {
                    var putRequest = new PutObjectRequest
                    {
                        BucketName = awsCredentials.BucketName,// $"approvaltool/client 1/Brand 1/to rework/ xyz;
                        Key = destinationKey,
                        ContentBody = text,
                        ContentType = "text/plain"
                    };

                    await s3.PutObjectAsync(putRequest);
                }

                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Delete File from S3
        /// </summary>
        /// <param name="awsCredentials"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async Task<Response<bool>> DeleteFile(AwsCredentialsModel awsCredentials, string filePath)
        {
            var response = new Response<bool>();

            try
            {
                using (var s3 = new AmazonS3Client(awsCredentials.AccessKey, awsCredentials.SecretAccessKey, RegionEndpoint.USEast1))
                {
                    await s3.DeleteObjectAsync(awsCredentials.BucketName, filePath);
                }
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

    }
}
