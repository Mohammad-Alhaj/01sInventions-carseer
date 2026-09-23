FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first so this layer is cached when only source files change.
COPY inventions-carseer/inventions-carseer.csproj inventions-carseer/
RUN dotnet restore inventions-carseer/inventions-carseer.csproj

COPY . .
RUN dotnet publish inventions-carseer/inventions-carseer.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# TLS is terminated in front of the container (ALB / CloudFront / nginx),
# so Kestrel serves plain HTTP and the app must not redirect to https.
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "inventions-carseer.dll"]
