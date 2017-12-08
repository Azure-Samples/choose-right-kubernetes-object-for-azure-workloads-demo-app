GO
IF(db_id(N'snoopyshoppingcart') IS NULL)
BEGIN
	CREATE DATABASE snoopyshoppingcart
END;

GO
USE snoopyshoppingcart

GO
IF NOT EXISTS (SELECT * FROM sysobjects WHERE NAME='shopping' AND XTYPE='U')
BEGIN
	CREATE TABLE shopping
	(
		AddedOn datetime,
		ConnectionID nvarchar(100),
		IP nvarchar(20),
		CartItem nvarchar(100)
	)
END


