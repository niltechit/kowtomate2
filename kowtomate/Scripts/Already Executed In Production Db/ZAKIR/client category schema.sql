CREATE table Common_Category
(
	Id int not null primary key Identity(1,1),
	Name nvarchar(200) not null CONSTRAINT UQ_Common_Category_Name unique,
	TimeInMinutes decimal(10,2) null,
	PriceInUSD Decimal(10,2) not null,
	IsActive BIT NOT NULL,
	[CreatedDate] Datetime not null,
	CreatedByUsername nvarchar(50),
	UpdatedDate datetime,
	UpdatedByUsername nvarchar(50)
)
 
GO
 
CREATE table Common_Service
(
	Id int not null primary key Identity(1,1),
	Name nvarchar(200) not null CONSTRAINT UQ_Common_Service_Name unique,
	TimeInMinutes decimal(10,2) null,
	PriceInUSD Decimal(10,2) null,
	IsActive BIT NOT NULL,
	CreatedDate Datetime not null,
	CreatedByUsername nvarchar(50),
	UpdatedDate datetime,
	UpdatedByUsername nvarchar(50)
)
GO
 
CREATE table Common_CategoryService
(
	Id int not null primary key Identity(1,1),
	CommonCategoryId int NOT NULL,
	CommonServiceId int NOT NULL,
	TimeInMinutes decimal(10,2) null,
	PriceInUSD Decimal(10,2) null,
	CONSTRAINT FK_CategoryService_Category FOREIGN KEY (CommonCategoryId) REFERENCES Common_Category(Id),
    CONSTRAINT FK_CategoryService_Service FOREIGN KEY (CommonServiceId) REFERENCES Common_Service(Id),
    CONSTRAINT UQ_CategoryService_Category_Service UNIQUE (CommonCategoryId, CommonServiceId)
)
 
GO
CREATE table Client_Category
(
	Id int not null primary key Identity(1,1),
	ClientCompanyId INT NOT NULL,
	CommonCategoryId INT NOT NULL,
	TimeInMinutes decimal(10,2) null,
	PriceInUSD Decimal(10,2) not null,
	IsActive BIT NOT NULL,
	[CreatedDate] Datetime not null,
	CreatedByUsername nvarchar(50),
	UpdatedDate datetime,
	UpdatedByUsername nvarchar(50),
	CONSTRAINT FK_ClientCategory_Category FOREIGN KEY (CommonCategoryId) REFERENCES Common_Category(Id),
    CONSTRAINT FK_ClientCategory_Company FOREIGN KEY (ClientCompanyId) REFERENCES Common_Company(Id),
    CONSTRAINT UQ_CategoryService_ClientCompany_Category UNIQUE (ClientCompanyId, CommonCategoryId)
)
 
GO
CREATE table Client_CategoryService
(
	Id int not null primary key Identity(1,1),
	ClientCategoryId INT NOT NULL,
	CommonServiceId int NOT NULL,
	TimeInMinutes decimal(10,2) null,
	PriceInUSD Decimal(10,2) not null,
	[CreatedDate] Datetime not null,
	CreatedByUsername nvarchar(50),
	UpdatedDate datetime,
	UpdatedByUsername nvarchar(50),
	CONSTRAINT FK_ClientCategoryService_Category FOREIGN KEY (ClientCategoryId) REFERENCES Client_Category(Id),
    CONSTRAINT FK_ClientCategoryService_Service FOREIGN KEY (CommonServiceId) REFERENCES Common_Service(Id),
    CONSTRAINT UQ_ClientCategoryService_CategoryService UNIQUE (ClientCategoryId, CommonServiceId)
)