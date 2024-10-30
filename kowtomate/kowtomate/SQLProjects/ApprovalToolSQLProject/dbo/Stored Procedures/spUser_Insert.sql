-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save User info 
-- =============================================

CREATE PROCEDURE [dbo].[spUser_Insert](
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
           INSERT INTO [dbo].[User]
           ([ContactId]
           ,[RoleId]
           ,[Username]
           ,[PasswordHash]
           ,[PasswordSalt]
           ,[UserGuid]
		   ,[Status]
		   ,[CreatedDateUtc]
          )
     VALUES
           (@ContactId
           ,@RoleId
           ,@Username
           ,@PasswordHash
           ,@PasswordSalt
           ,@UserGuid
		   ,@Status
		   , SYSDATETIME()
          )
END