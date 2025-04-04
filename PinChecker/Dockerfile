FROM mcr.microsoft.com/playwright/dotnet:v1.51.0-noble

WORKDIR /app

# Copy project files
COPY PinChecker/. ./PinChecker/

# Set environment variable for Playwright browsers
ENV PLAYWRIGHT_BROWSERS_PATH=/ms-playwright

# Restore dependencies and publish
WORKDIR /app/PinChecker
RUN dotnet restore && \
    dotnet publish -c Release -o /app/publish --runtime linux-x64 --self-contained false

# Install Playwright browsers
RUN cd /app/publish && \
    pwsh -Command "& { \
        Import-Module ./Microsoft.Playwright.dll; \
        Install-Playwright -Force \
    }"

# Set the working directory to the publish directory
WORKDIR /app/publish

# Run the application
ENTRYPOINT ["dotnet", "PinChecker.dll"]