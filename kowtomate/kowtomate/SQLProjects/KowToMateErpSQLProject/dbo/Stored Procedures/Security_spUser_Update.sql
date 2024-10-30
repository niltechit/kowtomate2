

CREATE PROCEDURE [dbo].[Security_spUser_Update]	
    @UserId INT,
	@ContactId INT ,
    @FirstName NVARCHAR(150),
	@LastName NVARCHAR(150) ,
	@UserName NVARCHAR(100),
	@Email NVARCHAR(100) ,
	@CompanyId int ,
	@Phone NVARCHAR(20),
	@RoleId INT

AS
BEGIN

  UPDATE [dbo].[Security_User]   SET  [Username] = @UserName,   [RoleId] = @RoleId WHERE  Id = @UserId
  
  UPDATE Security_Contact SET CompanyId = @CompanyId,FirstName = @FirstName, LastName = @LastName,Email = ISNULL(@Email,''), Phone = @Phone WHERE Id = @ContactId

END

