SET migrate="..\..\..\packages\FluentMigrator.1.6.2\tools\Migrate.exe"

%migrate% -t=migrate -a .\Papyrus.Infrastructure.Migrations.dll -conn Papyrus -db sqlServer2012

call listmigrations.bat
