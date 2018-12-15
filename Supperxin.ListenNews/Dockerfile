FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Supperxin.ListenNews/Supperxin.ListenNews.csproj", "Supperxin.ListenNews/"]
RUN dotnet restore "Supperxin.ListenNews/Supperxin.ListenNews.csproj"
COPY . .
WORKDIR "/src/Supperxin.ListenNews"
RUN dotnet build "Supperxin.ListenNews.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Supperxin.ListenNews.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Supperxin.ListenNews.dll"]