USE [KowToMateERP]
GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Menu_GetAll]    Script Date: 5/12/2022 5:46:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================
CREATE PROCEDURE [dbo].[SP_Security_Menu_GetListWithDetails]
AS
BEGIN  
SELECT m.[Id]
      ,m.[Name]
	  ,p.[Name] ParentMenuName
      ,m.[Icon]     
      ,m.[MenuUrl]
      ,m.[Status]
      ,m.[CreatedDate]     
      ,m.[ObjectId]
  FROM [dbo].[Security_Menu] m WITH(NOLOCK)
  LEFT JOIN [dbo].[Security_Menu] p WITH(NOLOCK) on p.Id = m.ParentId
END


-- New
USE [KowToMateERP]
GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Menu_Insert]    Script Date: 5/12/2022 6:12:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

ALTER PROCEDURE [dbo].[SP_Security_Menu_Insert](
             
           @Name varchar(100),
           @ParentId int,
           @Icon varchar(50),
           @IsLeftMenu bit,
           @IsTopMenu bit,
           @IsExternalMenu bit,
           @MenuUrl varchar(150),
           @Status int,
           @CreatedByContactId int,
           @ObjectId varchar(40)
)
AS
BEGIN  
   INSERT INTO [dbo].[Security_Menu]
           ([Name]
           ,[ParentId]
           ,[Icon]
           ,[IsLeftMenu]
           ,[IsTopMenu]
           ,[IsExternalMenu]
           ,[MenuUrl]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[ObjectId]
		   ,[DisplayOrder]
		   )
     VALUES
           (
		   @Name,
           @ParentId, 
           @Icon, 
           @IsLeftMenu, 
           @IsTopMenu,
           @IsExternalMenu,
           @MenuUrl, 
           @Status, 
           SYSDATETIME(),  
           @CreatedByContactId, 
           @ObjectId
		   ,ISNULL((SELECT MAX(DisplayOrder) FROM [dbo].[Security_Menu]),0) + 1
		   )
END




