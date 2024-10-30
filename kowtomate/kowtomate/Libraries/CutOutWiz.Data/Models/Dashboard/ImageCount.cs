using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Data.Models.Dashboard
{
    public class ImageCount
    {
        public int Raw { get; set; }
        public int Completed { get; set; }
        public int Pending { get; set; }
        public int Rejected { get; set; }
    }
}
