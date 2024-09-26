FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS ouro-build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# obtain headscale
RUN wget https://github.com/juanfont/headscale/releases/download/v0.23.0/headscale_0.23.0_linux_amd64
RUN chmod +x headscale_0.23.0_linux_amd64
RUN mv headscale_0.23.0_linux_amd64 out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff
WORKDIR /App
COPY --from=ouro-build-env /App/out .

EXPOSE 8080/tcp
ENTRYPOINT ["./docker-run.sh"]