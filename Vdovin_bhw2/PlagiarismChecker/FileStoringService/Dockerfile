FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY FileStoringService.csproj .
RUN dotnet restore FileStoringService.csproj
COPY . .
WORKDIR /src
RUN dotnet build FileStoringService.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish FileStoringService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileStoringService.dll"]