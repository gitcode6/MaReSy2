CREATE TABLE [dbo].[users]
(
	[userID] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [username] NVARCHAR(30) NOT NULL UNIQUE, 
    [firstname] NVARCHAR(25) NOT NULL, 
    [lastname] NVARCHAR(25) NOT NULL, 
    [password] NVARCHAR(20) NOT NULL, 
    [email] NVARCHAR(50) NOT NULL UNIQUE, 
    [roleID] INT NOT NULL,
    FOREIGN KEY (roleID) REFERENCES roles(roleID),
)
