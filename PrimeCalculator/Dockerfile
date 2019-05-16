FROM mcr.microsoft.com/dotnet/core/runtime:2.1-stretch-slim AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["PrimeCalculator/PrimeCalculator.csproj", "PrimeCalculator/"]
RUN dotnet restore "PrimeCalculator/PrimeCalculator.csproj"
COPY . .
WORKDIR "/src/PrimeCalculator"
RUN dotnet build "PrimeCalculator.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "PrimeCalculator.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PrimeCalculator.dll"]