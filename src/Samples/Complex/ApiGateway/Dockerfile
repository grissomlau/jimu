FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Samples/Complex/ApiGateway/ApiGateway.csproj Samples/Complex/ApiGateway/
COPY Jimu.Client/Jimu.Client.csproj Jimu.Client/
COPY Jimu/Jimu.csproj Jimu/
COPY Jimu.Modules/Jimu.Client.ApiGateway.SwaggerIntegration/Jimu.Client.ApiGateway.SwaggerIntegration.csproj Jimu.Modules/Jimu.Client.ApiGateway.SwaggerIntegration/
COPY Jimu.Modules/Jimu.Client.Discovery.ConsulIntegration/Jimu.Client.Discovery.ConsulIntegration.csproj Jimu.Modules/Jimu.Client.Discovery.ConsulIntegration/
RUN dotnet restore Samples/Complex/ApiGateway/ApiGateway.csproj
COPY . .
WORKDIR /src/Samples/Complex/ApiGateway
RUN dotnet build ApiGateway.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ApiGateway.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
