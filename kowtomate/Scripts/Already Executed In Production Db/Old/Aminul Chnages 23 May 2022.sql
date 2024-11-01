USE [KowToMateERP]
GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Module_Insert]    Script Date: 5/23/2022 1:41:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Aminul Islam
-- Create date: 23 May 2022
-- Description: Add New Module 
-- =============================================

ALTER PROCEDURE [dbo].[SP_Security_Module_Insert]( 
           @Name varchar(100),
		   @Icon varchar(50),
           @Status tinyint,
           @CreatedByContactId int,
           @ObjectId varchar(40)
)
AS
BEGIN  

DECLARE @DisplayOrder DECIMAL(10,2) = 1.0

SET @DisplayOrder =(SELECT ISNULL(MAX(DisplayOrder),0) FROM [dbo].[Security_Module]) 

INSERT INTO [dbo].[Security_Module]
           ([Name]
		   ,[Icon]
		   ,[DisplayOrder]
           ,[Status]
           ,[CreatedDate]
           ,[CreatedByContactId]
           ,[ObjectId])
     VALUES
           (
		   @Name,
		   @Icon,
		   @DisplayOrder + 1,
           @Status, 
           SYSDATETIME(), 
           @CreatedByContactId, 
           @ObjectId
		   )
END

GO
/****** Object:  UserDefinedFunction [dbo].[fnSplit]    Script Date: 5/23/2022 3:32:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  UserDefinedFunction [dbo].[fnSplit]    Script Date: 05/14/2014 16:11:10 ******/
-- =============================================
-- Author:		Aminul
-- Create date: 23 May 2022
-- Description: Split values from delimiter separated string like '1,2,3'.
-- Execution: SELECT * FROM [dbo].[Split]('1,2,3', ',')
-- =============================================
CREATE FUNCTION [dbo].[fnSplit](@list varchar(8000), @delimiter char(1))       
RETURNS 
@tableList TABLE(
	value NVARCHAR(100)
	)
AS
BEGIN
	DECLARE @value    NVARCHAR(100)
	DECLARE @position INT

	SET @list = LTRIM(RTRIM(@list)) + ','
	SET @position = CHARINDEX(@delimiter, @list, 1)

	IF REPLACE(@list, @delimiter, '') <> ''
	BEGIN
		WHILE @position > 0
		BEGIN
			SET @value = LTRIM(RTRIM(LEFT(@list, @position - 1)))
			IF @value <> ''
			BEGIN
				INSERT INTO @tableList (value) 
				VALUES (@value)
			END
			SET @list = RIGHT(@list, LEN(@list) - @position)
			SET @position = CHARINDEX(@delimiter, @list, 1)

		END
	END	
	RETURN
END


GO

SET ANSI_PADDING ON
GO

/****** Object:  Index [IX_Security_Menu_ObjectId]    Script Date: 5/23/2022 3:46:14 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Security_Menu_ObjectId] ON [dbo].[Security_Menu]
(
	[ObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO



