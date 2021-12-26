FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY LemonSharp.User.API/LemonSharp.User.API.csproj LemonSharp.User.API/
RUN dotnet restore LemonSharp.User.API/LemonSharp.User.API.csproj

# copy everything and build
COPY . .

WORKDIR /app/LemonSharp.User.API
RUN dotnet publish -c Release -o out

# build runtime image
FROM base AS final
WORKDIR /app
COPY --from=build-env /app/LemonSharp.User.API/out .
ENTRYPOINT ["dotnet", "LemonSharp.User.API.dll"]
