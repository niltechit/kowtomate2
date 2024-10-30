-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save User info 
-- =============================================

CREATE PROCEDURE [dbo].[spContact_Insert](
    @CompanyId AS INT,
	@FirstName AS NVARCHAR(100),
	@LastName AS NVARCHAR(100),
	@Email AS NVARCHAR(100),
	@Phone AS NVARCHAR(100),
	@ContactGuid AS VARCHAR(100)
)
AS
BEGIN
	  BEGIN TRY
          INSERT INTO [dbo].[Contact]
           ([CompanyId]
           ,[FirstName]
           ,[LastName]
           ,[Email]
           ,[Phone]
           ,[ContactGuid]
           ,[CreatedDateUtc]
		   )
         
     VALUES
           (@CompanyId,
            @FirstName, 
            @LastName,
            @Email,
            @Phone,
            @ContactGuid,
            SYSDATETIME())
			Select SCOPE_IDENTITY()

	  END TRY
	  BEGIN CATCH
	  END CATCH
END