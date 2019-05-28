FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine AS builder
WORKDIR /source
COPY src/log.csproj .
RUN dotnet restore

COPY src/. .
RUN dotnet publish --output /log/ --configuration Release

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-runtime
WORKDIR /log
COPY --from=builder /log .
EXPOSE 5000
ENTRYPOINT ["dotnet", "log.dll"]