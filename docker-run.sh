#!/bin/sh

# script used for starting ouroboros in headscale containers
# create cfg
cp cfg.docker.json cfg.json

# set things
sed -i 's/"<DOCKER_IS_REMOTE>"/'$HS_IS_REMOTE'/' cfg.json
sed -i 's/<DOCKER_ADDRESS>/'$HS_ADDRESS'/' cfg.json
sed -i 's/<DOCKER_API_KEY>/'$HS_API_KEY'/' cfg.json
sed -i 's/<DOCKER_LOGIN_URL>/'$HS_LOGIN_URL'/' cfg.json
sed -i 's/<DOCKER_CLIENT_ID>/'$GH_CLIENT_ID'/' cfg.json
sed -i 's/<DOCKER_CLIENT_SECRET>/'$GH_CLIENT_SECRET'/' cfg.json
sed -i 's/"<DOCKER_USER_MAP>"/'"$USER_MAP"'/' cfg.json

# run ouroboros
# using exec makes sure we are correctly passing through SIGTERMs
exec dotnet Ouroboros.dll