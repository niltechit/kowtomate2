using CutOutWiz.Services.Models.SOP;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace KowToMateAdmin.Reports
{
    public class PageHeader
    {
        //private readonly static string _lopsem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas dictum felis ut turpis viverra, a ultrices nisi tempor. Aliquam suscipit dui sit amet facilisis aliquam. In scelerisque sem ut elit molestie tempor. In finibus sagittis nulla, vitae vestibulum ante tristique sit amet. Phasellus facilisis rhoncus nunc id scelerisque. Praesent cursus erat nec turpis interdum condimentum. Aenean ut facilisis eros. Nam semper tincidunt libero in porttitor. Praesent nec dui vitae leo vulputate varius ut non risus. Quisque imperdiet euismod ipsum facilisis finibus. Duis ac felis eget leo malesuada gravida id at felis. Cras posuere, tortor sit amet bibendum tincidunt, augue lectus pulvinar nisl, ac blandit velit arcu sed nulla. Mauris id venenatis turpis, ut fringilla nunc. Aenean commodo fermentum nulla, non porta sapien viverra sed. Sed sed risus interdum, maximus sapien ac, bibendum diam.";

        //public static void PageText(Document pdf,SopPdfGeneratorModel pdfGeneratorModel)
        //{
        //    var instructionTitle = new Paragraph(pdfGeneratorModel.HeaderForInstruction, new Font(Font.HELVETICA, 18, Font.BOLD | Font.UNDERLINE));
        //    instructionTitle.SpacingAfter = 4f;
        //    instructionTitle.IndentationLeft = 10;
        //    pdf.Add(instructionTitle);
        //    var instruction = new Paragraph(pdfGeneratorModel.Instruction, new Font(Font.HELVETICA, 15, Font.NORMAL));
        //    instruction.IndentationLeft = 12;
        //    pdf.Add(instruction);

        //    var title = new Paragraph(pdfGeneratorModel.HeaderForService, new Font(Font.HELVETICA,18,Font.BOLD | Font.UNDERLINE));
        //    title.SpacingAfter = 4f;
        //    title.IndentationLeft = 10;
        //    pdf.Add(title);
        //    //Font _fontStyle = FontFactory.GetFont("Tahoma", 8f, Font.ITALIC);
        //    //string _lopsem = "";
        //    //var phrase = new Phrase(_lopsem, _fontStyle);
        //    //pdf.Add(phrase);

        //    foreach (var seletedService in pdfGeneratorModel.SopStandardServiceList)
        //    {
        //        var service = new Paragraph(seletedService.Name, new Font(Font.HELVETICA,15, Font.NORMAL));
        //        service.IndentationLeft = 12;
        //        instructionTitle.SpacingAfter = 4f;
        //        pdf.Add(service);
        //    }
           
        //    // Create and add a Paragraph
        //    //var p = new Paragraph("Paragraph On the Right", _fontStyle);
        //    //p.SpacingBefore = 20f;
        //    //p.SetAlignment("RIGHT");

        //    //pdf.Add(p);

        //    float margeborder = 1.5f;
        //    float widhtColumn = 8.5f;
        //    float space = 1.0f;

        //    MultiColumnText columns = new MultiColumnText();
        //    columns.AddSimpleColumn(margeborder,
        //                            pdf.PageSize.Width - margeborder- space - widhtColumn);
        //    columns.AddSimpleColumn(margeborder + widhtColumn + space,
        //                            pdf.PageSize.Width - margeborder);

        //    //Paragraph para = new Paragraph(_lopsem, new Font(Font.HELVETICA, 8f));
        //    //para.SpacingAfter = 9f;
        //    //para.Alignment = Element.ALIGN_JUSTIFIED;
        //    //columns.AddElement(para);

        //    //pdf.Add(columns);

        //    var attachtmentTitle = new Paragraph("Attachment", new Font(Font.HELVETICA, 18, Font.BOLD | Font.UNDERLINE));
        //    attachtmentTitle.SpacingAfter = 4f;
        //    attachtmentTitle.IndentationLeft = 10;
        //    pdf.Add(attachtmentTitle);

        //    var y = 350;
        //    foreach (var file in pdfGeneratorModel.SopTemplateFileList)
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
        //            //img.SetAbsolutePosition(x, y);
        //            img.SpacingAfter = 8f;
        //            pdf.Add(img);
        //            y = y - 210;

        //        }
        //        else
        //        {
        //            //img.SetAbsolutePosition(x, y);
        //            img.SpacingAfter = 8f;
        //            pdf.Add(img);
        //            y = y - 210;

        //        }
        //        //pdf.NewPage();
        //    }
        //    //PdfContentByte cb = writer.DirectContent;
        //    //cb.SetLineWidth(3f);
        //    //cb.MoveTo(50, 20);
        //    //cb.LineTo(20, 80);
        //}




    }
}
