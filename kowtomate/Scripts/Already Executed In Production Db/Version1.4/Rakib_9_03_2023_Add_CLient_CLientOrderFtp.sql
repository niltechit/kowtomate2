CREATE TABLE Client_ClientOrderFtp (
    Id BIGINT PRIMARY KEY IDENTITY(1,1),
    ClientCompanyId BIGINT NOT NULL,
    Host VARCHAR(255) NOT NULL,
    Port INT NOT NULL,
    Username VARCHAR(255) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    RootFolder VARCHAR(255) NOT NULL,
    IsEnable BIT NOT NULL
);


alter table Client_ClientOrderFtp add constraint df_Client_ClientOrderFtp_IsEnable default 1 for IsEnable


GO
/****** Object:  StoredProcedure [dbo].[SP_Client_ClientOrderFtp_GetAll]    Script Date: 3/9/2023 6:40:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



Create PROCEDURE [dbo].[SP_Client_ClientOrderFtp_GetAll]

AS
BEGIN  

	SELECT * FROM [dbo].[Client_ClientOrderFtp] where IsEnable = 1

END


