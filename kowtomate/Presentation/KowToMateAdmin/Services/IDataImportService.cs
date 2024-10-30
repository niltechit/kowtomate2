using CutOutWiz.Data.DynamicReports;

namespace KowToMateAdmin.Services
{
    public interface IDataImportService
    {
        //byte[] GenerateExcel(IEnumerable<IDictionary<string, object>> items, IDictionary<string, Type> columns, string sheetName);

        byte[] GenerateExcel(IEnumerable<IDictionary<string, object>> items, List<ReportTableColumn> columns, string sheetName);
    }
}