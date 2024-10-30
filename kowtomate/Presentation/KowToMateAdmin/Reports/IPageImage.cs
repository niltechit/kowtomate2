using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Blazor_PDF.PDF
{
    public interface IPageImage
    {
        void PageImagePrint(Document pdf, PdfWriter writer);
    }
}