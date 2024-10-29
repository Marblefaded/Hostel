#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Hostel/Suo.Admin.csproj", "Hostel/"]
COPY ["GlobalSpecialWords/GlobalSpecialWords.csproj", "GlobalSpecialWords/"]
COPY ["Hostel.TelegramBot/Suo.TelegramBot.csproj", "Suo.TelegramBot/"]
COPY ["Authorization.SingleService/Suo.Autorization.SingleService.csproj", "Authorization.SingleService/"]
COPY ["Authorization.SingleService.Core/Suo.Autorization.SingleService.Core.csproj", "Authorization.SingleService.Core/"]
COPY ["Authorization.SingleService.Infrastructure/Suo.Autorization.SingleService.Infrastructure.csproj", "Authorization.SingleService.Infrastructure/"]
COPY ["JWTCoding/JWTCoding.csproj", "JWTCoding/"]
COPY ["HostelDB/HostelDB.csproj", "HostelDB/"]
COPY ["NPOIPdfEngine/NPOIPdfEngine.csproj", "NPOIPdfEngine/"]
RUN dotnet restore "Hostel/Suo.Admin.csproj"
COPY . .
WORKDIR "/src/Hostel"
RUN dotnet build "Suo.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Suo.Admin.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN echo "ttf-mscorefonts-installer msttcorefonts/accepted-mscorefonts-eula select true" | debconf-set-selections
RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list
RUN apt update && apt install -y ttf-mscorefonts-installer fontconfig
RUN fc-cache -f -v
RUN echo "Asia/Novosibirsk" > /etc/timezone \
    && rm -f /etc/localtime \
    && dpkg-reconfigure -f noninteractive tzdata \
    && date
ENTRYPOINT ["dotnet", "Suo.Admin.dll"]