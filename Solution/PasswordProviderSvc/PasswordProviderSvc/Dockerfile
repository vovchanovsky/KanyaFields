#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PasswordProviderSvc/PasswordProviderSvc/PasswordProviderSvc.csproj", "PasswordProviderSvc/PasswordProviderSvc/"]
COPY ["PasswordProviderSvc/PasswordProviderSvc.Infrastructure/PasswordProviderSvc.Infrastructure.csproj", "PasswordProviderSvc/PasswordProviderSvc.Infrastructure/"]
COPY ["Shared/Utilities/Utilities.csproj", "Shared/Utilities/"]
COPY ["Shared/KafkaInfrastructure/KafkaInfrastructure.csproj", "Shared/KafkaInfrastructure/"]
COPY ["PasswordProviderSvc/PasswordProviderSvc.InfrastructureInterfaces/PasswordProviderSvc.InfrastructureInterfaces.csproj", "PasswordProviderSvc/PasswordProviderSvc.InfrastructureInterfaces/"]
COPY ["PasswordProviderSvc/PasswordProviderSvc.Domain/PasswordProviderSvc.Domain.csproj", "PasswordProviderSvc/PasswordProviderSvc.Domain/"]
COPY ["PasswordProviderSvc/PasswordProviderSvc.Application/PasswordProviderSvc.Application.csproj", "PasswordProviderSvc/PasswordProviderSvc.Application/"]
RUN dotnet restore "PasswordProviderSvc/PasswordProviderSvc/PasswordProviderSvc.csproj"
COPY . .
WORKDIR "/src/PasswordProviderSvc/PasswordProviderSvc"
RUN dotnet build "PasswordProviderSvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PasswordProviderSvc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PasswordProviderSvc.dll"]