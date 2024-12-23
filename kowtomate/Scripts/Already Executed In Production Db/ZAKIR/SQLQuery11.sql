USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Order_ClientOrderItems_CheckWithCompanyIdAndFilePath]    Script Date: 11/27/2023 2:38:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItems_CheckWithCompanyIdAndFilePath]
    -- Add the parameters for the stored procedure here
	-- exec [dbo].[SP_Order_ClientOrderItems_CheckWithCompanyIdAndFilePath] 1177,'/ecommerce/2023-09-05/32114317-1.jpg','2023-09-05'
    @CompanyId int,
    @InternalFileInputPath nvarchar(max),
    @CreatedDate datetime = null
AS 
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- Insert statements for procedure here
    SELECT 
        [Id],
        [CompanyId],
        [ClientOrderId],
        [FileName],
        [ExteranlFileInputPath],
        [ExternalFileOutputPath],
        [InternalFileInputPath],
        [InternalFileOutputPath],
        [UnitPrice],
        [Status],
        [IsDeleted],
        [CreatedDate],
        [CreatedByContactId],
        [UpdatedDate],
        [UpdatedByContactId],
        [ObjectId],
        [FileSize],
        [DistributedTime],
        [DistributedByContactId],
        [DeadlineTime],
        [InProductionTime],
        [ProductionDoneTime],
        [InQcTime],
        [QcByContactId],
        [RejectCount],
        [TeamId],
        [ExternalStatus],
        [FileByteString],
        [ProductionFileByteString],
        [ProductionDoneFilePath],
        [PartialPath],
        [FileNameWithoutExtension],
        [FileGroup],
        [IsExtraOutPutFile],
        [ArrivalTime],
        [IbrProcessedImageUrl],
        [IbrStatus]
    FROM 
        Order_ClientOrderItem item WITH(NOLOCK)
    WHERE 
        item.CompanyId = @CompanyId
        AND (@CreatedDate IS NULL OR CONVERT(date, item.CreatedDate) = @CreatedDate)
        AND item.InternalFileInputPath LIKE '%' + @InternalFileInputPath
END
