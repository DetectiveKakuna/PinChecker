FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG BUILD_CONFIGURATION=Release

# Copy csproj and restore as distinct layers
COPY ["PinChecker/PinChecker.csproj", "PinChecker/"]
RUN dotnet restore "PinChecker/PinChecker.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/PinChecker"
RUN dotnet publish "PinChecker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app

# For non-root user security best practice (with fallback if not set)
ARG APP_UID=1000
USER $APP_UID

# Set environment
ENV ASPNETCORE_ENVIRONMENT=local

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PinChecker.dll"]