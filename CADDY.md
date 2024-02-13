## Exposing Ouroboros on the same port as headscale

This is necessary to make interactive auth work, as it lets ouroboros intercept the
`/register/` route used for interactive machine setup.

Your ouroboros server should be served behind a proxy like this to make interactive auth work:
```caddyfile
domain.com {
    # use your own host:post combos here,
    # eg in a docker setup this may be like headscale:8080
    
    reverse_proxy /ouroboros/* localhost:42069 # ouroboros
    reverse_proxy /register/*  localhost:42069
    reverse_proxy              localhost:27896 # headscale
}
```

## Exposing gRPC on the same port

If your headscale server is on a remote server, here's how to setup your configs to get
it to work nicely.

NOTE: this assumes your headscale is in an isolated network or behind a firewall,
such that your listen and grpc ports are not publicly accessible. If this is true,
such as with docker or with a correctly configured firewall, this config works nicely.
If not, it's horribly insecure and bad.

```caddyfile
# Caddyfile

domain.com {
    @grpc protocol grpc
    handle @grpc {
        reverse_proxy h2c://headscaleserver.com:50433 # use your port here
    }
    reverse_proxy headscaleserver.com:8080 # use your port here
}
```

```yml
# headscale config.yml
listen_addr: 0.0.0.0:8080 # whatever port you want, just make it match

grpc_listen_addr: 0.0.0.0:50433 # as above

# if your port is exposed this is a horrible idea
# we are just enabling insecure traffic between headscale and caddy
# as caddy will then apply security between it and ouroboros
# therefore as long as 50433 is hidden by a firewall, container, or similar, you're safe
grpc_allow_insecure: true
```

Then you just need to setup your Ouroboros config to match, so it knows how to find headscale:
```json
{
  "hs_is_remote": true,
  "hs_address": "headscale.mydomain.com:443",
  "hs_api_key": "lkCh3TBkhQ.x2JtgfqbpuGDxCt0koXDtr9DGe3Bg-UZ-WO_xxxxxxx"
}
```