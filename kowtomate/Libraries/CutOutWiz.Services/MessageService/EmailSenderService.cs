using CutOutWiz.Core;
using CutOutWiz.Services.Models.Message;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Text.RegularExpressions;

namespace CutOutWiz.Services.MessageService
{
    public class EmailSenderService : IEmailSenderService
    {
        private string ApiKey { get; set; }
        private string ApiSecretKey { get; set; }
        private string EmailFrom { get; set; }
        private bool EnableSending { get; set; }
        private IConfiguration _configuration { get; set; }

        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
            ApiKey = _configuration["EmailSettings:Mailjet:ApiKey"];
            ApiSecretKey = _configuration["EmailSettings:Mailjet:SecretKey"];
            EmailFrom = _configuration["EmailSettings:FromEmail"];
            EnableSending = Convert.ToBoolean(_configuration["EmailSettings:EnableSending"]);
        }

        /// <summary>
        /// Send Email
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        public async Task<Response<bool>> SendEmail(EmailMessageModel emailMessage)
        {
            var response = new Response<bool>();

            if(!EnableSending)
            {
                response.Message = "Email sending disabled";
                return response;
            }

            try
            {
                emailMessage.Body = Regex.Replace(emailMessage.Body, @"\r\n?|\n", "<br />");

                var receiver = new List<JObject>();

                if (emailMessage.Recipents == null || !emailMessage.Recipents.Any())
                {
                    response.Message = "Not found any recipent.";
                    return response;
                }

                foreach (var address in emailMessage.Recipents)
                {
                    if (address != null)
                    {
                        receiver.Add(new JObject { { "Email", address.Trim() } });
                    }
                }

                MailjetClient client = new MailjetClient(ApiKey, ApiSecretKey)
                {
                    Version = ApiVersion.V3_1,
                };

                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }.Property(Send.Messages, new JArray {
                new JObject {
                     {"From", new JObject {
                      {"Email", EmailFrom},
                      }},

                     {"To", new JArray {
                      receiver
                      }},

                     {"Subject", emailMessage.Subject},

                     {"HTMLPart", emailMessage.Body}
                }
                });

                var emailSendResponse = await client.PostAsync(request);

                //TODO: need to replay message according to status
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Get Email Message
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public EmailMessageModel GetEmailMessage(DataTable fileTracking)
        {
            string Batch = fileTracking.Rows[0]["Article"].ToString();
            string Brand = fileTracking.Rows[0]["Brand"].ToString();
            string ParentDirectory = fileTracking.Rows[0]["ParentDirectory"].ToString();
            string Client = ParentDirectory.Split("/")[0];
            string Comment = fileTracking.Rows[0]["Comments"].ToString();

            List<string> images = new List<string>();
            foreach (DataRow row in fileTracking.Rows)
            {
                string imageName = row["FileName"].ToString() + "." + row["FileType"].ToString();
                images.Add(imageName);
            }

            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo BdZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, BdZone);

            string emailReceiver = _configuration.GetSection("EmailSettings").GetSection("Recipents").Value;

            List<string> emailReceiverList = emailReceiver.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            //string emailBody = $"Images has been rejected by client from COW Proof. order# {orderNo} <br /> Date: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}";

            string emailBody = "Hello Team,"
                        + "<br />"
                        + "<br />"
                        + "<br />"
                        + $"{ images.Count() } images have been rejected by { Client}. Details below:"
                        + "<br />"
                        + "<br />"
                        + "<br />"
                        + $"Client Name: &nbsp;{ Client}"
                        + "<br />"
                        + $"No. of Images: &nbsp; { images.Count() }"
                        + "<br />"
                        + $"Date of Rejection: &nbsp; { localDateTime.ToString("dd-MM-yyyy")}"
                        + "<br />"
                        + $"Time of Rejection: &nbsp; { localDateTime.ToString("hh:mm tt")}"
                        + "<br />"
                        + $"Brand Name: &nbsp; { Brand}"
                        + "<br />" 
                        + $"Batch Details: &nbsp; { Batch}"
                        + "<br />"
                        + $"Comment: {Comment}"
                        + "<br />"
                        + $"Image Name: &nbsp; { string.Join("&nbsp;&nbsp;&nbsp;&nbsp; <br />", images) }"
                        + "<br />"
                        + "<br />"
                        + "<br />"
                        + "Thanks,"
                        + "<br />"
                        + "KOW Approval Tool";

            return new EmailMessageModel
            {
                Subject = $"Image Rejection Notification for {Batch} of {Brand} of {Client}",
                Body = emailBody,
                Recipents = emailReceiverList
            };
        }
    }
}
