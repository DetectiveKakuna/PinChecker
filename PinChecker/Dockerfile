FROM mcr.microsoft.com/playwright/dotnet:v1.51.0-noble

WORKDIR /app

# Copy everything from the current directory
COPY . .

# Set environment variable for Playwright browsers
ENV PLAYWRIGHT_BROWSERS_PATH=/ms-playwright

# Install the browsers explicitly using the playwright CLI
RUN apt-get update && apt-get install -y curl unzip && \
    curl -sL https://playwright.azureedge.net/builds/chromium/1161/chromium-linux.zip -o /tmp/chromium.zip && \
    mkdir -p /root/.cache/ms-playwright/chromium_headless_shell-1161/chrome-linux && \
    unzip /tmp/chromium.zip -d /root/.cache/ms-playwright/chromium_headless_shell-1161/chrome-linux && \
    # Create symbolic link from chrome-linux/chrome-linux/chrome to the expected headless_shell location
    ln -s /root/.cache/ms-playwright/chromium_headless_shell-1161/chrome-linux/chrome-linux/chrome /root/.cache/ms-playwright/chromium_headless_shell-1161/chrome-linux/headless_shell && \
    chmod +x /root/.cache/ms-playwright/chromium_headless_shell-1161/chrome-linux/chrome-linux/chrome && \
    chmod +x /root/.cache/ms-playwright/chromium_headless_shell-1161/chrome-linux/headless_shell && \
    rm /tmp/chromium.zip

# Restore and publish the application
RUN dotnet restore && \
    dotnet publish -c Release -o /app/publish --runtime linux-x64 --self-contained false

# Set the working directory to the publish directory
WORKDIR /app/publish

# Install cron
RUN apt-get update && apt-get -y install cron

# Create a crontab file to run the app every minute
RUN echo "* * * * * cd /app/publish && dotnet PinChecker.dll >> /var/log/cron.log 2>&1" > /etc/cron.d/pin-checker-cron
RUN chmod 0644 /etc/cron.d/pin-checker-cron

# Apply cron job
RUN crontab /etc/cron.d/pin-checker-cron

# Create log file
RUN touch /var/log/cron.log

# Run cron in the foreground
CMD ["sh", "-c", "cron && tail -f /var/log/cron.log"]