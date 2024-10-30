
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[HR_sp_Designation_Insert](
            @Name  nvarchar(100),
            @Status int,
			@ObjectId varchar(40),
            @CreatedByContactId int
)
AS
BEGIN  

    Insert Into  [dbo].[HR_Designation] 
	(
		Name,
		Status,
		CreatedDate,
		CreatedByContactId,
		ObjectId
	)

	Values
	(
	    @Name,
		@Status,
		SYSDATETIME(),
		@CreatedByContactId, 
		@ObjectId
	)
  
END



