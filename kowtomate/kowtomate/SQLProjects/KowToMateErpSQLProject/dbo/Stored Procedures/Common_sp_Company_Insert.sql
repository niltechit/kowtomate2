
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Common_sp_Company_Insert](
             
            @Name  nvarchar(100),
            @Code  nvarchar(6),
            @CompanyType tinyint,
            @Telephone varchar(30),
            @Address1 varchar(100),
            @Address2 varchar(100),
            @City varchar(30),
            @State varchar(30),
            @Zipcode varchar(10),
            @Country varchar(50),
            @Status int,
            @CreatedByContactId int,
            @ObjectId varchar(40)
)
AS
BEGIN  
    INSERT INTO [dbo].[Common_Company]
           (       
		    Name,
            Code,
            CompanyType,
            Telephone,
            Address1,
            Address2,
            City,
            State,
            Zipcode,
            Country,
            Status,
			CreatedDate,
            CreatedByContactId, 
            ObjectId
           
           )
     VALUES
          (
		    @Name,
            @Code,
            @CompanyType,
            @Telephone,
            @Address1,
            @Address2,
            @City,
            @State,
            @Zipcode,
            @Country,
            @Status,
			SYSDATETIME(),
            @CreatedByContactId, 
            @ObjectId
          
		   )
END


