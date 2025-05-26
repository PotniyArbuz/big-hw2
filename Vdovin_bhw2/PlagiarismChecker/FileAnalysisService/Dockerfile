FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8082
ENV ASPNETCORE_URLS=http://+:8082


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY FileAnalysisService.csproj .
RUN dotnet restore FileAnalysisService.csproj
COPY . .
WORKDIR /src
RUN dotnet build FileAnalysisService.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish FileAnalysisService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileAnalysisService.dll"]