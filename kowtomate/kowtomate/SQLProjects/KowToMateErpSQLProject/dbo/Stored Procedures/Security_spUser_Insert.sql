
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save User info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_spUser_Insert](
    @CompanyId AS INT,
    @ContactId AS INT,
	@RoleId AS INT,
	@Username AS NVARCHAR(100),
	@PasswordHash AS NVARCHAR(100),
	@PasswordSalt AS NVARCHAR(100),
	@UserGuid AS VARCHAR(100),
	@Status as int
)
AS
BEGIN  
           INSERT INTO [dbo].[Security_User]
           ([CompanyId]
		   ,[ContactId]
           ,[RoleId]
           ,[Username]
           ,[PasswordHash]
           ,[PasswordSalt]
           ,[ObjectId]
		   ,[Status]
		   ,[CreatedDate]
          )
     VALUES
           (@CompanyId
		   ,@ContactId
           ,@RoleId
           ,@Username
           ,@PasswordHash
           ,@PasswordSalt
           ,@UserGuid
		   ,@Status
		   , SYSDATETIME()
          )
END




