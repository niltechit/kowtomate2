using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Models.Common
{
    public class FileServerViewModel
    {
        public string Host { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
        public string SubFolder { get; set; }
        public int? Port { get; set; }   
    }
}
