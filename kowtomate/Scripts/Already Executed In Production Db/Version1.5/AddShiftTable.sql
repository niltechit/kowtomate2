CREATE TABLE Common_Shift (
   Id bigint IDENTITY(1,1) PRIMARY KEY,
   StartTime nvarchar(20) NOT NULL,
   EndTime nvarchar(20) NOT NULL,
   Name VARCHAR(255) NOT NULL,
   Code VARCHAR(50) NOT NULL
);

CREATE TABLE Common_Contact_Shift (
   Id BIGINT IDENTITY(1,1) PRIMARY KEY,
   ContactId int NOT NULL,
   ShiftId BIGINT NOT NULL,
   CreatedDate DATETIME NOT NULL,
   CreatedByContactId int NOT NULL,
   FOREIGN KEY (ContactId) REFERENCES Security_Contact(Id),
   FOREIGN KEY (ShiftId) REFERENCES Common_Shift(Id)
);



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Shift_Insert]    Script Date: 4/11/2023 5:54:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Rakib
-- Create date: 11 April 2023
-- Description:	Save Shift info 
-- =============================================

Create PROCEDURE [dbo].[SP_Common_Shift_Insert](
       @StartTime nvarchar(20),
       @EndTime nvarchar(20),
	   @Name varchar(250),
       @Code varchar(50)
)
AS
BEGIN  
	INSERT INTO [dbo].[Common_Shift]
           ([Name]
           ,[Code]
           ,[StartTime]
           ,[EndTime]
          )
     VALUES
           (
		   @Name,
           @Code,
           @StartTime,
           @EndTime
		   )
	 SELECT SCOPE_IDENTITY();
END



GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Shift_GetAll]    Script Date: 4/12/2023 11:36:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rakib
-- Create date: 12 April 2023
-- Description:	Get Company info 
-- =============================================
Create PROCEDURE [dbo].[SP_Common_Shift_GetAll]
AS
BEGIN  
	SELECT  *
  FROM [dbo].[Common_Shift] WITH(NOLOCK) ORDER BY Id desc
END


GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_GetAssignOrderItemByContactId]    Script Date: 4/14/2023 12:43:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Author:		Rakib
-- Create date:4/14/2023
-- Description:	Get An Editor all assigned image which exist in inproduction,distribute, reworkinproduction, reworkdistributation

-- =============================================
Create PROCEDURE [dbo].[SP_Order_ClientOrderItem_GetAssignOrderItemByContactId]
	@ContactId int
AS
BEGIN

	SELECT COUNT(ci.Id) as TotalPrductionOngoingItem
	FROM [dbo].[Order_ClientOrderItem] as ci inner join dbo.Order_AssignedImageEditor as aie on ci.Id = aie.Order_ImageId where AssignContactId = @ContactId and Status in (7,8,11,12)
	

END





