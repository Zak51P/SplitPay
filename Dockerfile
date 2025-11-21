FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SplitPay.sln", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["src/SplitPay.Api/SplitPay.Api.csproj", "src/SplitPay.Api/"]
COPY ["src/SplitPay.Application/SplitPay.Application.csproj", "src/SplitPay.Application/"]
COPY ["src/SplitPay.Infrastructure/SplitPay.Infrastructure.csproj", "src/SplitPay.Infrastructure/"]
COPY ["src/SplitPay.Domain/SplitPay.Domain.csproj", "src/SplitPay.Domain/"]

RUN dotnet restore "SplitPay.sln"

COPY . .

RUN dotnet publish "src/SplitPay.Api/SplitPay.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "SplitPay.Api.dll"]
