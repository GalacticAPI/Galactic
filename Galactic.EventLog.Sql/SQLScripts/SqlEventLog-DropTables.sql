-- ---------- INITIALIZATION ----------

-- Use the event log database.
USE YOUR_DB_NAME_HERE;
GO

-- Allows for the use of identifiers that contain characters not generally allowed,
-- or are the same as Transact-SQL reserved words.
SET QUOTED_IDENTIFIER ON;
GO

-- ---------- DROP TABLES ----------

-- ----------------------------------------
-- dbo.EventLog

-- Check that the table doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.EventLog') AND type in (N'U'))
BEGIN
	-- The table exists, drop it.
	DROP TABLE dbo.EventLog;
END
GO
-- ----------------------------------------

-- ----------------------------------------
-- dbo.EventSource

-- Check that the table doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.EventSource') AND type in (N'U'))
BEGIN
	-- The table exists, drop it.
	DROP TABLE dbo.EventSource;
END
GO
-- ----------------------------------------


-- ----------------------------------------
-- dbo.EventCategory

-- Check that the table doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.EventCategory') AND type in (N'U'))
BEGIN
	-- The table exists, drop it.
	DROP TABLE dbo.EventCategory;
END
GO
-- ----------------------------------------

-- ----------------------------------------
-- dbo.EventSeverity

-- Check that the table doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.EventSeverity') AND type in (N'U'))
BEGIN
	-- The table exists, drop it.
	DROP TABLE dbo.EventSeverity;
END
GO
-- ----------------------------------------

