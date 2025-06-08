FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Platform/Worker/Modulith.Platform.Worker.csproj", "Platform/Worker/"]
COPY ["SharedKernel/Modulith.SharedKernel.Infrastructure/Modulith.SharedKernel.Infrastructure.csproj", "SharedKernel/Modulith.SharedKernel.Infrastructure/"]
RUN dotnet restore "Platform/Worker/Modulith.Platform.Worker.csproj"
COPY . .
WORKDIR "/src/Platform/Worker"
RUN dotnet build "Modulith.Platform.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Modulith.Platform.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Modulith.Platform.Worker.dll"] 