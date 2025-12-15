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

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/NTools.API"
RUN dotnet build "NTools.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "NTools.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NTools.API.dll"]
