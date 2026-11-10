# Build context is the repository root
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/BookingWeb/BookingWeb.csproj src/BookingWeb/
RUN dotnet restore src/BookingWeb/BookingWeb.csproj

COPY src/BookingWeb/ src/BookingWeb/
RUN dotnet publish src/BookingWeb/BookingWeb.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "BookingWeb.dll"]
