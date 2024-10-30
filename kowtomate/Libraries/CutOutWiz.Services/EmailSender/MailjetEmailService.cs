using CutOutWiz.Core;
using CutOutWiz.Services.Models.EmailSender;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace CutOutWiz.Services.EmailSender
{
    public class MailjetEmailService : IMailjetEmailService
    {
        public MailjetEmailService()
        {
            //EnableSending = Convert.ToBoolean(_configuration["EmailSettings:EnableSending"]);
        }

        /// <summary>
        /// Send Email
        /// </summary>
        /// <param name="emailSendRequest"></param>
        /// <returns></returns>
        public async Task<Response<bool>> SendEmail(EmailSendRequestModel emailSendRequest)
        {
            var response = new Response<bool>();

            //if (!EnableSending)
            //{
            //    response.Message = "Email sending disabled";
            //    return response;
            //}

            try
            {
                emailSendRequest.Body = Regex.Replace(emailSendRequest.Body, @"\r\n?|\n", "<br />");

                var receiver = new List<JObject>();

                if (string.IsNullOrWhiteSpace(emailSendRequest.ToEmail))
                {
                    response.Message = "Not found any recipent.";
                    return response;
                }

                receiver.Add(new JObject { { "Email", emailSendRequest.ToEmail.Trim() } });

                //foreach (var address in emailMessage.Recipents)
                //{
                //    if (address != null)
                //    {
                //        receiver.Add(new JObject { { "Email", address.Trim() } });
                //    }
                //}

                MailjetClient client = new MailjetClient(emailSendRequest.ApiKey, emailSendRequest.ApiSecret)
                {
                    Version = ApiVersion.V3_1,
                };

                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }.Property(Send.Messages, new JArray {
                new JObject {
                     {"From", new JObject {
                      {"Email", emailSendRequest.FromEmail},
                      }},

                     {"To", new JArray {
                      receiver
                      }},

                     {"Subject", emailSendRequest.Subject},

                     {"HTMLPart", emailSendRequest.Body}
                }
                });

                var emailSendResponse = await client.PostAsync(request);

                //TODO: need to replay message according to status
                response.IsSuccess = emailSendResponse.IsSuccessStatusCode;
                
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
