FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
ENV ASPNETCORE_URLS=http://*:${PORT}
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /app/appsettings.GCloud.json ./appsettings.json
ENTRYPOINT ["dotnet", "FinancialTrackerApi.dll"]
