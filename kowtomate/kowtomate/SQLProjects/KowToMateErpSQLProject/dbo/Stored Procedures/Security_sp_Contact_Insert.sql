

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_Contact_Insert](
             
			@CompanyId int,
			@FirstName nvarchar(100),
			@LastName nvarchar(100),
			@Designation int,
            @Email nvarchar(100),
            @Phone varchar(20),
            @ProfileImageUrl varchar(200),
            @Status int,
            @CreatedByContactId int,
            @ObjectId varchar(40)
)
AS
BEGIN  
  INSERT INTO [dbo].[Security_Contact]
           ([CompanyId]
           ,[FirstName]
           ,[LastName]
           ,[Designation]
           ,[Email]
           ,[Phone]
           ,[ProfileImageUrl]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[ObjectId])
     VALUES
           (
		   @CompanyId,
           @FirstName,
           @LastName, 
           @Designation, 
           @Email, 
           @Phone, 
           @ProfileImageUrl, 
           @Status, 
           SYSDATETIME(), 
           @CreatedByContactId,
           @ObjectId

		   )
END


