
ALTER TABLE table_name
ADD SubFolder varchar(100);

ALTER PROCEDURE [dbo].[SP_Common_FileServer_Update](
	  @Id int,
	  @FileServerType tinyint,
      @Name varchar(100),
      @UserName varchar(100),
      @Password varchar(100),
      @AccessKey varchar(100),
      @SecretKey varchar(100),
      @RootFolder varchar(150),
      @SshKeyPath varchar(150),
      @Host varchar(150),
      @Protocol varchar(10),
      @Status tinyint,
      @UpdatedByContactId int,
	  @IsDefault bit,
	  @SubFolder varchar(100)
)
AS
BEGIN  

	if(@IsDefault = 'True')
	begin
		UPDATE dbo.Common_FileServer set IsDefault='False' where IsDefault ='True'
	end

    UPDATE [dbo].[Common_FileServer]    
    SET 
	   FileServerType = @FileServerType,
       Name = @Name,
       UserName = @UserName,
       Password = @Password,
       AccessKey = @AccessKey,
       SecretKey = @SecretKey,
       RootFolder = @RootFolder,
       SshKeyPath = @SshKeyPath,
       Host = @Host,
       Protocol = @Protocol,
       Status = @Status,
       UpdatedDate = SYSDATETIME(),
       UpdatedByContactId = @UpdatedByContactId,
	   IsDefault=@IsDefault,
	   SubFolder=@SubFolder
    WHERE Id = @Id
  
END


ALTER PROCEDURE [dbo].[SP_Common_FileServer_Insert](
	  @FileServerType tinyint,
      @Name varchar(100),
      @UserName varchar(100),
      @Password varchar(100),
      @AccessKey varchar(100),
      @SecretKey varchar(100),
      @RootFolder varchar(150),
      @SshKeyPath varchar(150),
      @Host varchar(150),
      @Protocol varchar(10),
      @Status tinyint,
      @CreatedByContactId int,
      @ObjectId varchar(40),
	  @IsDefault bit,
	  @SubFolder varchar(100)
)
AS
BEGIN  

	if(@IsDefault = 'True')
	begin
		UPDATE dbo.Common_FileServer set IsDefault='False' where IsDefault ='True' 
	end

    Insert Into  [dbo].[Common_FileServer] 
	(
		  FileServerType,
		  Name,
		  UserName,
		  Password,
		  AccessKey,
		  SecretKey,
		  RootFolder,
		  SshKeyPath,
		  Host,
		  Protocol,
		  Status,
		  CreatedDate,
		  CreatedByContactId,
		  ObjectId,
		  IsDefault,
		  SubFolder
	)

	Values
	(
		  @FileServerType,
		  @Name,
		  @UserName,
		  @Password,
		  @AccessKey,
		  @SecretKey,
		  @RootFolder,
		  @SshKeyPath,
		  @Host,
		  @Protocol,
		  @Status,
		  SYSDATETIME(),
		  @CreatedByContactId, 
		  @ObjectId,
		  @IsDefault,
		  @SubFolder
	)
  
	SELECT SCOPE_IDENTITY()

END



ALTER PROCEDURE [dbo].[SP_Common_FileServer_GetByObjectId]
@ObjectId varchar(40)
AS
BEGIN  

	SELECT Id, FileServerType, Name, UserName, Password, AccessKey, SecretKey,
				RootFolder, RootFolder, SshKeyPath, Host, Protocol,
				Status, CreatedDate, CreatedByContactId,
				UpdatedDate, UpdatedByContactId, ObjectId,IsDefault,SubFolder
	FROM [dbo].[Common_FileServer] WHERE [ObjectId] = @ObjectId

END




