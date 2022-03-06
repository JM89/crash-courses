PROJECT=

build-app:
	@echo "Building my app"
	dotnet clean
	dotnet restore
	dotnet build

test-app: build-app
	@echo "Running tests"
	dotnet test

publish-app:
	@echo "Publishing application $(PROJECT)"
	dotnet publish $(PROJECT) --configuration Release --self-contained true --runtime win10-x64 --output artefact