CREATE TABLE [dbo].[productsSets]
(
	[productSetID] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [setID] INT NOT NULL, 
    [productID] INT NOT NULL, 
    [productamount] INT NOT NULL,
    FOREIGN KEY (setID) REFERENCES sets(setID),
    FOREIGN KEY (productID) REFERENCES products(productID),
)
