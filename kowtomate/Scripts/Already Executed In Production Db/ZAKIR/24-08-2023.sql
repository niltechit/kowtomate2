
-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: <Create Date,,>
-- Description:	Get ClientOrderTtems for Retouched.ai processing 
-- =============================================
ALTER PROCEDURE [dbo].[SP_Order_ClientOrderItems_for_retouchedai]
    -- Add the parameters for the stored procedure here
    @CompanyId bigint 
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
    -- Insert statements for procedure here
SELECT TOP 100
        item.[Id],
        item.[CompanyId],
        item.[ClientOrderId],
        item.[FileName],
        item.[ExteranlFileInputPath],
        item.[ExternalFileOutputPath],
        item.[InternalFileInputPath],
        item.[InternalFileOutputPath],
        item.[UnitPrice],
        item.[Status],
        item.[IsDeleted],
        item.[CreatedDate],
        item.[CreatedByContactId],
        item.[UpdatedDate],
        item.[UpdatedByContactId],
        item.[ObjectId],
        item.[FileSize],
        item.[DistributedTime],
        item.[DistributedByContactId],
        item.[DeadlineTime],
        item.[InProductionTime],
        item.[ProductionDoneTime],
        item.[InQcTime],
        item.[QcByContactId],
        item.[RejectCount],
        item.[TeamId],
        item.[ExternalStatus],
        item.[FileByteString],
        item.[ProductionFileByteString],
        item.[ProductionDoneFilePath],
        item.[PartialPath],
        item.[FileNameWithoutExtension],
        item.[FileGroup],
        item.[IsExtraOutPutFile],
        item.[ArrivalTime],
        item.[IbrProcessedImageUrl],
        item.[IbrStatus]
    FROM
        Order_ClientOrderItem item
    INNER JOIN
        Order_ClientOrder oco ON oco.Id = item.ClientOrderId
	--LEFT JOIN Order_AssignedImageEditor AS oa with(nolock) ON oa.Order_ImageId = item.Id AND oa.IsActive = 1
    WHERE
        item.CompanyId = 1176
        AND item.Status BETWEEN 1 AND 7 AND item.IbrStatus is null OR item.IbrStatus <> 1
    ORDER BY
		item.Status desc,
        oco.ArrivalTime ASC, item.ArrivalTime ASC;
END
GO
-- =============================================
-- Author:		Md Zakir Hossain
-- Create date: <Create Date,,>
-- Description:	Check item retouched.ai processed.
-- =============================================
CREATE PROCEDURE SP_Order_ClientOrderItems_CheckRetouched_Processed
	-- Add the parameters for the stored procedure here
	@Id bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [Id]
    ,[CompanyId]
    ,[ClientOrderId]
    ,[FileName]
    ,[ExteranlFileInputPath]
    ,[ExternalFileOutputPath]
    ,[InternalFileInputPath]
    ,[InternalFileOutputPath]
    ,[UnitPrice]
    ,[Status]
    ,[IsDeleted]
    ,[CreatedDate]
    ,[CreatedByContactId]
    ,[UpdatedDate]
    ,[UpdatedByContactId]
    ,[ObjectId]
    ,[FileSize]
    ,[DistributedTime]
    ,[DistributedByContactId]
    ,[DeadlineTime]
    ,[InProductionTime]
    ,[ProductionDoneTime]
    ,[InQcTime]
    ,[QcByContactId]
    ,[RejectCount]
    ,[TeamId]
    ,[ExternalStatus]
    ,[FileByteString]
    ,[ProductionFileByteString]
    ,[ProductionDoneFilePath]
    ,[PartialPath]
    ,[FileNameWithoutExtension]
    ,[FileGroup]
    ,[IsExtraOutPutFile]
    ,[ArrivalTime]
    ,[IbrProcessedImageUrl]
    ,[IbrStatus] FROM Order_ClientOrderItem item
    WHERE item.Id =@Id
END
GO
