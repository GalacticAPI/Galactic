-- ---------- INITIALIZATION ----------

-- Use the event log database.
USE YOUR_DB_NAME_HERE;
GO

-- Allows for the use of identifiers that contain characters not generally allowed,
-- or are the same as Transact-SQL reserved words.
SET QUOTED_IDENTIFIER ON;
GO

-- ---------- STORED PROCEDURES ----------

-- ----------------------------------------
-- AddCategory
-- Summary: Adds a category to the log.
-- Param: @name = The name of the category.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.AddCategory') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.AddCategory;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.AddCategory
	@name nvarchar(80)
AS
-- Inserts a category with the supplied name into the log.
INSERT INTO dbo.EventCategory (name)
	VALUES (@name);
GO
-- ----------------------------------------

-- ----------------------------------------
-- AddEvent
-- Summary: Adds an event to the log.
-- Param: @sourceId = The source ID of the event.
-- Param: @categoryId = The category ID of the event.
-- Param: @severityId = The severity level ID of the event.
-- Param: @details = The details of the event.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.AddEvent') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.AddEvent;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.AddEvent
	@sourceId int,
	@categoryId int,
	@severityId int,
	@details nvarchar(4000)
AS
-- Inserts an event to the log with the current date/time and supplied parameters.
INSERT INTO dbo.EventLog ("date", sourceId, categoryId, severityId, details)
	VALUES (SYSDATETIMEOFFSET(), @sourceId, @categoryId, @severityId, @details);
GO
-- ----------------------------------------

-- ----------------------------------------
-- AddSeverityLevel
-- Summary: Adds a severity level to the log.
-- Param: @name = The name of the severity level.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.AddSeverityLevel') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.AddSeverityLevel;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.AddSeverityLevel
	@name nvarchar(20)
AS
-- Inserts a severity level with the supplied name into the log.
INSERT INTO dbo.EventSeverity (name)
	VALUES (@name);
GO
-- ----------------------------------------

-- ----------------------------------------
-- AddSource
-- Summary: Adds a source to the log.
-- Param: @name = The name of the source.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.AddSource') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.AddSource;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.AddSource
	@name nvarchar(80)
AS
-- Inserts a source with the supplied name into the log.
INSERT INTO dbo.EventSource (name)
	VALUES (@name);
GO
-- ----------------------------------------

-- ----------------------------------------
-- DeleteCategory
-- Summary: Deletes a category from the log.
-- Param: @id = The ID of the category.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.DeleteCategory') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.DeleteCategory;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.DeleteCategory
	@id int
AS
-- Deletes a category from the log that matches the supplied ID.
DELETE FROM dbo.EventCategory
	WHERE @id = id
GO
-- ----------------------------------------

-- ----------------------------------------
-- DeleteEvent
-- Summary: Deletes an event from the log.
-- Param: @date = The date of the event.
-- Param: @sourceId = The source ID of the event.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.DeleteEvent') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.DeleteEvent;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.DeleteEvent
	@date datetime2,
	@sourceId int
AS
-- Deletes an event from the log that matches the supplied date and source ID.
DELETE FROM dbo.EventLog
	WHERE @date = date AND @sourceId = sourceId;
GO
-- ----------------------------------------

-- ----------------------------------------
-- DeleteSeverityLevel
-- Summary: Deletes a severity level from the log.
-- Param: @id = The ID of the severity level.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.DeleteSeverityLevel') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.DeleteSeverityLevel;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.DeleteSeverityLevel
	@id int
AS
-- Deletes a severity level from the log that matches the supplied ID.
DELETE FROM dbo.EventSeverity
	WHERE @id = id
GO
-- ----------------------------------------

-- ----------------------------------------
-- DeleteSource
-- Summary: Deletes a source from the log.
-- Param: @id = The ID of the source.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.DeleteSource') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.DeleteSource;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.DeleteSource
	@id int
AS
-- Deletes a source from the log that matches the supplied ID.
DELETE FROM dbo.EventSource
	WHERE @id = id
GO
-- ----------------------------------------

-- ----------------------------------------
-- GetCategoryIdByName
-- Summary: Gets the id of a category from the log with the supplied name.
-- Param: @name = The name of the category to return.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.GetCategoryIdByName') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.GetCategoryIdByName;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.GetCategoryIdByName
	@name nvarchar(80) = NULL
AS
-- Get the id of a category from the log that matches the name supplied.
SELECT id
	FROM dbo.EventCategory
	WHERE @name = name
GO
-- ----------------------------------------

-- ----------------------------------------
-- GetCategories
-- Summary: Gets categories from the log that match specified criteria.
-- Note: All parameters are optional. Supplying no parameters will return all categories.
-- Param: @id = The ID of the category to return.
-- Param: @name = The name of the category to return.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.GetCategories') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.GetCategories;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.GetCategories
	@id int = NULL,
	@name nvarchar(80) = NULL
AS
-- Get categories from the log that match non-null parameters.
SELECT id, name
	FROM dbo.EventCategory
	WHERE (@id = id OR @id IS NULL) AND
		  (@name = name OR @name IS NULL)
GO
-- ----------------------------------------

-- ----------------------------------------
-- GetEvents
-- Summary: Gets events from the log that match specified criteria.
-- Note: All parameters are optional. Supplying no parameters will return all events.
-- Param: @sourceId = The source ID of events to return.
-- Param: @categoryId = The category ID of events to return.
-- Param: @severityId = The severity level ID of events to return.
-- Param: @begin = The beginning of the date range for returning events.
-- Param: @end = The end of the date range for returning events.
-- Return: Events in the log that match the specified criteria ordered by date.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.GetEvents') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.GetEvents;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.GetEvents
	@sourceId int = NULL,
	@categoryId int = NULL,
	@severityId int = NULL,
	@begin datetime2 = NULL,
	@end datetime2 = NULL
AS
-- Get events from the log that match non-null parameters.
SELECT "date", sourceId, categoryId, severityId, details
	FROM dbo.EventLog
	WHERE (@sourceId = sourceId OR @sourceId IS NULL) AND
		  (@categoryId = categoryId OR @categoryId IS NULL) AND
		  (@severityId = severityId OR @severityId IS NULL) AND
		  (@begin <= "date" OR @begin IS NULL) AND
		  (@end >= "date" OR @end IS NULL)
	ORDER BY "date";
GO
-- ----------------------------------------

-- ----------------------------------------
-- GetSeverityLevels
-- Summary: Gets severity levels from the log that match specified criteria.
-- Note: All parameters are optional. Supplying no parameters will return all severity levels.
-- Param: @id = The ID of the severity level to return.
-- Param: @name = The name of the severity level to return.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.GetSeverityLevels') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.GetSeverityLevels;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.GetSeverityLevels
	@id int = NULL,
	@name nvarchar(20) = NULL
AS
-- Get severity levels from the log that match non-null parameters.
SELECT id, name
	FROM dbo.EventSeverity
	WHERE (@id = id OR @id IS NULL) AND
		  (@name = name OR @name IS NULL)
GO
-- ----------------------------------------

-- ----------------------------------------
-- GetSources
-- Summary: Gets sources from the log that match specified criteria.
-- Note: All parameters are optional. Supplying no parameters will return all sources.
-- Param: @id = The ID of the source to return.
-- Param: @name = The name of the source to return.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.GetSources') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.GetSources;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.GetSources
	@id int = NULL,
	@name nvarchar(80) = NULL
AS
-- Get sources from the log that match non-null parameters.
SELECT id, name
	FROM dbo.EventSource
	WHERE (@id = id OR @id IS NULL) AND
		  (@name = name OR @name IS NULL)
GO
-- ----------------------------------------

-- ----------------------------------------
-- UpdateCategory
-- Summary: Updates a category in the log.
-- Param: @id = The ID of the category.
-- Param: @name = The name of the category.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.UpdateCategory') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.UpdateCategory;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.UpdateCategory
	@id int,
	@name nvarchar(80)
AS
-- Updates a category in the log that matches the supplied ID with the supplied name.
UPDATE dbo.EventCategory
	SET name = @name
	WHERE @id = id
GO
-- ----------------------------------------

-- ----------------------------------------
-- UpdateEvent
-- Summary: Updates an event in the log.
-- Param: @date = The date of the event.
-- Param: @sourceId = The source ID of the event.
-- Param: @categoryId = The category ID of the event.
-- Param: @severityId = The severity level ID of the event.
-- Param: @details = The details of the event.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.UpdateEvent') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.UpdateEvent;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.UpdateEvent
	@date datetime2,
	@sourceId int,
	@categoryId int,
	@severityId int,
	@details nvarchar(4000)
AS
-- Updates an event in the log that matches the supplied date and source ID with the supplied parameters.
UPDATE dbo.EventLog
	SET categoryId = @categoryId,
		severityId = @severityId,
		details = @details
	WHERE @date = "date" AND
		  @sourceId = sourceId;
GO
-- ----------------------------------------

-- ----------------------------------------
-- UpdateSeverityLevel
-- Summary: Updates a severity level in the log.
-- Param: @id = The ID of the severity level.
-- Param: @name = The name of the severity level.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.UpdateSeverityLevel') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.UpdateSeverityLevel;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.UpdateSeverityLevel
	@id int,
	@name nvarchar(20)
AS
-- Updates a severity level in the log that matches the supplied ID with the supplied name.
UPDATE dbo.EventSeverity
	SET name = @name
	WHERE @id = id
GO
-- ----------------------------------------

-- ----------------------------------------
-- UpdateSource
-- Summary: Updates a source in the log.
-- Param: @id = The ID of the source.
-- Param: @name = The name of the source.

-- Check that the procedure doesn't already exist.
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.UpdateSource') AND type in (N'P', N'PC'))
BEGIN
	-- The procedure exists, drop it.
	DROP PROCEDURE dbo.UpdateSource;
END
GO

-- Create the procedure.
CREATE PROCEDURE dbo.UpdateSource
	@id int,
	@name nvarchar(80)
AS
-- Updates a source in the log that matches the supplied ID with the supplied name.
UPDATE dbo.EventSeverity
	SET name = @name
	WHERE @id = id
GO
-- ----------------------------------------

-- ---------- GRANT PERMISSIONS TO STORED PROCEDURES ----------

-- Grant permissions for the service account on the database.
GRANT EXECUTE ON dbo.AddCategory TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.AddEvent TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.AddSeverityLevel TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.AddSource TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.DeleteCategory TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.DeleteEvent TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.DeleteSeverityLevel TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.DeleteSource TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.GetCategories TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.GetCategoryIdByName TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.GetEvents TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.GetSeverityLevels TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.GetSources TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.UpdateCategory TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.UpdateEvent TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.UpdateSeverityLevel TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GRANT EXECUTE ON dbo.UpdateSource TO [YOUR_SERVICE_ACCOUNT_NAME_HERE];
GO