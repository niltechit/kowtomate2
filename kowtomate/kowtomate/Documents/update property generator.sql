
GO
/****** Object:  StoredProcedure [dbo].[Helper_GenerateUpdateEntityCode]    Script Date: 01/10/2022 8:24:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Md Aminul
-- Create date: 7-May-2018
-- Description:	Generate Script 
-- [dbo].[Helper_GenerateUpdateEntityCode]  'TutorialGroup'
-- =============================================
ALTER PROCEDURE  [dbo].[Helper_GenerateUpdateEntityCode]
@tableName varchar(200)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   

SELECT  
 LOWER(@tableName) +'.' + C.name  + ' = serviceModel.'  + C.name + ';'  AS Table_Name        
FROM   sys.objects AS T
       JOIN sys.columns AS C ON T.object_id = C.object_id
       --JOIN sys.types AS P ON C.system_type_id = P.system_type_id
WHERE  T.type_desc = 'USER_TABLE' and T.name = @tableName
AND c.name not in  ('Id','')
ORDER BY C.column_id

END


