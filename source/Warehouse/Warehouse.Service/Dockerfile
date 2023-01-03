FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy Source
COPY ["Warehouse/Warehouse.Service/", "Warehouse/Warehouse.Service/"]
COPY ["Warehouse/Warehouse.Components/", "Warehouse/Warehouse.Components/"]
COPY ["Shared/", "Shared/"]

# Copy Contracts
COPY ["Ordering/Ordering.Contracts/", "Ordering/Ordering.Contracts/"]
COPY ["Warehouse/Warehouse.Contracts/", "Warehouse/Warehouse.Contracts/"]

WORKDIR "/src/Warehouse/Warehouse.Service"
RUN dotnet build "Warehouse.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Warehouse.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Warehouse.Service.dll"]
