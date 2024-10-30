
using CutOutWiz.Core;
using CutOutWiz.Services.Models.DynamicReports;

namespace CutOutWiz.Services.ImportExport
{
    public interface IDataImportExportService
    {
        public byte[] GenerateExcel<T>(string sheetName, List<T> items, List<string> exceptColumns);
        Response<List<T>> ImportExcel<T>(string path, string sheetName, out List<string> excelColumns);
        Task<byte[]> GenerateExcel(IEnumerable<IDictionary<string, object>> items, List<ReportTableColumnModel> columns, string sheetName, Action<int> progressCallback);
        Task<byte[]> GenerateExcel(IEnumerable<IDictionary<string, object>> items, List<ReportTableColumnModel> columns,
            string sheetName, Action<int> progressCallback, string groupingColumn);
    }
}