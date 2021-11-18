FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SeaBattle.Web/SeaBattle.Web.csproj", "SeaBattle.Web/"]
RUN dotnet restore "SeaBattle.Web/SeaBattle.Web.csproj"
COPY . .
WORKDIR "/src/SeaBattle.Web"
RUN dotnet build "SeaBattle.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SeaBattle.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SeaBattle.Web.dll"]
