#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_ENVIRONMENT=Docker
WORKDIR /app
EXPOSE 80

# Build libgdiplus with pango
RUN apt-get update && apt-get install  -y --allow-unauthenticated \
     libgif-dev autoconf libtool automake build-essential gettext libglib2.0-dev libcairo2-dev libtiff-dev libexif-dev
RUN apt-get update && apt-get install -y libpango1.0-dev 

RUN apt-get install -y --allow-unauthenticated git
RUN git clone https://github.com/mono/libgdiplus.git /libgdiplus
WORKDIR /libgdiplus
RUN ./autogen.sh --with-pango --prefix=/usr
RUN make  
RUN make install

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
#COPY ["NTools.API/emagine.pfx", "NTools.API/"]
COPY ["NTools.API/NTools.API.csproj", "NTools.API/"]
COPY ["NTools.Application/NTools.Application.csproj", "NTools.Application/"]
COPY ["NTools.Domain/NTools.Domain.csproj", "NTools.Domain/"]
COPY ["Core.Domain/Core.Domain.csproj", "Core.Domain/"]
COPY ["NTools.DTO/NTools.DTO.csproj", "NTools.DTO/"]
RUN dotnet restore "NTools.API/NTools.API.csproj"
COPY . .
WORKDIR "/src/NTools.API"
RUN dotnet build "NTools.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NTools.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NTools.API.dll"]