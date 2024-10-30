
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_Contact_Update](
            @Id int,
            @CompanyId int,
			@FirstName nvarchar(100),
			@LastName nvarchar(100),
			@Designation int,
            @Email nvarchar(100),
            @Phone varchar(20),
            @ProfileImageUrl varchar(200),
            @Status int,
            @UpdatedByContactId int
           
)
AS
BEGIN  
		UPDATE [dbo].[Security_Contact]
		SET [CompanyId] = @CompanyId
			,[FirstName] = @FirstName
			,[LastName] = @LastName
			,[Designation] = @Designation
			,[Email] = @Email
			,[Phone] = @Phone
			,[ProfileImageUrl] = @ProfileImageUrl
			,[Status] = @Status
			,[UpdatedDate] = SYSDATETIME()
			,[UpdatedByContactId] = @UpdatedByContactId
      
        WHERE Id = @Id
END



