DECLARE @debug varchar(max), @errorNumber int, @errorMessage varchar(max)

BEGIN TRY
    BEGIN TRANSACTION SqlMigrationTransaction
        SET @debug = 'Starting Migrations' + CHAR(13);
                IF (SELECT COUNT(NAME) FROM SqlMigration WHERE Name = '2008-01-01_01h11m-test.sql') = 0
        BEGIN
        set @debug = @debug + CHAR(13) + 'Starting 2008-01-01_01h11m-test.sql'
                exec ('command1')
                exec ('command2')
                set @debug = @debug + CHAR(13) + 'Ending 2008-01-01_01h11m-test.sql'
        INSERT INTO SqlMigration (Name) VALUES ('2008-01-01_01h11m-test.sql')
        END
                IF (SELECT COUNT(NAME) FROM SqlMigration WHERE Name = '2008-01-01_01h12m-test.sql') = 0
        BEGIN
        set @debug = @debug + CHAR(13) + 'Starting 2008-01-01_01h12m-test.sql'
                exec ('command1')
                exec ('command2')
                set @debug = @debug + CHAR(13) + 'Ending 2008-01-01_01h12m-test.sql'
        INSERT INTO SqlMigration (Name) VALUES ('2008-01-01_01h12m-test.sql')
        END
            COMMIT TRANSACTION SqlMigrationTransaction

END TRY
BEGIN CATCH
    SET @errorNumber = ERROR_NUMBER();
    SET @errorMessage = ERROR_MESSAGE();
END CATCH

SELECT @errorNumber as ErrorNumber, @errorMessage as ErrorMessage, @debug as Tracing;