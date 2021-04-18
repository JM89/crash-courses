IF NOT EXISTS(SELECT 1 FROM CrashCourseDB.sys.tables where NAME = 'BlogPost' )
BEGIN	
	CREATE TABLE BlogPost (
		BlogPostId INT NOT NULL IDENTITY(1,1),
		Title VARCHAR(255) NOT NULL,
		Content NVARCHAR(4000) NOT NULL,
		CreationDate DATETIME NOT NULL
		PRIMARY KEY (BlogPostId)
	);
	
	INSERT INTO dbo.BlogPost VALUES 
	('Best practices for writing C# code', 'Blah', GETUTCDATE()),
	('How to design a distributed system properly', 'Blah', GETUTCDATE())
END
