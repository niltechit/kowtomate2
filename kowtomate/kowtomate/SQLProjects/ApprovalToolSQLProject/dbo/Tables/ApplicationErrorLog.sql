CREATE TABLE [dbo].[ApplicationErrorLog]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Date] DATETIME NOT NULL, 
    [Type] INT NOT NULL, 
    [LogMessage] NVARCHAR(500) NOT NULL
)
