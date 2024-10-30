
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_Menu_Insert](
             
           @Name varchar(100),
           @ParentId int,
           @Icon varchar(50),
           @IsLeftMenu bit,
           @IsTopMenu bit,
           @IsExternalMenu bit,
           @MenuUrl varchar(150),
           @Status int,
           @CreatedDate datetime,
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
           ,[ObjectId])
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
           @CreatedDate, 
           @CreatedByContactId, 
           @ObjectId

		   )
END


