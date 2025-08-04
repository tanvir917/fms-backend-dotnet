# ASP.NET Core build
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Services/Staff/CareManagement.Staff.Api/CareManagement.Staff.Api.csproj", "src/Services/Staff/CareManagement.Staff.Api/"]
COPY ["src/Shared/CareManagement.Shared/CareManagement.Shared.csproj", "src/Shared/CareManagement.Shared/"]
RUN dotnet restore "src/Services/Staff/CareManagement.Staff.Api/CareManagement.Staff.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Services/Staff/CareManagement.Staff.Api"
RUN dotnet build "CareManagement.Staff.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CareManagement.Staff.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CareManagement.Staff.Api.dll"]
