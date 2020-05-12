# Reference Solution

## AspNet Core Backend

## Angular Frontend

### Create Angular Client Project

```bash
ng new demo-frontend --routing --skipGit --style=scss
```

### Features

* Proxy config to backend on port 5000
* Correlation Id Interceptor (always adds a correlation id to the requests)
* Date Http Interceptor (make sure Date properties are Date objects)
* Translation with ngx-translate

## Build

The local and ci build is done with [nuke build](https://nuke.build).

### Install nuke build

```bash
dotnet tool install Nuke.GlobalTool --global
```

### Setup nuke build

```bash
```