﻿namespace KowToMateAdmin.Models.Security
{
    public class LoginUserInfoViewModel
    {
        public int UserId { get; set; }
        public string UserObjectId { get; set; }
        public int ContactId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyObjectId { get; set; }
        //public string RoleName { get; set; }

        //public LoginUserInfoViewModel GetDemoData()
        //{
        //    return new LoginUserInfoViewModel
        //    {
        //        UserId = 1,
        //        //UserObjectId = "6075b64f-f4da-4617-ad41-074f03f196b5",
        //        CompanyId = 8,
        //        //RoleName = "",
        //        ContactId = 1,
        //    };
        //}
    }
}
