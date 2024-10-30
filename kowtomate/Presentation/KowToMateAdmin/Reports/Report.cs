using Blazor_PDF.PDF;
using CutOutWiz.Services.Models.SOP;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.JSInterop;

namespace KowToMateAdmin.Reports
{
    public class Report
    {
        private SopPdfGeneratorModel _pdfGeneratorModel;
        //private int _pagenumber;
        //private IPageImage _pageImage;

        //public void Generate(IJSRuntime js, SopPdfGeneratorModel pdfGeneratorModel, string filename = "report.pdf")
        //{
        //    //_pagenumber = pagenumber;
        //    _pdfGeneratorModel = pdfGeneratorModel;
        //    js.InvokeVoidAsync("jsDownloadFile",
        //                        filename,
        //                        ReportPDF()
        //                        );
        //}
        //private byte[] ReportPDF()
        //{
        //    var memoryStream = new MemoryStream();

        //    // Marge in centimeter, then I convert with 
        //    float margeLeft = 1.5f;
        //    float margeRight = 3.5f;
        //    float margeTop = 25.0f;
        //    float margeBottom = 25.0f;

        //    Document pdf = new Document(
        //                            PageSize.A4,
        //                            margeLeft,
        //                            margeRight,
        //                            margeTop,
        //                            margeBottom
        //                           );

        //    pdf.AddTitle("Blazor-PDF");
        //    pdf.AddAuthor("Christophe Peugnet");
        //    pdf.AddCreationDate();
        //    pdf.AddKeywords("blazor");
        //    pdf.AddSubject("Create a pdf file with iText");

        //    PdfWriter writer = PdfWriter.GetInstance(pdf, memoryStream);

        //    //HEADER and FOOTER
        //    var fontStyle = FontFactory.GetFont("Arial", 16,Font.BOLD, BaseColor.White);
        //    var sopTitle = $"Sop Title : {_pdfGeneratorModel.Name}";
        //    var labelHeader = new Chunk(sopTitle, fontStyle);
        //    HeaderFooter header = new HeaderFooter(new Phrase(labelHeader), false)
        //    {
        //        BackgroundColor = new BaseColor(133, 76, 199),
        //        Alignment = Element.ALIGN_CENTER,
        //        Border = Rectangle.NO_BORDER
        //    };
        //    //header.Border = Rectangle.NO_BORDER;
        //    pdf.Header = header;

        //    var labelFooter = new Chunk("Page", fontStyle);
        //    HeaderFooter footer = new HeaderFooter(new Phrase(labelFooter), true)
        //    {
        //        Border = Rectangle.NO_BORDER,
        //        Alignment = Element.ALIGN_RIGHT
        //    };
        //    pdf.Footer = footer;

        //    pdf.Open();

        //    //if (_pagenumber == 1)
        //   PageHeader.PageText(pdf, _pdfGeneratorModel);
        //  // PageImage.PageImagePrint(pdf,_pdfGeneratorModel.SopTemplateFileList, writer);
        //    //PageList.PageFonts(pdf, writer);
        //     //else if (_pagenumber == 2)
        //    //    Page2.PageBookmark(pdf);
        //    //else if (_pagenumber == 3)
        //    //    Page3.PageImage(pdf, writer);
        //    //else if (_pagenumber == 4)
        //    //    Page4.PageTable(pdf, writer);
        //    //else if (_pagenumber == 5)
        //    //    Page5.PageFonts(pdf, writer);
        //    //else if (_pagenumber == 6)
        //    //    Page6.PageList(pdf);
        //    //else if (_pagenumber == 7)
        //    //    page7.PageShapes(pdf, writer);

        //    pdf.Close();

        //    return memoryStream.ToArray();
        //}

    }
}
