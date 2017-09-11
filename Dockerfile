FROM microsoft/dotnet:2.0.0-sdk AS builder
WORKDIR /source
COPY src/log.csproj .
RUN dotnet restore

COPY src/. .
RUN dotnet publish --output /log/ --configuration Release

FROM microsoft/aspnetcore:2.0.0
WORKDIR /log
COPY --from=builder /log .
EXPOSE 5000
ENTRYPOINT ["dotnet", "log.dll"]