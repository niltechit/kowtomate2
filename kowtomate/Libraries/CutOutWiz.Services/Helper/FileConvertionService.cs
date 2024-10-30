using CutOutWiz.Services.Models.Common;
using CutOutWiz.Services.StorageService;
using FluentFTP;
using FluentFTP.Exceptions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Helper
{
    

    public class FileConvertionService : IFileConvertionService
    {
        private readonly IPDFConverstionService _converstionService;
        public FileConvertionService(IPDFConverstionService converstionService)
        {

            _converstionService = converstionService;

        }

        /// <summary>
        /// Convert Any type of file to PNG and Return Base64String
        /// </summary>
        /// <param name="fileServer"></param>
        /// <param name="ftpFilePath"></param>
        /// <returns>Base64String of image</returns>

        /// <summary>
        /// Convert any type of file to PNG and return Base64 string.
        /// </summary>
        /// <param name="fileServer">The file server details.</param>
        /// <param name="ftpFilePath">The path to the file on the FTP server.</param>
        /// <returns>Base64 string of the image.</returns>
        public async Task<string> ConvertFileToPNG(FileServerModel fileServer, string ftpFilePath,string _webHostEnvironment)
        {
            try
            {
                // Download the file content from the FTP server
                var fileContent = await DownloadFileAsync(fileServer, ftpFilePath);
                if (fileContent == null)
                {
                    throw new Exception("Downloaded file content is null.");
                }

                // Determine the file extension
                var fileExtension = Path.GetExtension(ftpFilePath)?.ToLower();
                if (fileExtension == null)
                {
                    throw new Exception("File extension is null.");
                }

                // Create an image from the file content based on file type
                Bitmap image = null;
                if (fileExtension == ".pdf")
                {
                    var images = await _converstionService.ConvertPdfFileToPNG(fileServer, ftpFilePath, _webHostEnvironment);
                    return images.ToString();
                }
                else
                {
                    image = await CreateImageFromText(fileContent);
                }

                if (image == null)
                {
                    throw new Exception("Failed to create image from file content.");
                }

                // Convert the image to a PNG and encode it as a base64 string
                using var memoryStream = new MemoryStream();
                image.Save(memoryStream, ImageFormat.Png);
                var convertedMemoryStream = memoryStream.ToArray();
                var convertBase64 = Convert.ToBase64String(convertedMemoryStream);

                var imageUri = $"data:image/png;base64,{convertBase64}";

                return imageUri;
            }
            catch (Exception ex)
            {
                // Log the exception (you can replace this with your logging framework)
                Console.WriteLine($"Error in ConvertFileToPNG: {ex.Message}");
                // Optionally rethrow the exception if you want to handle it further up the call stack
                throw;
            }
        }

        /// <summary>
        /// Download File from provided location from FTP
        /// </summary>
        /// <param name="fileServer"></param>
        /// <param name="ftpFilePath"></param>
        /// <returns></returns>
        private async Task<string> DownloadFileAsync(FileServerModel fileServer, string ftpFilePath)
        {
            var host = fileServer.Host.Split(':');
            var ftpClient = new AsyncFtpClient($"{host[0]}:{host[1]}", fileServer.UserName, fileServer.Password, Convert.ToInt32(host[2]));

            // Enable detailed logging
            ftpClient.Config.LogToConsole = true;

            await ftpClient.AutoConnect();

            try
            {
                using var memoryStream = new MemoryStream();
                await ftpClient.DownloadStream(memoryStream, fileServer.SubFolder + '/' + ftpFilePath);
                memoryStream.Position = 0;

                using var reader = new StreamReader(memoryStream);
                var content = await reader.ReadToEndAsync();
                return content;
            }
            catch (FtpCommandException ex)
            {
                Console.WriteLine($"FTP command error: {ex.CompletionCode} - {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                throw;
            }
            finally
            {
                await ftpClient.Disconnect();
            }
        }

        /// <summary>
        /// Create PNG image file from Text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private async Task<Bitmap> CreateImageFromText(string text)
        {
            try
            {
                var font = new Font("Arial", 12);
                var textColor = Color.Black;
                var backColor = Color.White;

                var textSize =await MeasureText(text, font);
                var bitmap = new Bitmap(textSize.Width, textSize.Height);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(backColor);
                    graphics.DrawString(text, font, new SolidBrush(textColor), 0, 0);
                }
                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateImageFromText: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Measures the size of the specified text when rendered with the specified font.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <param name="font">The font to use for the measurement.</param>
        /// <returns>A Size object representing the width and height of the rendered text.</returns>
        private async Task<Size> MeasureText(string text, Font font)
        {
            try
            {
                using (var bitmap = new Bitmap(1, 1))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        var sizeF = graphics.MeasureString(text, font);
                        return new Size((int)sizeF.Width, (int)sizeF.Height);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MeasureText: {ex.Message}");
                throw;
            }
        }

    }
}
