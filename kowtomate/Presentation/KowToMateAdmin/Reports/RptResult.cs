

using CutOutWiz.Services.Models.SOP;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace KowToMateAdmin.Reports
{
	public class RptResult : PdfFooterPart
	{
        //#region Declaration 
        //PdfWriter _pdfWriter;
        //int _maxColumn = 8;
        //Document _document;
        //PdfPTable _pdfPTable = new PdfPTable(8);
        //PdfPCell _pdfCell;
        //Font _fontStyle;
        //MemoryStream _memoryStream = new MemoryStream();
        //SOPTemplateVM _sopTemplate = new SOPTemplateVM();
        //#endregion

        //public byte[] Report(SOPTemplateVM sopTemplate)
        //{ 
        //    _sopTemplate = sopTemplate;

        //    _document = new Document(PageSize.A4,10f,10f,10f,10f);
        //    _pdfPTable.WidthPercentage = 100;
        //    _pdfPTable.HorizontalAlignment = Element.ALIGN_LEFT;
        //    _fontStyle = FontFactory.GetFont("Tahoma",8f,1);
        //    _pdfWriter =PdfWriter.GetInstance(_document,_memoryStream);
        //    _pdfWriter.PageEvent = new PdfFooterPart();
        //    _document.Open();

        //    float[] sizes = new float[_maxColumn];
        //    for (int i = 0; i < _maxColumn; i++)
        //    {
        //        if (i == 0)
        //        {
        //            sizes[i] = 50;
        //        }
        //        else {
        //            sizes[i] = 100;
        //        }
        //    }
        //    _pdfPTable.SetWidths(sizes);
        //    this.ReportHeader();
        //    this.ReportBody();
        //    _pdfPTable.HeaderRows = 2;
        //    _document.Add(_pdfPTable);
        //    this.OnEndPage(_pdfWriter,_document);
        //    return _memoryStream.ToArray();
        //}
        //private void ReportHeader()
        //{
        //    _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1, BaseColor.Black);
        //    _pdfCell = new PdfPCell(new Phrase("SOP Information", _fontStyle));
        //    _pdfCell.Colspan = _maxColumn;
        //    _pdfCell.HorizontalAlignment=Element.ALIGN_CENTER;
        //    _pdfCell.Border = 0;
        //    _pdfCell.ExtraParagraphSpace = 0;
        //    _pdfPTable.AddCell(_pdfCell);
        //    _pdfPTable.CompleteRow();
        //}
        //private void ReportBody()
        //{ 
        //    _fontStyle = FontFactory.GetFont("Tahoma", 8f, 1, BaseColor.Black);
        //    var fontStyle = FontFactory.GetFont("Tahoma", 8f, 1, BaseColor.Black);
        //    #region Basic Information first Rows
        //    _pdfCell = new PdfPCell(new Phrase("Name : ", _fontStyle));
        //    _pdfCell.Colspan = 2;
        //    _pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    _pdfCell.Border = 0;
        //    _pdfCell.ExtraParagraphSpace = 0;
        //    _pdfPTable.AddCell(_pdfCell);

        //    _pdfCell = new PdfPCell(new Phrase(_sopTemplate.Name, _fontStyle));
        //    _pdfCell.Colspan = 2;
        //    _pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    _pdfCell.Border = 0;
        //    _pdfCell.ExtraParagraphSpace = 0;
        //    _pdfPTable.AddCell(_pdfCell);
        //    _pdfPTable.CompleteRow();
        //    #endregion

        //    #region Basic Information secound Rows
        //    _pdfCell = new PdfPCell(new Phrase("Instruction : ", _fontStyle));
        //    _pdfCell.Colspan = 2;
        //    _pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    _pdfCell.Border = 0;
        //    _pdfCell.ExtraParagraphSpace = 0;
        //    _pdfPTable.AddCell(_pdfCell);

        //    _pdfCell = new PdfPCell(new Phrase(_sopTemplate.Instruction, _fontStyle));
        //    _pdfCell.Colspan = 2;
        //    _pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    _pdfCell.Border = 0;
        //    _pdfCell.ExtraParagraphSpace = 0;
        //    _pdfPTable.AddCell(_pdfCell);
        //    _pdfPTable.CompleteRow();
        //    #endregion

        //    #region Attachment Information Third Rows
        //    _pdfCell = new PdfPCell(new Phrase("File Name : ", _fontStyle));
        //    _pdfCell.Colspan = 2;
        //    _pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    _pdfCell.Border = 0;
        //    _pdfCell.ExtraParagraphSpace = 0;
        //    _pdfPTable.AddCell(_pdfCell);
        //    _pdfPTable.CompleteRow();

        //    //_pdfCell = new PdfPCell(new Phrase(_sopTemplate.Instruction, _fontStyle));
        //    //_pdfCell.Colspan = 2;
        //    //_pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //    //_pdfCell.VerticalAlignment = Element.ALIGN_CENTER;
        //    //_pdfCell.BackgroundColor = BaseColor.White;
        //    //_pdfCell.ExtraParagraphSpace = 0;
        //    //_pdfPTable.AddCell(_pdfCell);
        //    #endregion
        //    #region Attachment body

        //    foreach (var item in _sopTemplate.SopTemplateFileList)
        //    {
        //        _pdfCell = new PdfPCell(new Phrase(item.FileName, _fontStyle));
        //        _pdfCell.Colspan = 2;
        //        _pdfCell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //        _pdfCell.Border = 0;
        //        _pdfCell.ExtraParagraphSpace = 0;
        //        _pdfPTable.AddCell(_pdfCell);
        //        _pdfPTable.CompleteRow();
        //    }
        //    #endregion
        //}
    }
}
