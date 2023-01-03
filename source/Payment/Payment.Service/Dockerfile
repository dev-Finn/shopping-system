FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy Source
COPY ["Payment/Payment.Service/", "Payment/Payment.Service/"]
COPY ["Payment/Payment.Components/", "Payment/Payment.Components/"]
COPY ["Shared/", "Shared/"]

# Copy Contracts
COPY ["Ordering/Ordering.Contracts/", "Ordering/Ordering.Contracts/"]
COPY ["Payment/Payment.Contracts/", "Payment/Payment.Contracts/"]

WORKDIR "/src/Payment/Payment.Service"
RUN dotnet build "Payment.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Payment.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.Service.dll"]
