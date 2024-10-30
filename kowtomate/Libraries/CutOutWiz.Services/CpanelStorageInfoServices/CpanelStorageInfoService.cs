using CutOutWiz.Core;
using CutOutWiz.Core.Models.CpanelStorage;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace CutOutWiz.Services.CpanelStorageInfoServices
{
    public class CpanelStorageInfoService : ICpanelStorageInfoService
    {
        private readonly IConfiguration _configuration;
        public CpanelStorageInfoService(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        public async Task<Response<CpanelStorageInfoViewModel>> GetCpanelStorageInfo()
        {
            var response = new Response<CpanelStorageInfoViewModel>();

            HttpClient client = new HttpClient();

            try
            {
                // Fetch values from configuration
                string url = _configuration.GetSection("CpanelStorage:apiUrl").Value;
                string apiKey = _configuration.GetSection("CpanelStorage:apiKey").Value;

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", apiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                int count = 0;

                while (count <= 3)
                {
                    try
                    {
                        var apiResponse = await client.GetAsync(url);

                        if (apiResponse != null && apiResponse.IsSuccessStatusCode)
                        {
                            var apiResponseParseString = await apiResponse.Content.ReadAsStringAsync();

                            var optionsDeserial = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            };

                            var storageInfoResponse = JsonConvert.DeserializeObject(apiResponseParseString);
                            dynamic storageResponseObject = JObject.Parse(storageInfoResponse.ToString());
                            CpanelStorageInfoViewModel cpanelStorageInfo = new CpanelStorageInfoViewModel()
                            {
                                total= storageResponseObject.results.storage_info[0].total,
                                used = storageResponseObject.results.storage_info[0].used,
                                available = storageResponseObject.results.storage_info[0].available,
                                used_percentage = storageResponseObject.results.storage_info[0].used_percentage,
                            };


                            if (storageInfoResponse != null)
                            {
                                response.IsSuccess = true;
                                response.Result = cpanelStorageInfo;
                            }
                            else
                            {
                                response.Message = "Failed to parse. Response json: " + apiResponseParseString;
                            }
                        }
                        else
                        {
                            response.Message = "No response from api. Json Response:";
                        }

                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Message = "Error on adding product in CA. " + ex.Message;
                        Thread.Sleep(500);
                        count++;
                    }
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }
        //public async Task<Response<List<ProjectWiseCpanelStorageInfoViewModel>>> GetCpanelStorageByProjectWise()
        //{
        //    var response = new Response<List<ProjectWiseCpanelStorageInfoViewModel>>();

        //    HttpClient client = new HttpClient();

        //    try
        //    {
        //        Fetch values from configuration

        //       string url = _configuration.GetSection("CpanelStorage:apiUrl2").Value;
        //        string apiKey = _configuration.GetSection("CpanelStorage:apiKey").Value;

        //        client.BaseAddress = new Uri(url);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", apiKey);
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var responseResult = new List<ProjectWiseCpanelStorageInfoViewModel>();

        //        int count = 0;

        //        while (count <= 3)
        //        {
        //            try
        //            {
        //                var apiResponse = await client.GetAsync(url);

        //                if (apiResponse != null && apiResponse.IsSuccessStatusCode)
        //                {
        //                    var apiResponseParseString = await apiResponse.Content.ReadAsStringAsync();

        //                    var optionsDeserial = new JsonSerializerOptions
        //                    {
        //                        PropertyNameCaseInsensitive = true,
        //                    };

        //                    var storageInfoResponse = JsonConvert.DeserializeObject(apiResponseParseString);
        //                    dynamic storageResponseObject = JObject.Parse(storageInfoResponse.ToString());

        //                    for (int i = 0; i < storageResponseObject.project_storage_info; i++)
        //                    {
        //                        ProjectWiseCpanelStorageInfoViewModel cpanelStorageInfo = new ProjectWiseCpanelStorageInfoViewModel()
        //                        {
        //                            projectname = storageResponseObject.results.project_storage_info[0].projectname,
        //                            used = storageResponseObject.results.project_storage_info[0].used,
        //                        };
        //                        responseResult.Add(cpanelStorageInfo);
        //                    }




        //                    if (storageInfoResponse != null)
        //                    {
        //                        response.IsSuccess = true;
        //                        response.Result = responseResult;
        //                    }
        //                    else
        //                    {
        //                        response.Message = "Failed to parse. Response json: " + apiResponseParseString;
        //                    }
        //                }
        //                else
        //                {
        //                    response.Message = "No response from api. Json Response:";
        //                }

        //                return response;
        //            }
        //            catch (Exception ex)
        //            {
        //                response.Message = "Error on adding product in CA. " + ex.Message;
        //                Thread.Sleep(500);
        //                count++;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = ex.Message;
        //    }

        //    return response;
        //}

        public async Task<Response<List<ProjectWiseCpanelStorageInfoViewModel>>> GetCpanelStorageByProjectWise()
        {
            var response = new Response<List<ProjectWiseCpanelStorageInfoViewModel>>();

            HttpClient client = new HttpClient();

            try
            {
                // Fetch values from configuration
                string url = _configuration.GetSection("CpanelStorage:apiUrl2").Value;
                string apiKey = _configuration.GetSection("CpanelStorage:apiKey").Value;

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", apiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var responseResult = new List<ProjectWiseCpanelStorageInfoViewModel>();

                int count = 0;

                while (count <= 3)
                {
                    try
                    {
                        var apiResponse = await client.GetAsync(url);

                        if (apiResponse != null && apiResponse.IsSuccessStatusCode)
                        {
                            var apiResponseParseString = await apiResponse.Content.ReadAsStringAsync();

                            var storageResponseObject = JObject.Parse(apiResponseParseString);

                            // Check if "project_storage_info" exists in the response
                            if (storageResponseObject["results"]?["project_storage_info"] != null)
                            {
                                var projectStorageInfo = storageResponseObject["results"]["project_storage_info"];

                                foreach (var property in projectStorageInfo)
                                {
                                    ProjectWiseCpanelStorageInfoViewModel cpanelStorageInfo = new ProjectWiseCpanelStorageInfoViewModel()
                                    {
                                        projectname = property.Path.Split(".")[2].ToString(),
                                        used = property.First.ToString()?.ToString(),
                                        // Add other properties as needed
                                    };

                                    responseResult.Add(cpanelStorageInfo);
                                }

                                response.IsSuccess = true;
                                response.Result = responseResult;
                            }
                            else
                            {
                                response.Message = "Missing or invalid 'project_storage_info' in the response.";
                            }
                        }
                        else
                        {
                            response.Message = "No response from API. Status code: " + apiResponse.StatusCode;
                        }

                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Message = "Error on retrieving storage info. " + ex.Message;
                        Thread.Sleep(500);
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message;
            }

            return response;
        }


        private async Task<Response<List<ProjectWiseCpanelStorageInfoViewModel>>> FetchCpanelStorageInfo()
        {
            var response = new Response<List<ProjectWiseCpanelStorageInfoViewModel>>();
            HttpClient client = new HttpClient();

            try
            {
                string url = _configuration.GetSection("CpanelStorage:apiUrl2").Value;
                string apiKey = _configuration.GetSection("CpanelStorage:apiKey").Value;

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", apiKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var responseResult = new List<ProjectWiseCpanelStorageInfoViewModel>();

                int count = 0;

                while (count <= 3)
                {
                    try
                    {
                        var apiResponse = await client.GetAsync(url);

                        if (apiResponse != null && apiResponse.IsSuccessStatusCode)
                        {
                            var apiResponseParseString = await apiResponse.Content.ReadAsStringAsync();

                            var storageResponseObject = JObject.Parse(apiResponseParseString);

                            if (storageResponseObject["results"]?["project_storage_info"] != null)
                            {
                                var projectStorageInfos = storageResponseObject["results"]["project_storage_info"].ToObject<List<ProjectWiseCpanelStorageInfoViewModel>>();

                                responseResult.AddRange(projectStorageInfos);

                                response.IsSuccess = true;
                                response.Result = responseResult;
                            }
                            else
                            {
                                response.Message = "Missing or invalid 'project_storage_info' in the response.";
                            }
                        }
                        else
                        {
                            response.Message = "No response from API. Status code: " + apiResponse.StatusCode;
                        }

                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.Message = "Error on retrieving storage info. " + ex.Message;
                        Thread.Sleep(500);
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = "Error: " + ex.Message;
            }

            return response;
        }

    }
}
