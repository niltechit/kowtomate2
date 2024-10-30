using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutOutWiz.Core.Utilities
{
    public static class StatusConstants
    {
        public const int Active = 1;
        public const int InActive = 2;
        public const int Deleted = 3;
    }

    public static class ApprovalToolActionTypeConstants
    {
        public const string Accepted = "Accepted";
        public const string Rejected = "Rejected";
    }

    public static class ApprovalToolImageFoldersConstants
    {
        public const string ToProcess = "To Process";
        public const string ForApproval = "For Approval";
        public const string ForRework = "For Rework";
        public const string Completed = "Completed";
    }

    public static class ClaimTypesConstants
    {
        public const string UserId = "UserId";
        public const string ContactId = "ContactId";
        public const string CompanyId = "CompanyId";
        public const string CompanyObjectId = "CompanyObjectId";
        public const string ClientRootFolderPath = "ClientRootFolderPath";
    }

    public static class SourceDriveConstants
    {
        public const string AmazonS3 = "Amazon S3";
    }
}
