##SqlMigration
============

An easy migration tool for using straight sql files to migrate a database in a forward only direction.  Each change to the database is a new sql file created in your migrations folder.  The file needs to be in the following format so it knows how to order the scripts when creating the overall deployment sql script.  

2008-04-06_16h20m_WhateverDescriptionYouWant.sql  

This translates to a change script at 4/6/2008 4:20PM.

##Migration folder

This folder contains sql change scripts with filenames with the correct format for SqlMigration to pick up on.  It can be anywhere as you pass this location in during usage.  

If you want to add some test data so you can test your datalayer or do small integration tests easily on your CI builds, you can include a folder called 'test' inside your migration folder.  The files follow the same format as regular migrations, and will only be run included in the deployment script if you use the /t option.

##Usage

