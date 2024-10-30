alter table CompanyGeneralSettings
add IsZipParentFolderSave bit default 0

update CompanyGeneralSettings set
IsZipParentFolderSave = 0
