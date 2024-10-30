
using CutOutWiz.Core;

namespace CutOutWiz.Services.Models.DynamicReports
{
    public class DynamicReportFilter : BaseSearchFilter
    {
        //public string Where { get; set; }

        public string SqlQuery { get; set; }

        public int? ClientCompanyId { get; set; }

        public byte? SqlType { get; set; }

        public string MainQuery { get; set; }
    }
}
