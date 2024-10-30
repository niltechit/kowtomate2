

CREATE PROC [dbo].[Security_sp_GetCompanyById]
(
   @CompanyId as int
)
AS
BEGIN
SELECT * FROM [dbo].[Common_Company] where Id = @CompanyId

END




