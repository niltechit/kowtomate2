ALTER TABLE CompanyGeneralSettings
ADD isFtpFolderPreviousStructureWiseStayInFtp bit default 0;

go

-- =============================================
-- Author: Md Zakir Hossain
-- Create date: <Create Date,,>
-- Description: Check item with CompanyId, CreatedDate, and InternalFileInputPath.
-- =============================================
CREATE PROCEDURE [dbo].[SP_Order_ClientOrderItems_CheckWithCompanyIdAndFilePath]
    -- Add the parameters for the stored procedure here
	-- exec [dbo].[SP_Order_ClientOrderItems_CheckWithCompanyIdAndFilePath] 1177,'/ecommerce/2023-09-05/32114317-1.jpg','2023-09-05'
    @CompanyId int,
    @InternalFileInputPath varchar(max),
    @CreatedDate datetime
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
        Order_ClientOrderItem item
    WHERE 
        item.CompanyId = @CompanyId
        AND CONVERT(date, item.CreatedDate)  = @CreatedDate
        AND item.InternalFileInputPath LIKE '%' + @InternalFileInputPath
END
