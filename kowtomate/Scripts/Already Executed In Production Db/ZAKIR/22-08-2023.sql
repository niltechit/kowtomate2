ALTER TABLE Order_ClientOrderItem
ADD IbrProcessedImageUrl NVARCHAR(max),
	IbrStatus INT


GO

CREATE PROCEDURE [dbo].[SP_Order_ClientOrderItem_Update_after_IbrProcessed]
(
    @Id bigint,
	@IbrProcessedImageUrl NVARCHAR(max)=NULL,
	@IbrStatus INT=NULL
)
AS
BEGIN
    UPDATE [dbo].Order_ClientOrderItem
    SET
       
        IbrProcessedImageUrl = ISNULL(@IbrProcessedImageUrl, IbrProcessedImageUrl),
        IbrStatus = ISNULL(@IbrStatus, IbrStatus)
    WHERE Id = @Id
END

