# Angular build
FROM node:12.16.3-stretch as nodeBuild
WORKDIR /src
COPY ProjectArena.WebClient .
WORKDIR /src/ProjectArena.WebClient
RUN npm install
RUN npm run-script build

# Dotnet restore
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS dotnetBuild
WORKDIR /src
COPY ProjectArena.Api .
COPY ProjectArena.Application .
COPY ProjectArena.Domain .
COPY ProjectArena.Engine .
COPY ProjectArena.Infrastructure .
COPY ProjectArena.Tests .
WORKDIR /src/ProjectArena.Api
RUN dotnet restore
WORKDIR /src/ProjectArena.Tests
RUN dotnet restore

# Dotnet testing
FROM dotnetBuild AS dotnetTesting
WORKDIR /src/ProjectArena.Api
RUN dotnet build
WORKDIR /src/ProjectArena.Tests
RUN dotnet test

# Dotnet publish
FROM dotnetBuild AS dotnetPublish
WORKDIR /src/ProjectArena.Api
RUN dotnet publish -c Release -o /src/publish

# Angular copy
WORKDIR /src/ProjectArena.WebClient
COPY dist/* /src/public/wwwroot/

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=publish /src/publish .
CMD ASPNETCORE_URLS=https://*:$PORT dotnet ProjectArena.Api.dll