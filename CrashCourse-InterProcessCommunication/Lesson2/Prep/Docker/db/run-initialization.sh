sleep 90s

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P VerySecret1234! -d master -i create-database.sql

sleep 5s

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P VerySecret1234! -d CrashCourseDB -i create-table.sql