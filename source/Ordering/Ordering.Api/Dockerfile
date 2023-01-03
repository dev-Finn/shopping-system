FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy Source
COPY ["Ordering/Ordering.Api/", "Ordering/Ordering.Api/"]
COPY ["Ordering/Ordering.Components/", "Ordering/Ordering.Components/"]
COPY ["Shared/", "Shared/"]

# Copy Contracts
COPY ["Ordering/Ordering.Contracts/", "Ordering/Ordering.Contracts/"]
COPY ["Payment/Payment.Contracts/", "Payment/Payment.Contracts/"]
COPY ["Warehouse/Warehouse.Contracts/", "Warehouse/Warehouse.Contracts/"]

WORKDIR "/src/Ordering/Ordering.Api"
RUN ls
RUN dotnet build "Ordering.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ordering.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ordering.Api.dll"]
