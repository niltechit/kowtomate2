ALTER TABLE Order_ClientOrder
ADD SourceServerId bigint FOREIGN KEY REFERENCES Client_ClientOrderFtp(Id);



GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrder_Insert]    Script Date: 7/17/2023 6:39:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_Order_ClientOrder_Insert]
(
	@CompanyId int,
	@FileServerId int,
	@OrderNumber nvarchar(30),
	@OrderPlaceDate datetime,
	@CreatedDate datetime,
	@UpdatedDate datetime,
	@UpdatedByContactId int,
	@ObjectId nvarchar(40),
	@IsDeleted bit ,
	@ExternalOrderStatus tinyint,
	@InternalOrderStatus tinyint,
	@Instructions varchar(40),
	@AssignedTeamId int,
	@SourceServerId int
)
as 
begin 
	insert into Order_ClientOrder(CompanyId,FileServerId,OrderNumber,OrderPlaceDate,CreatedDate,UpdatedDate,UpdatedByContactId,ObjectId,IsDeleted,ExternalOrderStatus,InternalOrderStatus,Instructions,AssignedTeamId,SourceServerId)
	values(@CompanyId,@FileServerId,@OrderNumber,@OrderPlaceDate,@CreatedDate,@UpdatedDate,@UpdatedByContactId,@ObjectId,@IsDeleted,@ExternalOrderStatus,@InternalOrderStatus,@Instructions,@AssignedTeamId,@SourceServerId)
	SELECT SCOPE_IDENTITY();
end