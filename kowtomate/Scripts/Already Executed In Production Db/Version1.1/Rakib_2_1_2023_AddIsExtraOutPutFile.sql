ALTER TABLE Order_ClientOrderItem
	add IsExtraOutPutFile bit null

	Go
	update Order_ClientOrderItem set IsExtraOutPutFile = 0
	Go

	ALTER TABLE Order_ClientOrderItem
	alter column IsExtraOutPutFile bit not null 



GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItem_insert]    Script Date: 1/2/2023 5:25:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItem_insert]
(
	
	@CompanyId int,
	@FileName nvarchar(max),
	@ClientOrderId int,

	@PartialPath nvarchar(250),
	@Status int,
	@IsDeleted bit,
	@CreatedDate datetime,
	@UpdatedDate datetime,
	@ObjectId nvarchar(max),
	@FileSize bigint,
	@TeamId int,
	@ExternalStatus int,
	@FileByteString nvarchar(max),
	@InternalFileOutputPath nvarchar(max),
	@InternalFileInputPath nvarchar(max),
	@ExternalFileOutputPath nvarchar(max),
	@FileNameWithoutExtension nvarchar(200),
	@FileGroup int,
	@IsExtraOutPutFile bit
)
as
begin
	insert into 
	Order_ClientOrderItem([FileName],ClientOrderId,[Status],IsDeleted, CreatedDate,UpdatedDate,ObjectId,FileSize,ExternalStatus,FileByteString,InternalFileOutputPath,InternalFileInputPath,ExternalFileOutputPath,CompanyId,PartialPath,FileNameWithoutExtension,[FileGroup],IsExtraOutPutFile)
					  
	values(@FileName,@ClientOrderId,@Status,@IsDeleted, @CreatedDate,@UpdatedDate,@ObjectId,@FileSize,@ExternalStatus,@FileByteString,@InternalFileOutputPath,@InternalFileInputPath,@ExternalFileOutputPath,@CompanyId,@PartialPath,@FileNameWithoutExtension,@FileGroup,@IsExtraOutPutFile)

    SELECT SCOPE_IDENTITY();
end
