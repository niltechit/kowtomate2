using CutOutWiz.Services.Models.SOP;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Blazor_PDF.PDF
{
    public class PageImage 
    {
        //private IWebHostEnvironment _webHostEnvironment;
        //public PageImage(IWebHostEnvironment webHostEnvironment)
        //{
        //    _webHostEnvironment = webHostEnvironment;
        //}

        //public static void PageImagePrint(Document pdf,List<SOPTemplateFile> fileList, PdfWriter writer)
        //{
        //    var attachtmentTitle = new Paragraph("Attachment", new Font(Font.HELVETICA, 18, Font.BOLD | Font.UNDERLINE));
        //    attachtmentTitle.SpacingAfter = 4f;
        //    attachtmentTitle.IndentationLeft = 10;
        //    pdf.Add(attachtmentTitle);
        //    var x = 40;
        //    var y = 400;
        //    foreach (var file in fileList)
        //    {
               
        //        var path = $@"\{file.RootFolderPath}\{file.FileName}";
        //        //string image = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\images\Logo.png"}";
        //        string image = $"{Directory.GetCurrentDirectory()}{path}";
                
        //        Image img = Image.GetInstance(image);

        //        img.ScaleAbsolute(400, 200);
        //        //img.SetAbsolutePosition(
        //        //        (PageSize.A4.Width - img.ScaledWidth) / 2,
        //        //        (PageSize.A4.Height - img.ScaledHeight) / 2);
        //        if (y <= 0)
        //        {
                    
        //            pdf.NewPage();
        //            y = 600;
        //            img.SetAbsolutePosition(x, y);
        //            img.SpacingAfter = 8f;
        //            pdf.Add(img);
        //            y = y - 210;

        //        }
        //        else
        //        {
        //            img.SetAbsolutePosition(x, y);
        //            img.SpacingAfter = 8f;
        //            pdf.Add(img);
        //            y = y - 210;

        //        }
               
                
        //        //pdf.NewPage();
        //    }
        //     //PdfContentByte cb = writer.DirectContent;
        //    //cb.SetLineWidth(3f);
        //    //cb.MoveTo(50, 20);
        //    //cb.LineTo(20, 80);
        //}
    }
}
