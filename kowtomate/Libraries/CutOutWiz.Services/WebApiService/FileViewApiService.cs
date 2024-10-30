using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.IBRModels;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Core;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static CutOutWiz.Core.Utilities.Enums;
using DocumentFormat.OpenXml.Drawing.Charts;
using CutOutWiz.Services.Models.Common;
using Microsoft.Extensions.Configuration;

namespace CutOutWiz.Services.WebApiService
{
    public class FileViewApiService : IFileViewApiService
    {
        private readonly IConfiguration _configuration;
        private string _apiBaseURL;
        public FileViewApiService(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiBaseURL = _configuration.GetSection("GeneralSettings:ImageViewApiURL").Value;
        }
        public string ConvertedFileAndGetOutputUrl(string filePath,FileServerModel fileServer,int width, int height)
        {
            string returnImageUrl;
            try
            {
                if (string.IsNullOrEmpty(_apiBaseURL)) return "Api Base Url Null";
                
                string baseUrl = $"{_apiBaseURL}api/2024-09/file-show-from-ftp-path";
                string apiKey = "l7DyKJtb56nmzgvPYN/3oUgLjQ2/fGKxTIGB5mguBld5gUh3rb7ez5Urg1+tA3Y1aeMMsGYNNHTUsHugV5CXFw==";
                string ftpCredentialWithEncoded = "UVg0++nunvKhCtFIh4w0dm058ONv5yyb/vzyInvfmqTy597co0x2vLKgSpcycXbPheIaf76w4yxPQc2fMjaxtg==";

                returnImageUrl = ConvertedFileToJpeg(baseUrl, apiKey, ftpCredentialWithEncoded, filePath,fileServer,width,height);
            }
            catch (Exception ex)
            {
                returnImageUrl = string.Empty;
                Console.WriteLine($"Unhandled exception: {ex.Message}");
            }
            return returnImageUrl;
        }

        private string ConvertedFileToJpeg(string baseUrl, string apiKey, string ftpCredentialWithEncoded, string filePath,FileServerModel fileServer, int width = 500, int height = 500)
        {
            var url = string.Empty;
            try
            {
                var splitedFilePath = @$"{fileServer.SubFolder}{filePath.Replace("\\", "/")}";
                
                // Build the query string with the provided parameters
                url = $"{baseUrl}?apiKey={Uri.EscapeDataString(apiKey)}" +
                          $"&ftpCredentialWithEncoded={Uri.EscapeDataString(ftpCredentialWithEncoded)}" +
                          $"&remoteFilePath={Uri.EscapeDataString(splitedFilePath)}" +
                          $"&width={width}&height={height}";

                var clientRest = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.Get;

                int maxRetries = 3;
                int retryCount = 0;

                while (retryCount <= maxRetries)
                {
                    try
                    {
                        var response = clientRest.Execute(request);

                        if (response != null && response.IsSuccessStatusCode)
                        {
                            // Handle successful response
                            return url;
                        }

                        if (response != null)
                        {
                            Console.WriteLine($"Failed attempt {retryCount + 1}. StatusCode: {response.StatusCode}, Response: {response.Content}");
                            return url;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during attempt {retryCount + 1}: {ex.Message}");
                    }

                    retryCount++;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex.Message}");
                return string.Empty;
            }
            return url;
        }

    }
}
