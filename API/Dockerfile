FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443

# copy all .cproj files and restore as distinct layers. Use of the same COPY command
# for every dockerfile in the project to take advantage of docker caching
COPY skizz.sln skizz.sln
COPY API/API.csproj API/API.csproj
COPY Core/Core.csproj Core/Core.csproj
COPY Infrastructure/Infrastructure.csproj Infrastructure/Infrastructure.csproj

# restore package deps
RUN dotnet restore skizz.sln

# copy the app folders over
COPY API API
COPY Core Core
COPY Infrastructure Infrastructure
WORKDIR /app/API
RUN dotnet publish -c Release -o /app/src/out

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/src/out .
ENTRYPOINT [ "dotnet", "API.dll" ]
