##SqlMigration
============

An easy migration tool for using straight sql files to migrate a database in a forward only direction.  Each change to the database is a new sql file created in your migrations folder.  The file needs to be in the following format so it knows how to order the scripts when creating the overall deployment sql script.  

2008-04-06_16h20m_WhateverDescriptionYouWant.sql  -- This translates to a change script at 4/6/2008 4:20PM.

The process is very simple.  Each migration file is run only once.  Once it is run, the file name is kept in a table in your database called 'SqlMigration'.  This table is always checked to see if the migration script has already been run, so to make sure it is run once and only once in each of your databases.  

##Migration folder

This folder contains sql change scripts with filenames with the correct format for SqlMigration to pick up on.  It can be anywhere as you pass this location in during usage.  

If you want to add some test data so you can test your datalayer or do small integration tests easily on your CI builds, you can include a folder called 'test' inside your migration folder.  The files follow the same format as regular migrations, and will only be run included in the deployment script if you use the /t option.

##Usage
#Migrate forward
This is to migrate the database forward using the SqlMigration tool.  
Example: SqlMigration.Runner.exe /m /sd "c:\your\migrations\folder" /cs "your connection string"


#Pro tips

Do not rename or delete your change scripts, as you may make creating your current database schema / structure impossible.  

Do not edit already run scripts as they will not be run again, just create a new script to make your changes. Each script is immutable in a sense.
