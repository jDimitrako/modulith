# Use the official .NET runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
# Restore and publish the main API project (replace with your main API project if needed)
RUN dotnet restore "Modulith.Web/Modulith.Web.csproj"
RUN dotnet publish "Modulith.Web/Modulith.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Modulith.Web.dll"] 