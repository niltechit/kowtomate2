
-- =============================================
-- Author:		Shabuj Hossain
-- Create date: 13 Jan 2021
-- Description:	Save Company info 
-- =============================================

CREATE PROCEDURE [dbo].[Common_sp_Company_Update](
            @Id  int,
            @Name  nvarchar(100),
            @Code  nvarchar(6),
            @CompanyType tinyint,
            @Telephone varchar(30),
            @Address1 varchar(100),
            @Address2 varchar(100),
            @City varchar(30),
            @State varchar(30),
            @Zipcode varchar(10),
            @Country varchar(50),
            @Status int,
            @UpdatedByContactId int
)
AS
BEGIN  
    UPDATE [dbo].[Common_Company]
    SET
	    Name = @Name, 
        Code = @Code,
		CompanyType= @CompanyType,
		Telephone =@Telephone,
		Address1=@Address1,
		Address2= @Address2,
		City =@City,
		State = @State,
		Zipcode =@Zipcode ,
		Country = @Country,
		Status = @Status,
		UpdatedDate = SYSDATETIME(),
		UpdatedByContactId =@UpdatedByContactId
		
     WHERE Id = @Id
END



