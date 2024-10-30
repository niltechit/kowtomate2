using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Data.Models.Log
{
    public class LogModel
    {
        public string DateRange { get; set; }
        public string Username { get; set; }
        public List<string> Logs { get; set; } = new List<string>();
    }
}
