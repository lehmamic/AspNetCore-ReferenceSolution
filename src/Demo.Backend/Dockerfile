FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM base AS final
WORKDIR /app
COPY publish/demo/demo.backend .
COPY publish/demo/demo.frontend ./wwwroot
ENTRYPOINT ["dotnet", "Demo.Backend.dll"]
