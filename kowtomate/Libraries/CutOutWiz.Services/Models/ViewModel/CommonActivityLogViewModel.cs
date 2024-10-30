
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Core.Models.ViewModel
{
    public class CommonActivityLogViewModel
    {
        public int Id { get; set; }
        public byte ActivityLogFor { get; set; }
        public int PrimaryId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByContactId { get; set; }
        public string ObjectId { get; set; }
        public string ContactObjectId { get; set; }
        public string CompanyObjectId { get; set; }
        public byte Category { get; set; }
        public byte Type { get; set; }
        public LoginUserInfoViewModel loginUser { get; set; }
        public string RazorPage { get; set; }
        public string MethodName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
