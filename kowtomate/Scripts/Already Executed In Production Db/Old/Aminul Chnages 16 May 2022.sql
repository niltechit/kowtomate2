USE [KowToMateERP]
GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_Delete]    Script Date: 5/16/2022 4:12:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Delete Company info 
-- =============================================
ALTER PROCEDURE [dbo].[SP_Security_Contact_Delete](
@ObjectId  varchar(40)        
)
AS
BEGIN  
    DELETE FROM  [dbo].[Security_Contact] WHERE ObjectId = @ObjectId
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetById]    Script Date: 5/16/2022 4:07:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

ALTER PROCEDURE [dbo].[SP_Security_Contact_GetById]
@ContactId int
AS
BEGIN  
	SELECT [Id]
      ,[CompanyId]
      ,[FirstName]
      ,[LastName]
      ,[DesignationId]
      ,[Email]
      ,[Phone]
      ,[ProfileImageUrl]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Contact] Where Id= @ContactId
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetAll]    Script Date: 5/16/2022 4:07:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================
ALTER PROCEDURE [dbo].[SP_Security_Contact_GetAll]
AS
BEGIN  
	SELECT [Id]
      ,[CompanyId]
      ,[FirstName]
      ,[LastName]
      ,[DesignationId]
      ,[Email]
      ,[Phone]
      ,[ProfileImageUrl]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Contact]
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetListWithDetails]    Script Date: 5/16/2022 4:07:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Aminul Islam
-- Create date: 16 May 2022
-- Description:	Get company name
-- =============================================
CREATE PROCEDURE [dbo].[SP_Security_Contact_GetListWithDetails]
AS
BEGIN  
	SELECT c.[Id]     
	   ,com.[Name] CompanyName
      ,c.[FirstName]
      ,c.[LastName]
	  ,d.[Name] DesignationName
      ,c.[Email]
      ,c.[Phone]
      ,c.[Status]
      ,c.[CreatedDate]
      ,c.[CreatedByContactId]
      ,c.[ObjectId]
  FROM [dbo].[Security_Contact] c WITH(NOLOCK)
  INNER JOIN [dbo].Common_Company com on com.Id = c.CompanyId
  LEFT JOIN [dbo].[HR_Designation] d on d.Id = c.DesignationId  
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_GetById]    Script Date: 5/16/2022 4:07:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Md Aminul Islam
-- Create date: 16 May 2022
-- Description: Get Contact Details
-- =============================================
CREATE PROCEDURE [dbo].[SP_Security_Contact_GetByObjectId]
@ObjectId varchar(40)
AS
BEGIN  
	SELECT [Id]
      ,[CompanyId]
      ,[FirstName]
      ,[LastName]
      ,[DesignationId]
      ,[Email]
      ,[Phone]
      ,[ProfileImageUrl]
      ,[Status]
      ,[CreatedDate]
      ,[CreatedByContactId]
      ,[UpdatedDate]
      ,[UpdatedByContactId]
      ,[ObjectId]
  FROM [dbo].[Security_Contact] Where ObjectId= @ObjectId
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Security_Contact_Update]    Script Date: 5/16/2022 4:48:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================
ALTER PROCEDURE [dbo].[SP_Security_Contact_Update](
    @ObjectId nvarchar(40),
	@FirstName nvarchar(100),
	@LastName nvarchar(100),
	@DesignationId int,
    @Email nvarchar(100),
    @Phone varchar(20),
    @Status int,
    @UpdatedByContactId int           
)
AS
BEGIN  
	UPDATE [dbo].[Security_Contact]
	SET 
		[FirstName] = @FirstName
		,[LastName] = @LastName
		,[DesignationId] = @DesignationId
		,[Email] = @Email
		,[Phone] = @Phone
		,[Status] = @Status
		,[UpdatedDate] = SYSDATETIME()
		,[UpdatedByContactId] = @UpdatedByContactId      
    WHERE ObjectId = @ObjectId
END

USE [KowToMateERP]
GO
/****** Object:  StoredProcedure [dbo].[SP_Common_Company_GetAll]    Script Date: 5/16/2022 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_Common_Company_GetAll]
AS
BEGIN  
	SELECT 
	        Id,
			Name, 
			Code,
			CompanyType, 
			ISNULL(Telephone, '') Telephone, 
			ISNULL(Email, '') Email,
			Address1, 
			Address2, 
			City, 
			State, 
			Zipcode,
			Country,
			Status,
			CreatedDate,
			CreatedByContactId,
			UpdatedDate,
			UpdatedByContactId,
			ObjectId
    FROM [dbo].[Common_Company]
END