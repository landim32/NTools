# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["NTools.API/NTools.API.csproj", "NTools.API/"]
COPY ["NTools.Application/NTools.Application.csproj", "NTools.Application/"]
COPY ["NTools.ACL/NTools.ACL.csproj", "NTools.ACL/"]
COPY ["NTools.Domain/NTools.Domain.csproj", "NTools.Domain/"]
COPY ["NTools.DTO/NTools.DTO.csproj", "NTools.DTO/"]

RUN dotnet restore "NTools.API/NTools.API.csproj"

# Copy only necessary source code (exclude tests, docs, and sensitive files)
COPY ["NTools.API/", "NTools.API/"]
COPY ["NTools.Application/", "NTools.Application/"]
COPY ["NTools.ACL/", "NTools.ACL/"]
COPY ["NTools.Domain/", "NTools.Domain/"]
COPY ["NTools.DTO/", "NTools.DTO/"]

# Build the application (disable package generation for Docker build)
WORKDIR "/src/NTools.API"
RUN dotnet build "NTools.API.csproj" -c Release -o /app/build /p:GeneratePackageOnBuild=false

# Publish stage
FROM build AS publish
RUN dotnet publish "NTools.API.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:GeneratePackageOnBuild=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' --uid 1000 appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NTools.API.dll"]
