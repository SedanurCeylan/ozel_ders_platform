FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY OzelDersYonetim.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release --no-restore -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN mkdir -p /data && chown "$APP_UID:$APP_UID" /data
COPY --from=build --chown=$APP_UID:$APP_UID /app/publish .
USER $APP_UID
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    ConnectionStrings__DefaultConnection="Data Source=/data/app.db;Cache=Shared" \
    Storage__RootPath=/data/uploads
EXPOSE 8080
ENTRYPOINT ["dotnet", "OzelDersYonetim.dll"]
