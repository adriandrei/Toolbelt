FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Toolbelt.ApiTemplate/Toolbelt.ApiTemplate.csproj", "src/Toolbelt.ApiTemplate/"]
COPY ["src/Toolbelt.Abstractions/Toolbelt.Abstractions.csproj", "src/Toolbelt.Abstractions/"]
COPY ["src/Toolbelt.Cosmos/Toolbelt.Cosmos.csproj", "src/Toolbelt.Cosmos/"]
COPY ["src/Toolbelt.Shared/Toolbelt.Shared.csproj", "src/Toolbelt.Shared/"]
RUN dotnet restore "src/Toolbelt.ApiTemplate/Toolbelt.ApiTemplate.csproj"
COPY . .
WORKDIR "/src/src/Toolbelt.ApiTemplate"
RUN dotnet build "Toolbelt.ApiTemplate.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Toolbelt.ApiTemplate.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Toolbelt.ApiTemplate.dll"]