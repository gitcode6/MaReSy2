CREATE TABLE [dbo].[sets]
(
	[setID] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [setname] VARCHAR(50) NOT NULL, 
    [setdescription] VARCHAR(200) NULL, 
    [setimage] IMAGE NULL, 
    [setactive] INT NOT NULL
)
