FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Modulith.NewModule.Api/Modulith.NewModule.Api.csproj"
RUN dotnet publish "Modulith.NewModule.Api/Modulith.NewModule.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Modulith.NewModule.Api.dll"] 