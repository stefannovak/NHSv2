FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NHSv2.Communications/src/NHSv2.Communications.Worker/NHSv2.Communications.Worker.csproj", "NHSv2.Communications/src/NHSv2.Communications.Worker/"]
RUN dotnet restore "NHSv2.Communications/src/NHSv2.Communications.Worker/NHSv2.Communications.Worker.csproj"
COPY . .
WORKDIR "/src/NHSv2.Communications/src/NHSv2.Communications.Worker"
RUN dotnet build "NHSv2.Communications.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSv2.Communications.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NHSv2.Communications.Worker.dll"]
