UPDATE [dbo].[products]
SET productimage = (
SELECT BulkColumn
FROM OPENROWSET(BULK 'C:\Users\tobias.lehner\Downloads\reanimationspuppe.jpeg', SINGLE_BLOB) AS Bild
)
WHERE productId = 1;