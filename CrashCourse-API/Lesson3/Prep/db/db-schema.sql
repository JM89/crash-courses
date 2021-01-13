IF NOT EXISTS(SELECT 1 FROM master.sys.databases WHERE Name = 'CrashCourseDB')
	CREATE DATABASE CrashCourseDB;

USE CrashCourseDB;

IF NOT EXISTS(SELECT 1 FROM CrashCourseDB.sys.tables where NAME = 'BlogPost' )
	CREATE TABLE BlogPost (
		BlogId INT NOT NULL IDENTITY(1,1),
		Title VARCHAR(255) NOT NULL,
		Content NVARCHAR(4000) NOT NULL,
		CreationDate DATETIME NOT NULL
		PRIMARY KEY (BlogId)
	);

