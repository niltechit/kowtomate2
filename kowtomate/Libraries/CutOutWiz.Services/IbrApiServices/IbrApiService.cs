using CutOutWiz.Core.Utilities;
using CutOutWiz.Core;
using CutOutWiz.Services.Models.IBRModels;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CutOutWiz.Core.Models.ViewModel;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Services.BLL;

namespace CutOutWiz.Services.IbrApiServices
{
    public class IbrApiService : IIbrApiService
    {

		private readonly IActivityAppLogService _activityAppLogService;
        public IbrApiService(IActivityAppLogService activityAppLogService)
        {
            _activityAppLogService = activityAppLogService;

		}
		public async Task<Response<IbrLoginResponse>> Login(IbrLoginRequest ibrLoginRequest)
        {
            var response = new Response<IbrLoginResponse>();

            try
            {
                var url = $"{AutomatedAppConstant.IbrWebApiBaseUrl}/system-sign-in";

                

                int count = 0;

                var clientRest = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.Post;

                request.AddJsonBody(ibrLoginRequest);


                while (count <= 3)
                {
                    try
                    {
                        //var responseJson = await client.PostAsync(url, content);
                        var responseJson = clientRest.Execute(request);


                        if (responseJson != null && responseJson.IsSuccessStatusCode)
                        {
                            var optionsDeserial = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            };


                            dynamic responseConvertToJson = JsonConvert.DeserializeObject(responseJson.Content);
                            dynamic tokenDeserialized = JObject.Parse(responseConvertToJson.ToString());


                            if (tokenDeserialized != null )
                            {
                                IbrLoginResponse ibrLoginResponse = new IbrLoginResponse()
                                {
                                    token = tokenDeserialized.results.token
                                };

                                response.IsSuccess = true;
                                response.Result = ibrLoginResponse;
                            }
                            else
                            {
                                response.Message = "Failed to parse. Response json: " ;
                            }
                        }
                        else
                        {
                             response.Message = "No response from api. Json Response:" ;
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
				var loginUser = new LoginUserInfoViewModel
				{
					ContactId = AutomatedAppConstant.ContactId
				};
				CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
				{
					//PrimaryId = (int)order.Id,
					ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoDownloadToEditorPc,
					loginUser = loginUser,
					ErrorMessage = ex.Message,
					MethodName = "login",
					RazorPage = "IbrApiService",
					Category = (int)ActivityLogCategory.IbrlogginApi,
				};
				await _activityAppLogService.InsertAppErrorActivityLog(activity);
			}

            return response;
        }

        //     public async Task<Response<string>> RequestForIbrProcess(string masterOrderId, byte[] downloadImageBytes,string ibrToken,string fileName,string model_base_url)
        //     {
        //         var mainResponse = new Response<string>();
        //         var jsonResponseStringForMessage = "";
        //         HttpClient client = new HttpClient();

        //         client.Timeout = TimeSpan.FromMinutes(5);

        //         try
        //         {
        //             //var url = $"{model_base_url}process-image";
        //             //var url = $"{model_base_url}";
        //             var url = "https://api-black.retouched.ai/v.03.13.23/process-image";

        //             client.BaseAddress = new Uri(url);
        //             client.DefaultRequestHeaders.Accept.Clear();
        //             client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ibrToken);

        //             var content = new MultipartFormDataContent();

        //             var fileContent = new ByteArrayContent(downloadImageBytes);
        //             fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

        //             content.Add(fileContent, "file", fileName);
        //             content.Add(new StringContent(masterOrderId), "order_master_id");

        //             int count = 0;

        //             while (count <= 3)
        //             {
        //                 try
        //                 {
        //                     var responseJson = await client.PostAsync(url, content);

        //                     var jsonResponseString = await responseJson.Content.ReadAsStringAsync();
        //			jsonResponseStringForMessage = jsonResponseString;

        //			if (responseJson != null && responseJson.IsSuccessStatusCode)
        //                     {
        //                         var optionsDeserial = new JsonSerializerOptions
        //                         {
        //                             PropertyNameCaseInsensitive = true,
        //                         };

        //                         var ibrProcessReponse = System.Text.Json.JsonSerializer.Deserialize<IbrBaseResponse<IbrImageProcessResults>>(jsonResponseString, optionsDeserial);



        //                         if ( ibrProcessReponse != null && ibrProcessReponse.status == "success")
        //                         {
        //                             mainResponse.IsSuccess = true;
        //                             mainResponse.Result = ibrProcessReponse.Results.output_urls.FirstOrDefault().psd_image_output_public_url;
        //                         }
        //                         else
        //                         {
        //                             mainResponse.Message = "Failed to parse. Response json: " + jsonResponseString;
        //                         }
        //                     }
        //                     else
        //                     {
        //                         //var jsonResponseString11 = await responseJson.Content.ReadAsStringAsync();
        //                         mainResponse.Message = "No response from api. Json Response:" + jsonResponseString;
        //                     }


        //                     return mainResponse;
        //                 }
        //                 catch (Exception ex)
        //                 {
        //			var loginUser = new LoginUserInfoViewModel
        //			{
        //				ContactId = AutomatedAppConstant.ContactId
        //			};
        //			CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
        //			{
        //				//PrimaryId = (int)order.Id,
        //				ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoDownloadToEditorPc,
        //				loginUser = loginUser,
        //				ErrorMessage = ex.Message + jsonResponseStringForMessage,
        //				MethodName = "RequestForIbrProcess",
        //				RazorPage = "IbrApiService",
        //				Category = (int)ActivityLogCategory.IbrProcessingApi,
        //			};
        //			await _activityAppLogService.InsertAppErrorActivityLog(activity);
        //			Thread.Sleep(20000);
        //                     count++;
        //                 }
        //             }

        //         }
        //         catch (Exception ex)
        //         {
        //	var loginUser = new LoginUserInfoViewModel
        //	{
        //		ContactId = AutomatedAppConstant.ContactId
        //	};
        //	CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
        //	{
        //		//PrimaryId = (int)order.Id,
        //		ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoDownloadToEditorPc,
        //		loginUser = loginUser,
        //		ErrorMessage = ex.Message,
        //		MethodName = "GetPsd",
        //		RazorPage = "IbrApiService",
        //		Category = (int)ActivityLogCategory.IbrProcessingApi,
        //	};
        //	await _activityAppLogService.InsertAppErrorActivityLog(activity);
        //}

        //         return mainResponse;
        //     }

        public async Task<Response<string>> RequestForIbrProcess(string masterOrderId, byte[] downloadImageBytes, string ibrToken, string fileName, string model_base_url)
        {
            var mainResponse = new Response<string>();
            var jsonResponseStringForMessage = "";
            const int maxRetries = 3;

            //RestClient restClient = new RestClient("https://api-black.retouched.ai/v.03.13.23");
            RestClient restClient = new RestClient(model_base_url);

            try
            {
                for (int count = 0; count < maxRetries; count++)
                {
                    RestRequest request = new RestRequest("process-image", Method.Post);
                    request.AddHeader("Authorization", "bearer " + ibrToken);
                    request.AddParameter("order_master_id", masterOrderId);
                    request.AddFile("file", downloadImageBytes, fileName, "application/octet-stream");

                    var response = await restClient.ExecuteAsync(request);

                    var jsonResponseString = response.Content;
                    jsonResponseStringForMessage = jsonResponseString;

                    if (response.IsSuccessful)
                    {
                        var optionsDeserial = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };

                        var ibrProcessReponse = System.Text.Json.JsonSerializer.Deserialize<IbrBaseResponse<IbrImageProcessResults>>(jsonResponseString, optionsDeserial);

                        if (ibrProcessReponse != null && ibrProcessReponse.status == "success")
                        {
                            mainResponse.IsSuccess = true;
                            mainResponse.Result = ibrProcessReponse.Results.output_urls.FirstOrDefault().psd_image_output_public_url;
                        }
                        else
                        {
                            mainResponse.Message = "Failed to parse. Response json: " + jsonResponseString;
                        }
                    }
                    else if ((int)response.StatusCode >= 500 && (int)response.StatusCode < 600)
                    {
                        mainResponse.Message = "Server error. HTTP Status Code: " + (int)response.StatusCode;
                    }
                    else
                    {
                        mainResponse.Message = "Non-success status code. HTTP Status Code: " + (int)response.StatusCode + ". Json Response: " + jsonResponseString;
                    }

                    return mainResponse;
                }
            }
            catch (Exception ex)
            {
                var loginUser = new LoginUserInfoViewModel
                {
                    ContactId = AutomatedAppConstant.ContactId
                };

                CommonActivityLogViewModel activity = new CommonActivityLogViewModel()
                {
                    ActivityLogFor = (int)ActivityLogCategoryConsoleApp.AutoDownloadToEditorPc,
                    loginUser = loginUser,
                    ErrorMessage = ex.Message + jsonResponseStringForMessage,
                    MethodName = "RequestForIbrProcess",
                    RazorPage = "IbrApiService",
                    Category = (int)ActivityLogCategory.IbrProcessingApi,
                };

                await _activityAppLogService.InsertAppErrorActivityLog(activity);
                await Task.Delay(20000);
            }

            return mainResponse;
        }


        public async Task<Response<IbrDefaultSettingsApiResponse>> GetIbrGeneralSetting(string ibrToken)
        {
            var response = new Response<IbrDefaultSettingsApiResponse>();

            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(5);

            try
            {
                var url = $"{AutomatedAppConstant.IbrWebApiBaseUrl}/default-settings";

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.UserAgent.ParseAdd("KowToMateAPIClient/1.0");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", ibrToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                int count = 0;

                while (count <= 3)
                {
                    try
                    {
                        var responseJson = await client.GetAsync(url);

                       

                        if (responseJson != null && responseJson.IsSuccessStatusCode)
                        {
                            var jsonResponseString = await responseJson.Content.ReadAsStringAsync(); 

                            var optionsDeserial = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            };

                            //var ibrApiResposneModel = System.Text.Json.JsonSerializer.Deserialize<IbrDefaultSettingsApiResponse>(jsonResponseString, optionsDeserial);
                            var ibrApiResposneModel = JsonConvert.DeserializeObject(jsonResponseString);
                            dynamic ibrResObject= JObject.Parse(ibrApiResposneModel.ToString());
                            IbrDefaultSettingsApiResponse ibrDefaultSettings = new IbrDefaultSettingsApiResponse()
                            {
                                service_type_id = ibrResObject.results.default_settings[0].service_type_id,
                                menu_id = ibrResObject.results.default_settings[0].menu_id,
                                subscription_plan_type_id = ibrResObject.results.default_settings[0].subscription_plan_type_id,
                                model_base_url = ibrResObject.results.default_settings[0].model_base_url,
                                downloadable_image_qty_limit = ibrResObject.results.default_settings[0].downloadable_image_qty_limit,
                                processable_image_qty_limit = ibrResObject.results.default_settings[0].processable_image_qty_limit
							};
                            

                            //var objectob=ibrApiResposneModel.results.FirstOrDefault();

                            if (ibrApiResposneModel != null)
                            {
                                response.IsSuccess = true;
                                response.Result = ibrDefaultSettings;
                            }
                            else
                            {
                                response.Message = "Failed to parse. Response json: " + jsonResponseString;
                            }
                        }
                        else
                        {
                            //var jsonResponseString11 = await responseJson.Content.ReadAsStringAsync();
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

        public async Task<Response<IbrOrderMasterInfoResponse>> GetOrderMasterId(IbrDefaultSettingsApiResponse ibrDefaultSettingsApiResponse,string ibrToken)
        {
            var response = new Response<IbrOrderMasterInfoResponse>();

            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(5);
            
            try
            {
               
                var url = $"{AutomatedAppConstant.IbrWebApiBaseUrl}/order-master-info";

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                // Set the User-Agent header in the default request headers
                client.DefaultRequestHeaders.UserAgent.ParseAdd("KowToMateAPIClient/1.0");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", ibrToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //var options = new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull };
                var json = System.Text.Json.JsonSerializer.Serialize(new IbrOrderMasterInfoRequest
                {
                    menu_id = ibrDefaultSettingsApiResponse.menu_id,
                    service_type_id = ibrDefaultSettingsApiResponse.service_type_id,
                    subscription_plan_type_id = ibrDefaultSettingsApiResponse.subscription_plan_type_id,
                    file_upload_from = 1, 
                    file_upload_by = 1
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                int count = 0;

                while (count <= 3)
                {
                    try
                    {
                        var responseJson = await client.PostAsync(url,content);

                        if (responseJson != null && responseJson.IsSuccessStatusCode)
                        {
                            var jsonResponseString = await responseJson.Content.ReadAsStringAsync();

                            var optionsDeserial = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            };

                            //var ibrOrderMasterInfoResponse = System.Text.Json.JsonSerializer.Deserialize<IbrOrderMasterInfoResponse>(jsonResponseString, optionsDeserial);
                            var ibrOrderMasterInfoResponse = JsonConvert.DeserializeObject(jsonResponseString);
                            dynamic ibrResObject = JObject.Parse(ibrOrderMasterInfoResponse.ToString());
                            IbrOrderMasterInfoResponse ibrOrderMasterInfo = new IbrOrderMasterInfoResponse()
                            {
                                order_id = ibrResObject.results.order_master_info.order_id,
                                
                            };

                            if (ibrOrderMasterInfoResponse != null)
                            {
                                response.IsSuccess = true;
                                response.Result = ibrOrderMasterInfo;
                            }
                            else
                            {
                                response.Message = "Failed to parse. Response json: " + jsonResponseString;
                            }
                        }
                        else
                        {
                            //var jsonResponseString11 = await responseJson.Content.ReadAsStringAsync();
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

    }
}
