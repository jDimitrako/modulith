FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Worker/Modulith.Worker.csproj", "Worker/"]
COPY ["SharedKernel/Modulith.SharedKernel/Modulith.SharedKernel.csproj", "SharedKernel/Modulith.SharedKernel/"]
COPY ["NewModule/Modulith.NewModule.Infrastructure/Modulith.NewModule.Infrastructure.csproj", "NewModule/Modulith.NewModule.Infrastructure/"]
COPY ["NewModule/Modulith.NewModule.Application/Modulith.NewModule.Application.csproj", "NewModule/Modulith.NewModule.Application/"]
COPY ["NewModule/Modulith.NewModule.Domain/Modulith.NewModule.Domain.csproj", "NewModule/Modulith.NewModule.Domain/"]
RUN dotnet restore "Worker/Modulith.Worker.csproj"
COPY . .
WORKDIR "/src/Worker"
RUN dotnet build "Modulith.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Modulith.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Modulith.Worker.dll"] 