# ASP.NET Core build
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Services/Auth/CareManagement.Auth.Api/CareManagement.Auth.Api.csproj", "src/Services/Auth/CareManagement.Auth.Api/"]
COPY ["src/Shared/CareManagement.Shared/CareManagement.Shared.csproj", "src/Shared/CareManagement.Shared/"]
RUN dotnet restore "src/Services/Auth/CareManagement.Auth.Api/CareManagement.Auth.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Services/Auth/CareManagement.Auth.Api"
RUN dotnet build "CareManagement.Auth.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CareManagement.Auth.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CareManagement.Auth.Api.dll"]
