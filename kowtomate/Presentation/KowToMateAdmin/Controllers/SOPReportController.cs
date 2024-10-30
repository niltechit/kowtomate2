using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KowToMateAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SOPReportController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        public SOPReportController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }
        [HttpGet("Index")]
        [Route("Index/{Id}")]
        public IActionResult Index(int Id)
        {
            WebReport web = new WebReport();
            // Load the Fast Report
            var path = $"{this._webHostEnvironment.WebRootPath}\\Report\\SOP_Template_Report.frx";
            web.Report.Load(path);

            // Passing ConnectionString In Fast Report
            var mssqlDataConnection = new MsSqlDataConnection();
            mssqlDataConnection.ConnectionString = _configuration.GetConnectionString("Default");
            var Conn = mssqlDataConnection.ConnectionString;
            web.Report.SetParameterValue("CONN", Conn);
            web.Report.SetParameterValue("TemplateId",Id);
            Dictionary<string, string> parameterValueList = new Dictionary<string, string>();
            parameterValueList.Add("CONN", Conn.ToString());
            parameterValueList.Add("TemplateId", Id.ToString());

            //Render The Report Pdf
            // prepare report
            web.Report.Prepare();
            //// save file in stream
            Stream stream = new MemoryStream();
            web.Report.Export(new PDFSimpleExport(), stream);
            stream.Position = 0;
            //// return stream in browser
            return File(stream, "application/zip", "report.pdf");
            //return web;
        }
        [HttpGet("SOPReport")]
        [Route("SOPReport/{url}")]
        public Task SOPReport(string url)
        {
            //url = "https://localhost:7073/sop/templates/Details/db35314b-71dc-4346-8e0e-dde46ccf6170";
            //SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
            //SelectPdf.PdfDocument doc = converter.ConvertUrl(url);
            //doc.Save("test.pdf");
            //doc.Close();
            SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
            SelectPdf.PdfDocument doc = converter.ConvertUrl("https://localhost:7073/sop/templates/Details/db35314b-71dc-4346-8e0e-dde46ccf6170");
            //SelectPdf.PdfDocument doc = converter.ConvertHtmlString(HTMLContent.ToString());
            doc.Save("test.pdf");
            return Task.CompletedTask;
        }

    }
}

