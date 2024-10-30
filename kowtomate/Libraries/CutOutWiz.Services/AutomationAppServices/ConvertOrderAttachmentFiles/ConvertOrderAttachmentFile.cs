using CutOutWiz.Core.Utilities;
using CutOutWiz.Core.Models.ViewModel;
using CutOutWiz.Services.BLL;
using MimeKit;
using System.Text.RegularExpressions;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.AutomationAppServices.ConvertOrderAttachmentFiles
{
    public class ConvertOrderAttachmentFile: IConvertOrderAttachmentFile
    {
        private readonly IActivityAppLogService _activityAppLogService;

        public ConvertOrderAttachmentFile(IActivityAppLogService activityAppLogService)
        {
            _activityAppLogService = activityAppLogService;
        }

        public async Task<MemoryStream> ConvertEMLFile(MemoryStream inputStream)
        {
            try
            {
                inputStream.Position = 0;
                // Read the .eml file from the MemoryStream
                MimeMessage message = MimeMessage.Load(inputStream);

                // Extract the text
                var body = message.TextBody ?? message.HtmlBody;

                if (!string.IsNullOrEmpty(body))
                {
                    // Remove email addresses
                    string emailPattern = @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}\b";
                    string cleanedBody = Regex.Replace(body, emailPattern, "", RegexOptions.IgnoreCase);

                    // Create a new text or HTML part with the cleaned content
                    if (message.TextBody != null)
                    {
                        var textPart = message.BodyParts.OfType<TextPart>().FirstOrDefault(tp => tp.IsPlain);
                        if (textPart != null)
                        {
                            textPart.Text = cleanedBody;
                        }
                    }
                    else if (message.HtmlBody != null)
                    {
                        var htmlPart = message.BodyParts.OfType<TextPart>().FirstOrDefault(tp => tp.IsHtml);
                        if (htmlPart != null)
                        {
                            htmlPart.Text = cleanedBody;
                        }
                    }

                    // Clear the recipients
                    message.From.Clear();
                    message.To.Clear();

                    // Save the cleaned message to a new MemoryStream
                    var outputStream = new MemoryStream();
                    message.WriteTo(outputStream);
                    outputStream.Position = 0; // Reset the position of the stream to the beginning

                    Console.WriteLine("Cleaned .eml file has been processed and returned as a stream.");
                    return outputStream;
                }
                else
                {
                    Console.WriteLine("The email does not contain a text or HTML body.");
                    return null;
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Failed to parse the .eml file: " + ex.Message);
                await AddLog($"Failed to parse the .eml file. Exceptoin: {ex.Message}"); 
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);

                await AddLog($"Exceptoin: {ex.Message}");
                return null;
            }
        }

        private async Task AddLog(string exceptionMessage)
        {
            CommonActivityLogViewModel activity = new CommonActivityLogViewModel
            {
                CreatedByContactId = AutomatedAppConstant.ContactId,
                ActivityLogFor = (int)ActivityLogForConstants.GeneralLog,
                PrimaryId = 0,
                ErrorMessage = exceptionMessage,
                MethodName = "ConvertEMLFile",
                RazorPage = "ConvertOrderAttachmentFile",
                Category = (int)ActivityLogCategory.OrderDeliveryToClient,
            };
            await _activityAppLogService.InsertAppErrorActivityLog(activity);
        }
    }
}
