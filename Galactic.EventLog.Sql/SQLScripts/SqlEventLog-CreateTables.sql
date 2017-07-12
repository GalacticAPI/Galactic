-- ---------- INITIALIZATION ----------

-- Use the event log database.
USE YOUR_DB_NAME_HERE;
GO

-- Allows for the use of identifiers that contain characters not generally allowed,
-- or are the same as Transact-SQL reserved words.
SET QUOTED_IDENTIFIER ON;
GO

-- ---------- CREATE TABLES ----------

-- Create a table to contain a list of sources in use by events in the log.
CREATE TABLE dbo.EventSource
(
	id int NOT NULL IDENTITY PRIMARY KEY,
	name nvarchar(80) UNIQUE NOT NULL
)

-- Create a table to contain a list of categories in use by events in the log.
CREATE TABLE dbo.EventCategory
(
	id int NOT NULL IDENTITY PRIMARY KEY,
	name nvarchar(80) UNIQUE NOT NULL
)

-- Create a table to contain a list of severity levels allowed for use by events in the log.
CREATE TABLE dbo.EventSeverity
(
	id int NOT NULL IDENTITY PRIMARY KEY,
	name nvarchar(20) UNIQUE NOT NULL
)

-- Create a table to contain all the events in the log.
CREATE TABLE dbo.EventLog
(
	id bigint NOT NULL IDENTITY PRIMARY KEY,
	"date" datetime2 NOT NULL,
	sourceId int NOT NULL REFERENCES dbo.EventSource(id) ON DELETE CASCADE,
	categoryId int NOT NULL DEFAULT 'None' REFERENCES dbo.EventCategory(id) ON DELETE SET DEFAULT,
	severityId int NOT NULL DEFAULT 'Unknown' REFERENCES dbo.EventSeverity(id) ON DELETE SET DEFAULT,
	details nvarchar(4000) NOT NULL
)