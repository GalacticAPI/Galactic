-- ---------- INITIALIZATION ----------

-- Use the event log database.
USE YOUR_DB_NAME_HERE;
GO

-- Allows for the use of identifiers that contain characters not generally allowed,
-- or are the same as Transact-SQL reserved words.
SET QUOTED_IDENTIFIER ON;
GO

-- ---------- INITIALIZE TABLES ----------

-- Initialize the severity levels for the event log.
EXEC dbo.AddSeverityLevel 'Error';
EXEC dbo.AddSeverityLevel 'Information';
EXEC dbo.AddSeverityLevel 'Warning';