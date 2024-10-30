ALTER TABLE Order_ClientOrder
ADD BatchPath NVARCHAR(MAX) NULL

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
	@SourceServerId int = null,
	@BatchPath NVARCHAR(MAX) = NULL
)
as 
begin 
	insert into Order_ClientOrder(CompanyId,FileServerId,OrderNumber,OrderPlaceDate,CreatedDate,UpdatedDate,UpdatedByContactId,ObjectId,IsDeleted,ExternalOrderStatus,InternalOrderStatus,Instructions,AssignedTeamId,SourceServerId,BatchPath)
	values(@CompanyId,@FileServerId,@OrderNumber,@OrderPlaceDate,@CreatedDate,@UpdatedDate,@UpdatedByContactId,@ObjectId,@IsDeleted,@ExternalOrderStatus,@InternalOrderStatus,@Instructions,@AssignedTeamId,@SourceServerId,@BatchPath)
	SELECT SCOPE_IDENTITY();
end

