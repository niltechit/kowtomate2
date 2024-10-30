using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Core.Utilities
{
    public class Enums
    {       
        public enum GeneralStatus
        {
            Active = 1,
            Inactive = 2
        }
        
        public enum FileServerType
        {
            GCP = 1,
            S3 = 2,
            FTP = 3,
        }

        public enum CompanyType
        {
            //System = 1,
            //Operation = 2,
            //Client = 3
            Admin = 1,
            Client = 2,
            System = 3,
        }

    }
}
