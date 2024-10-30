using CutOutWiz.Services.Models.Common;
using FluentFTP.Exceptions;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace CutOutWiz.Services.Helper
{
    public class PDFConverstionService : IPDFConverstionService
    {
        /// <summary>
        /// Convert Any type of file to PNG and Return Base64String
        /// </summary>
        /// <param name="fileServer"></param>
        /// <param name="ftpFilePath"></param>
        /// <returns>Base64String of image</returns>
        public async Task<string> ConvertPdfFileToPNG(FileServerModel fileServer, string ftpFilePath, string _webHostEnvironment)
        {
            try
            {
                // Download the file content from the FTP server
                var fileContent = await DownloadFileAsync(fileServer, ftpFilePath,_webHostEnvironment);
                if (fileContent == null)
                {
                    throw new Exception("Downloaded file content is null.");
                }

                // Create an image from the file content
                var image = await CreateImageFromText(fileContent);
                if (image == null)
                {
                    throw new Exception("Failed to create image from text.");
                }

                // Convert the image to a PNG and encode it as a base64 string
                using var memoryStream = new MemoryStream();
                image.Save(memoryStream, ImageFormat.Png);
                var convertedMemoryStream = memoryStream.ToArray();
                var convertBase64 = Convert.ToBase64String(convertedMemoryStream);

                // Get the file extension and construct the data URI
                var getExtension = System.IO.Path.GetExtension(ftpFilePath)?.Split(".");
                if (getExtension == null || getExtension.Length < 2)
                {
                    throw new Exception("Invalid file extension.");
                }
                var imageUri = $"data:image/{getExtension[1]};base64,{convertBase64}";

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
        private async Task<string> DownloadFileAsync(FileServerModel fileServer, string ftpFilePath, string _webHostEnvironment)
        {
            var host = fileServer.Host.Split(':');
            var ftpClient = new AsyncFtpClient($"{host[0]}:{host[1]}", fileServer.UserName, fileServer.Password, Convert.ToInt32(host[2]));

            // Enable detailed logging
            ftpClient.Config.LogToConsole = true;

            await ftpClient.AutoConnect();

            try
            {
                var destinationPath = $"{_webHostEnvironment}\\ConvertionFileSave\\{System.IO.Path.GetFileName(ftpFilePath)}";

                var download = await ftpClient.DownloadFile(destinationPath, fileServer.SubFolder + '/' + ftpFilePath);

                //string outPath = @"C:\Users\KOW\Desktop\autosave\ERKD-XMA\specification\Test pdf to view.txt";
                //int pagesToScan = 1;
                StringBuilder allText = new StringBuilder();
                string strText = string.Empty;
                try
                {
                    //destinationPath = @"D:\CutOutWiz Projects\kowtomate\Presentation\KowToMateAdmin\wwwroot\ConvertionFileSave\Test pdf to view.pdf";
                    PdfReader reader = new PdfReader(destinationPath);
                    // Get the total number of pages in the PDF
                    int pagesToScan = reader.NumberOfPages;
                    for (int page = 1; page <= pagesToScan; page++)
                    {
                        ITextExtractionStrategy its = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();

                        try
                        {
                            strText = PdfTextExtractor.GetTextFromPage(reader, page, its);
                            allText.Append(strText);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }

                        strText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(strText)));
                        string[] lines = strText.Split('\n');

                        // Converted text save in drive code
                        //foreach (string line in lines)
                        //{
                        //    using (System.IO.StreamWriter file = new System.IO.StreamWriter(outPath, true))
                        //    {
                        //        file.WriteLine(line);
                        //    }
                        //}
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
                return allText.ToString();
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

                var textSize = await MeasureText(text, font);
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
