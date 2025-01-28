# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Export the port
EXPOSE 7700

# Copy project files
COPY Colir.Exceptions/*.csproj Colir.Exceptions/
COPY Colir.DAL/*.csproj Colir.DAL/
COPY Colir.BLL/*.csproj Colir.BLL/
COPY Colir.WebApi/*.csproj Colir.WebApi/

# Restore dependencies
RUN dotnet restore Colir.WebApi/Colir.WebApi.csproj

# Install EF CLI
RUN dotnet tool install --global dotnet-ef --version 6.0.25
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copy remaining files and publish the application
COPY . .
RUN dotnet publish "Colir.WebApi/Colir.WebApi.csproj" -c Release -o /publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS final
WORKDIR /app

# Copy published output
COPY --from=build /publish .

# Expose application port
ENV ASPNETCORE_HTTP_PORTS=7700

# Entry point for the application
ENTRYPOINT ["dotnet", "Colir.WebApi.dll"]