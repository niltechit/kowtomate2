USE [KowToMateERP_Dev]
GO

/****** Object:  Table [dbo].[Security_LoginHistory]    Script Date: 6/12/2023 2:01:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Security_LoginHistory](
	[Id] [int] IDENTITY(1,1) primary key NOT NULL,
	[UserId] [int] NULL,
	[Username] [varchar](50) NOT NULL,
	[ActionTime] [datetime] NOT NULL,
	[ActionType] [bit] NULL,
	[IPAddress] [varchar](50) NULL,
	[DeviceInfo] [varchar](50) NULL,
	[Status] [bit] NULL,
)


-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: 09-06-2023
-- Description:	Get all login histories.
-- =============================================
ALTER PROCEDURE [dbo].[SP_Security_GetSecurityLoginHistories]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	 SELECT Id, UserId, Username, ActionTime, 

	CASE WHEN  ActionType=1 THEN 'Login' ELSE 'Logout' end as ActionType,
	 
	 IPAddress, DeviceInfo, Status
    FROM Security_LoginHistory order by ActionTime desc
END


-- =============================================
-- Author:		Md Zakir Hossain
-- Create date:09-06-2023
-- Description:	Insert Login and Logout History
-- =============================================
ALTER PROCEDURE [dbo].[SP_Security_InsertSecurityLoginHistory] 
	-- Add the parameters for the stored procedure here
	@UserId INT,
	@Username VARCHAR(50),
    @ActionTime DATETIME,
    @ActionType BIT,
    @IPAddress VARCHAR(50),
    @DeviceInfo VARCHAR(50),
    @Status BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO Security_LoginHistory 
	(
		UserId, 
		Username, 
		ActionTime, 
		ActionType, 
		IPAddress, 
		DeviceInfo, 
		Status
	)
    VALUES 
	(
		@UserId, 
		@Username, 
		@ActionTime, 
		@ActionType, 
		@IPAddress, 
		@DeviceInfo, 
		@Status
	)
	SELECT SCOPE_IDENTITY()
END


CREATE PROCEDURE [dbo].[SP_Security_UpdateSecurityLoginHistoryById]
	@Id INT,
	@UserId INT=NULL,
	@Username VARCHAR(50)=NULL,
	@ActionTime DATETIME=NULL,
	@ActionType BIT=NULL,
	@IPAddress VARCHAR(50)=NULL,
	@DeviceInfo VARCHAR(50)=NULL,
	@Status BIT=NULL
AS
BEGIN
	UPDATE [dbo].[Security_LoginHistory]
	SET
		[UserId] = ISNULL(@UserId, [UserId]),
		[Username] = ISNULL(@Username, [Username]),
		[ActionTime] = ISNULL(@ActionTime, [ActionTime]),
		[ActionType] = ISNULL(@ActionType, [ActionType]),
		[IPAddress] = ISNULL(@IPAddress, [IPAddress]),
		[DeviceInfo] = ISNULL(@DeviceInfo, [DeviceInfo]),
		[Status] = ISNULL(@Status, [Status])
	WHERE
		[Id] = @Id;
END

