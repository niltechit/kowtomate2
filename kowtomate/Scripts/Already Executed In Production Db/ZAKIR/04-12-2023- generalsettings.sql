ALTER TABLE CompanyGeneralSettings
ADD IsFtpIdleTimeChecking bit default 0;

go

ALTER TABLE CompanyGeneralSettings
ADD FtpIdleTime int null;

go

ALTER TABLE CompanyGeneralSettings
ADD FtpFileMovedPathAfterOrderCreated nvarchar(50);

go

ALTER TABLE CompanyGeneralSettings
ADD IsOrderCreatedThenFileMove bit default 0;

go


update CompanyGeneralSettings set

IsFtpIdleTimeChecking = 1,
FtpIdleTime = 2,
FtpFileMovedPathAfterOrderCreated = 'Copied',
IsOrderCreatedThenFileMove = 1
WHERE CompanyId = 1197