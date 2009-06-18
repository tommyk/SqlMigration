using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlMigration.Test
{
    internal class Sql_Files
    {
        private const string create_database = @"USE [master]
CREATE DATABASE [MigrationTest] ON  PRIMARY 
( NAME = N'MigrationTest', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\MigrationTest.mdf' ,  FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'MigrationTest_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\MigrationTest_log.ldf' ,  FILEGROWTH = 10%)
GO
EXEC dbo.sp_dbcmptlevel @dbname=N'MigrationTest', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MigrationTest].[dbo].[sp_fulltext_database] @action = 'disable'
end
GO
ALTER DATABASE [MigrationTest] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [MigrationTest] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [MigrationTest] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [MigrationTest] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [MigrationTest] SET ARITHABORT OFF 
GO
ALTER DATABASE [MigrationTest] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [MigrationTest] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [MigrationTest] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [MigrationTest] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [MigrationTest] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [MigrationTest] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [MigrationTest] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [MigrationTest] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [MigrationTest] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [MigrationTest] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [MigrationTest] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [MigrationTest] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [MigrationTest] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [MigrationTest] SET  READ_WRITE 
GO
ALTER DATABASE [MigrationTest] SET RECOVERY FULL 
GO
ALTER DATABASE [MigrationTest] SET  MULTI_USER 
GO
ALTER DATABASE [MigrationTest] SET PAGE_VERIFY CHECKSUM";


        private const string drop_database = @"USE [master]
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'MigrationTest')
BEGIN	
	ALTER DATABASE [MigrationTest] SET SINGLE_USER WITH ROLLBACK IMMEDIATE	
	DROP DATABASE [MigrationTest]
END";


        private const string create_names_table = @"USE [MigrationTest]
CREATE TABLE [dbo].[Names](
	[Name] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]";


        private const string _insert_names = @"INSERT INTO [MigrationTest].[dbo].[Names]
([Name]) VALUES ('Tom');
INSERT INTO [MigrationTest].[dbo].[Names]
([Name]) VALUES ('Tommy');
INSERT INTO [MigrationTest].[dbo].[Names]
([Name]) VALUES ('Kevin');";

        public static string Create_database
        {
            get { return create_database; }
        }

        public static string Drop_database
        {
            get { return drop_database; }
        }

        public static string Create_names_table
        {
            get { return create_names_table; }
        }

        public static string Insert_names
        {
            get { return _insert_names; }
        }
    }
}
