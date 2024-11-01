USE [KowToMateERP_Dev]
GO
/****** Object:  StoredProcedure [dbo].[SP_Client_ClientOrderFtp_GetFtpInfo_byCompanyId]    Script Date: 8/14/2023 11:46:31 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SP_Client_ClientOrderFtp_GetFtpInfo_byCompanyId]
    @ClientCompanyId bigint
AS
BEGIN
    SET NOCOUNT ON;

    SELECT * FROM Client_ClientOrderFtp WHERE ClientCompanyId = @ClientCompanyId ORDER BY IsDefault desc;
END


CREATE PROCEDURE [dbo].[SP_Client_ClientOrderFtp_GetFtpInfo_byCompanyIdAndSentOutputistrue]
    @ClientCompanyId bigint
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, OutputHost,OutputUsername,OutputPassword,OutputPort,OutputFolderName FROM Client_ClientOrderFtp
	WHERE ClientCompanyId = @ClientCompanyId AND SentOutputToSeparateFTP=1 ORDER BY IsDefault desc;
END
-- exec [dbo].[SP_Client_ClientOrderFtp_GetFtpInfo_byCompanyIdAndSentOutputistrue] 1176