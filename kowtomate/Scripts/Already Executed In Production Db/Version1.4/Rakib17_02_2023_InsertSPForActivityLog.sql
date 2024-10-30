
GO
/****** Object:  StoredProcedure [dbo].[SP_ActivityLog_Insert]    Script Date: 2/17/2023 11:55:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		
-- Create date: 8 Aug 2022
-- Description:	Save ActivityLog info 
-- =============================================

ALTER PROCEDURE [dbo].[SP_ActivityLog_Insert](
            
            @ActivityLogFor tinyint,
            @PrimaryId  int,
            @Description varchar(500),
            @CreatedByContactId int,
			@ObjectId varchar(504),
			@CompanyObjectId varchar(40),
			@ContactObjectId varchar(40),
			@Category int,
			@Type int
)
AS
BEGIN  
    INSERT INTO [dbo].[Common_ActivityLog]
           (
			ActivityLogFor,       
		    PrimaryId,
			Description,
			CreatedDate,
			CreatedByContactId,
			ObjectId,
			CompanyObjectId,
			ContactObjectId,
			Category,
			Type
           )
     VALUES
          (
			 @ActivityLogFor ,
            @PrimaryId  ,
            @Description ,
			SYSDATETIME(),
            @CreatedByContactId, 
            @ObjectId,
			@CompanyObjectId,
			@ContactObjectId,
			@Category,
			@Type
		   )

	SELECT SCOPE_IDENTITY();
END