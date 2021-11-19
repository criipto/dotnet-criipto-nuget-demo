# https://hub.docker.com/_/microsoft-dotnet
ARG DOTNET_VERSION=5.0
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY paket.* .
COPY src/*.fsproj ./src/

COPY .config/dotnet-tools.json .config/
RUN dotnet tool restore
RUN dotnet paket update
RUN dotnet paket restore

# copy everything else and build app
COPY src/ ./src
WORKDIR /source/src
RUN dotnet publish -c release -o /app 

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}
WORKDIR /app
COPY --from=build /app ./
COPY --from=build /source/src/appsettings.json ./
ENTRYPOINT ["dotnet", "Criipto.NugetSample.dll"]

#docker run  -p 8080:8080 --mount type=bind,source=<absolute path to production appsettings file>target=/app/appsettings.Production.json -it <container tag>