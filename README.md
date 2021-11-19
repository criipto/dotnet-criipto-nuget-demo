# Criipto Sample

## Getting started

### Installing build tools

All tools required are part of the tools manifest file so all you have to do is 

```console
dotnet tool restore
```

### Installing dependencies

With the tools installed run the following in a terminal

```console
dotnet paket update
```

## Running the application

### VS Code and Visual studio

If you are using VS Code there's already a launch configuration, so you should be able to start the site from the debug console (F5)
Same goes for Visual Studio pressing F5 will start the application

### Docker
There's a Dockerfile included in the example. The docker build uses the release build and will therefore look for a `appSettings.Production.json`. This however is not included since by design it shouldn't be committed to VCS. Because of this you will need to mount a settings file
If you are just look to spin the container up you can use the `appSettings.Development.json`. Do not so this for production environments though.

```console
docker build -t app .
docker run  -p 8080:8080 --mount type=bind,source="$(pwd)"/src/appsettings.Development.json,target=/app/appsettings.Production.json -it app
```