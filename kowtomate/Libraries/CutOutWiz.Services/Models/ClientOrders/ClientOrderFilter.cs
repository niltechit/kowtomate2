
using CutOutWiz.Core;

namespace CutOutWiz.Services.Models.ClientOrders
{
    public class ClientOrderFilter : BaseSearchFilter
    {
        public string Where { get; set; }
        public int TotalImageCount { get; set; }
        public string JoinClouse { get;set; }
    }
}
