#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Authorization.SingleService/Suo.Autorization.SingleService.csproj", "Authorization.SingleService/"]
COPY ["Authorization.SingleService.Core/Suo.Autorization.SingleService.Core.csproj", "Authorization.SingleService.Core/"]
COPY ["Authorization.SingleService.Infrastructure/Suo.Autorization.SingleService.Infrastructure.csproj", "Authorization.SingleService.Infrastructure/"]
COPY ["JWTCoding/JWTCoding.csproj", "JWTCoding/"]
COPY ["HostelDB/HostelDB.csproj", "HostelDB/"]
RUN dotnet restore "Authorization.SingleService/Suo.Autorization.SingleService.csproj"
COPY . .
WORKDIR "/src/Authorization.SingleService"
RUN dotnet build "Suo.Autorization.SingleService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Suo.Autorization.SingleService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN echo "Asia/Novosibirsk" > /etc/timezone \
    && rm -f /etc/localtime \
    && dpkg-reconfigure -f noninteractive tzdata \
    && date
ENTRYPOINT ["dotnet", "Suo.Autorization.SingleService.dll"]