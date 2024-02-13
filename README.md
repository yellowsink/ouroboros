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

// TODO: write once code is more settled