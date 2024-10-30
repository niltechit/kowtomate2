-- =============================================
-- Author:		Aminul
-- Create date: 11 Jan 2021
-- Description:	Get Last inserted id by table Name
-- =============================================
CREATE PROCEDURE [dbo].[spGetLastInsertedId]	
@tableName as varchar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT IDENT_CURRENT(@tableName) as InsertId
  

END