  alter table [dbo].[Security_Contact] add  isSharedFolderEnable bit


--GO
--/****** Object:  StoredProcedure [dbo].[SP_User_UpdateUserUploadFolderPath]    Script Date: 3/29/2023 2:03:11 PM ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO

---- =============================================
---- Author:		Rakib
---- Create date: 29 March 2023
---- Description:	Update Contact Upload Folder Path
---- =============================================

--Create PROCEDURE [dbo].[SP_User_UpdateUserUploadFolderPath](
--       @UploadFolderPath varchar(500),
--	   @Id int
	  
--)
--AS
--BEGIN  
--  UPDATE [dbo].[Security_Contact]    
--   SET 
--	   [UploadFolderPath] = @UploadFolderPath
	 
--       WHERE Id = @Id
--END


GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetById]    Script Date: 3/29/2023 1:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

ALTER PROCEDURE [dbo].[SP_Security_Contact_GetById]
@ContactId int
AS
BEGIN  
	SELECT *
  FROM [dbo].[Security_Contact] Where Id= @ContactId
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetAll]    Script Date: 3/29/2023 2:40:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================
ALTER PROCEDURE [dbo].[SP_Security_Contact_GetAll]
AS
BEGIN  
	SELECT *
  FROM [dbo].[Security_Contact]
END









GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrder_GetByOrderNumber]    Script Date: 3/29/2023 5:16:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[SP_Order_ClientOrder_GetByOrderNumber]
	@orderNumber nvarchar(100)
as 
begin 
	select * from Order_ClientOrder where OrderNumber = @orderNumber
end







GO
/****** Object:  StoredProcedure [dbo].[SP_User_UpdateUserDownloadFolderPath]    Script Date: 3/29/2023 6:29:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Rakib
-- Create date: 1 Jan 2023
-- Description:	Update Contact Folder Path
-- =============================================

ALTER PROCEDURE [dbo].[SP_User_UpdateUserDownloadFolderPath](
       @DownloadFolderPath varchar(500),
	   @Id int,
	   @IsUserActive bit,
	   @isSharedFolderEnable bit
)
AS
BEGIN  
  UPDATE [dbo].[Security_Contact]    
   SET 
	   [DownloadFolderPath] = @DownloadFolderPath,
	   IsUserActive=@IsUserActive,
	   isSharedFolderEnable = @isSharedFolderEnable
       WHERE Id = @Id
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Order_AssignedImageEditor_GetByOrder_ImageId]    Script Date: 3/30/2023 10:53:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



Create PROCEDURE [dbo].[SP_Order_AssignedImageEditor_GetByOrder_ImageId]
	  @OrderId int,
	  @AssignContactId int,
	  @Order_ImageId int
AS
BEGIN  

	Select * from dbo.Order_AssignedImageEditor where AssignContactId = @AssignContactId and OrderId = @Order_ImageId and Order_ImageId= @Order_ImageId

END

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetAllIsSharedFolderEditorContact]    Script Date: 3/31/2023 1:04:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	 Rakib	
-- Create date: 29 March 2023
-- Description: SP_Security_Contact_GetAll
-- =============================================
Create PROCEDURE [dbo].[SP_Security_Contact_GetAllIsSharedFolderQcContact]
AS
BEGIN  
  SET NOCOUNT ON;
  DECLARE @EditorRoleObjectId varchar (100)
  select @EditorRoleObjectId= ObjectId from Security_Role where Name = 'Qc'
  SELECT *
  FROM [dbo].[Security_Contact] where isSharedFolderEnable = 1 and Id in (  select ContactId from Security_User as su inner join Security_UserRole as sur on sur.UserObjectId=su.ObjectId where sur.RoleObjectId= @EditorRoleObjectId)
END


GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetAllIsSharedFolderEditorContact]    Script Date: 3/31/2023 1:07:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		
-- Create date: 29 March 2023
-- Description: SP_Security_Contact_GetAll
-- =============================================
Create PROCEDURE [dbo].[SP_Security_Contact_GetAllIsSharedFolderEditorContact]
AS
BEGIN  
  SET NOCOUNT ON;
  DECLARE @EditorRoleObjectId varchar (100)
  select @EditorRoleObjectId= ObjectId from Security_Role where Name = 'Editor'
  SELECT *
  FROM [dbo].[Security_Contact] where isSharedFolderEnable = 1 and Id in (  select ContactId from Security_User as su inner join Security_UserRole as sur on sur.UserObjectId=su.ObjectId where sur.RoleObjectId= @EditorRoleObjectId)
END
