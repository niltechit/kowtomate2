using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Data.AwsModel
{
    public class AwsCredentialsModel
    {
        public string BucketName { get; set; }
        public string AccessKey { get; set; }
        public string SecretAccessKey { get; set; }
    }
}
