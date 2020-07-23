#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["TimeTracker.Api/TimeTracker.Api.fsproj", "TimeTracker.Api/"]
COPY ["TimeTracker.Domain/TimeTracker.Domain.fsproj", "TimeTracker.Domain/"]
COPY ["TimeTracker.Common/TimeTracker.Common.fsproj", "TimeTracker.Common/"]
COPY ["TimeTracker.Services/TimeTracker.Services.fsproj", "TimeTracker.Services/"]
COPY ["TimeTracker.Data/TimeTracker.Data.fsproj", "TimeTracker.Data/"]
RUN dotnet restore "TimeTracker.Api/TimeTracker.Api.fsproj"
COPY . .
WORKDIR "/src/TimeTracker.Api"
RUN dotnet build "TimeTracker.Api.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TimeTracker.Api.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeTracker.Api.dll"]