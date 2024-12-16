FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NHSv2.Appointments/src/NHSv2.Appointments.EventStoreWorker/NHSv2.Appointments.EventStoreWorker.csproj", "NHSv2.Appointments/src/NHSv2.Appointments.EventStoreWorker/"]
RUN dotnet restore "NHSv2.Appointments/src/NHSv2.Appointments.EventStoreWorker/NHSv2.Appointments.EventStoreWorker.csproj"
COPY . .
WORKDIR "/src/NHSv2.Appointments/src/NHSv2.Appointments.EventStoreWorker"
RUN dotnet build "NHSv2.Appointments.EventStoreWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSv2.Appointments.EventStoreWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NHSv2.Appointments.EventStoreWorker.dll"]
