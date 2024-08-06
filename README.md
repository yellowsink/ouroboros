# Ouroboros

Ouroboros is a small UI on top of [headscale](https://github.com/juanfont/headscale), which takes a pragmatic approach.

## Goals
- Allow users to fully control and manage their own devices
- Allow users to add devices interactively themselves, instead of the server admin having to run a command
- Users should have to authenticate first, using a Github account

## Non-goals
- Managing server settings - for things the sysadmin only should be allowed to do, they can use the CLI
- Managing users - users must be setup manually and mapped to github accounts manually, OAuth is only for verification,
  not for signups

## Setup, usage and config

Go to https://github.com/settings/apps and create an app.
- Give it a sensible name and homepage
- Set the callback url to `https://your.server.com/ouroboros/auth/callback`
- The app requires *no* permissions
- Create a client secret and keep it safe

Create your config file cfg.json:
```json
{
  "hs_is_remote": true,
  "hs_address": "your.server.com:443",
  "hs_api_key": "FnxEEt2e4A.etc",
  "hs_bin_path": "/usr/bin/headscale",
  "hs_login_url": "your.server.com",
  "gh_client_id": "Iv1.etc",
  "gh_client_secret": "8e0f9-etc",
  "user_map": {
    "19270622": "sink",
    "00000000": "jim"
  }
}
```

| option           | default               | purpose                                            |
|------------------|-----------------------|----------------------------------------------------|
| hs_is_remote     | `false`               | sets if the headscale server is on a separate host |
| hs_address       | required if is_remote | the host and port used to connect to headscale     |
| hs_api_key       | required if is_remote | the api key used to connect to headscale           |
| hs_bin_path      | `headscale`           | the headscale binary path to use                   |
| hs_login_url     | required              | the login url used by the node clients             |
| gh_client_id     | required              | the github app oauth2 client id                    |
| gh_client_secret | required              | the github app oauth2 client secret                |
| user_map         | required              | map of github user ids to headscale usernames      |

Get headscale running via any means of your choice (I'm partial to docker), and get it running and exposed to the internet.
Ouroboros only needs to bind on TWO paths:
- `/ouroboros/*`
- `/register/*`

Any other paths, most importantly the ones used by headscale itself! are passed through to hs fine.

Here's a caddy config that does this:
```caddyfile
your.server.com {
    @grpc protocol grpc
    
    handle @grpc {
        reverse_proxy h2c://headscale:50443
    }
    
    reverse_poxy /ouroboros/* ouroboros:5000
    reverse_poxy /register/* ouroboros:5000
    
    reverse_proxy headscale:8080
}
```

Done!

## Docker

Create a container with environment variables like this:
```yml
services:
  ouroboros:
    image: yellosink/ouroboros:0.1.0
    ports: ["8080:5000"]
    environment:
    - HS_IS_REMOTE=true
    - HS_ADDRESS=my.server.com:443
    - HS_API_KEY=mysecretkey
    - HS_LOGIN_URL=my.server.com
    - GH_CLIENT_ID=myid
    - GH_CLIENT_SECRET=secret
    - 'USER_MAP={ "19270622": "sink" }'
```