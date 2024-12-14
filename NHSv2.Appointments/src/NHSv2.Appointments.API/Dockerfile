FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NHSv2.Appointments/NHSv2.Appointments.csproj", "NHSv2.Appointments/"]
RUN dotnet restore "NHSv2.Appointments/NHSv2.Appointments.csproj"
COPY . .
WORKDIR "/src/NHSv2.Appointments"
RUN dotnet build "NHSv2.Appointments.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NHSv2.Appointments.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NHSv2.Appointments.dll"]
