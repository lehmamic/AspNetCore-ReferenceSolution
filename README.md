# Reference Solution

## AspNet Core Backend

### Features

* Correlation Id Logging
* HttpClient Logging Extension (e.g. Correlation Id)
* Serilog
* Rebus Initialization
* Automapper
* Fluent Validation
* Example Controller with Example HttpClient Api
* HttpClient Factory mit Refit and Polly
* DB Migration Tool

## Angular Frontend

### Create Angular Client Project

```bash
ng new demo-frontend --routing --skipGit --style=scss
```

### Features

* Proxy config to backend on port 5000
* TSLint Rules
* Correlation Id Interceptor (always adds a correlation id to the requests)
* Date Http Interceptor (make sure Date properties are Date objects)
* Translation with ngx-translate
* Lazy loading modules
* NgRx initialization

## Build

The local and ci build is done with [nuke build](https://nuke.build).

### Install nuke build

```bash
dotnet tool install Nuke.GlobalTool --global
```

### Setup nuke build

```bash
```

### Features

* Build, Lint, test Docker Image with Back- and Frontend
* Setup Databases

## Infrastructure

### Features

* Example EFK Stack for Docker Swarm
* Example Traefik for Docker Swarm
* Example Portainer for Docker Swarm

## Documentation

### Features

* Example folder structure (from Simon Brown)
* Architecture decision record template
