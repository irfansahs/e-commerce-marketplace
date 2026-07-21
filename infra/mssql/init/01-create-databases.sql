-- Per-service databases (Sprint 0). Idempotent; run by mssql-init on every start.
IF DB_ID(N'identity_db') IS NULL CREATE DATABASE identity_db;
GO
IF DB_ID(N'catalog_db') IS NULL CREATE DATABASE catalog_db;
GO
IF DB_ID(N'cart_order_db') IS NULL CREATE DATABASE cart_order_db;
GO
IF DB_ID(N'inventory_db') IS NULL CREATE DATABASE inventory_db;
GO
IF DB_ID(N'payment_db') IS NULL CREATE DATABASE payment_db;
GO
IF DB_ID(N'notification_db') IS NULL CREATE DATABASE notification_db;
GO
