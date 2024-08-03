CREATE TABLE [dbo].[products]
(
	[productID] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [productname] NVARCHAR(50) NOT NULL, 
    [productdescription] NVARCHAR(200) NULL, 
    [productimage] IMAGE NULL, 
    [productactive] INT NOT NULL, 
    [productamount] INT NOT NULL
)
