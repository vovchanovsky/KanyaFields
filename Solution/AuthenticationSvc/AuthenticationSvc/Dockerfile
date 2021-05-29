#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AuthenticationSvc/AuthenticationSvc/AuthenticationSvc.csproj", "AuthenticationSvc/AuthenticationSvc/"]
RUN dotnet restore "AuthenticationSvc/AuthenticationSvc/AuthenticationSvc.csproj"
COPY . .
WORKDIR "/src/AuthenticationSvc/AuthenticationSvc"
RUN dotnet build "AuthenticationSvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthenticationSvc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationSvc.dll"]