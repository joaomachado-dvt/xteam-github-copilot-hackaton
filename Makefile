SOLUTION=Bookstore.sln
DOTNET_IMAGE=mcr.microsoft.com/dotnet/sdk:10.0

.PHONY: build build-docker test test-coverage test-docker coverage-docker

build:
	dotnet build $(SOLUTION) -nologo

build-docker:
	docker build -f Bookstore/Dockerfile -t bookstore .

test:
	dotnet test $(SOLUTION) -nologo

test-coverage:
	dotnet test $(SOLUTION) -nologo --collect:"XPlat Code Coverage" --results-directory TestResults

test-docker:
	docker run --rm \
		-v $(PWD):/src \
		-w /src \
		$(DOTNET_IMAGE) \
		dotnet test $(SOLUTION) -nologo

coverage-docker:
	docker run --rm \
		-v $(PWD):/src \
		-w /src \
		$(DOTNET_IMAGE) \
		dotnet test $(SOLUTION) -nologo --collect:"XPlat Code Coverage" --results-directory /src/TestResults
