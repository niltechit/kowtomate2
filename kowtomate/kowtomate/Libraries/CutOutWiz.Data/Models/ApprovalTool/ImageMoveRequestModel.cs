using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Data.Models.ApprovalTool
{
    public class ImageMoveRequestModel
    {
        public string? ActionType { get; set; }  //accepted, rejected
        public string? SelectedImages { get; set; }
        public string? Comments { get; set; }
        //public string 
    }
}
