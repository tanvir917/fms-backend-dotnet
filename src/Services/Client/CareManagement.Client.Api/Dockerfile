# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Services/Client/CareManagement.Client.Api/CareManagement.Client.Api.csproj", "src/Services/Client/CareManagement.Client.Api/"]
COPY ["src/Shared/CareManagement.Shared/CareManagement.Shared.csproj", "src/Shared/CareManagement.Shared/"]

RUN dotnet restore "src/Services/Client/CareManagement.Client.Api/CareManagement.Client.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Services/Client/CareManagement.Client.Api"
RUN dotnet build "CareManagement.Client.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CareManagement.Client.Api.csproj" -c Release -o /app/publish

# Use the official .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create uploads directory
RUN mkdir -p /app/uploads/documents

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "CareManagement.Client.Api.dll"]
