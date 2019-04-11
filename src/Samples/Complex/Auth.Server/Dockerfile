FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Samples/Complex/Auth.Server/Auth.Server.csproj Samples/Complex/Auth.Server/
COPY ../../jimu.miniddd.demo/src/microservices/Auth/Auth.IService/Auth.IService.csproj ../../jimu.miniddd.demo/src/microservices/Auth/Auth.IService/
COPY Jimu.Modules/Jimu.Server.Discovery.ConsulIntegration/Jimu.Server.Discovery.ConsulIntegration.csproj Jimu.Modules/Jimu.Server.Discovery.ConsulIntegration/
COPY Jimu.Server/Jimu.Server.csproj Jimu.Server/
COPY Jimu/Jimu.csproj Jimu/
COPY ../../jimu.miniddd.demo/src/microservices/Auth/Auth.Service/Auth.Service.csproj ../../jimu.miniddd.demo/src/microservices/Auth/Auth.Service/
COPY Jimu.Modules/Jimu.Server.ORM.DapperIntegration/Jimu.Server.ORM.DapperIntegration.csproj Jimu.Modules/Jimu.Server.ORM.DapperIntegration/
RUN dotnet restore Samples/Complex/Auth.Server/Auth.Server.csproj
COPY . .
WORKDIR /src/Samples/Complex/Auth.Server
RUN dotnet build Auth.Server.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Auth.Server.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Auth.Server.dll"]
