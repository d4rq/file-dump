FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FileDump.Api.Web/FileDump.Api.Web.csproj", "FileDump.Api.Web/"]
RUN dotnet restore "FileDump.Api.Web/FileDump.Api.Web.csproj"
COPY . .
WORKDIR "/src/FileDump.Api.Web"
RUN dotnet build "./FileDump.Api.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FileDump.Api.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileDump.Api.Web.dll"]
