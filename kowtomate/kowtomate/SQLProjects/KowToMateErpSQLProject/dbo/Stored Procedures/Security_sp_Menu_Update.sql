

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Security_sp_Menu_Update](
           @Id  int,
           @Name varchar(100),
           @ParentId int,
           @Icon varchar(50),
           @IsLeftMenu bit,
           @IsTopMenu bit,
           @IsExternalMenu bit,
           @MenuUrl varchar(150),
           @Status int,
           @UpdatedByContactId int
)
AS
BEGIN  
   UPDATE [dbo].[Security_Menu]
   SET 
      [Name] = @Name, 
      [ParentId] = @ParentId,
      [Icon] = @Icon, 
      [IsLeftMenu] = @IsLeftMenu,
      [IsTopMenu] = @IsTopMenu, 
      [IsExternalMenu] = @IsExternalMenu,
      [MenuUrl] = @MenuUrl, 
      [Status] = @Status,
      [UpdatedDate] = SYSDATETIME(),
      [UpdatedByContactId] = @UpdatedByContactId
   
		
     WHERE Id = @Id
END



