using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Services.Models.SOP
{
    public class SOPTemplateFile
    {
        public int Id { get; set; }

       public int SOPTemplateId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }

        public string ActualPath { get; set; }
        public string  ModifiedPath { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByContactId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedByContactId { get; set; }

        public int? FileModifiedByContactId { get; set; }   

        public string ObjectId { get; set; }
        public string RootFolderPath { get; set; }
        public string ViewPath { get; set; }
        public string FileByteString { get; set; }

        public IBrowserFile File { get; set; }

    }
}
