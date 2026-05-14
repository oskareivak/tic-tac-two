FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY tic-tac-two.sln ./
COPY Directory.Build.props ./
COPY Common/Common.csproj Common/
COPY ConsoleApp/ConsoleApp.csproj ConsoleApp/
COPY ConsoleUI/ConsoleUI.csproj ConsoleUI/
COPY DAL/DAL.csproj DAL/
COPY Domain/Domain.csproj Domain/
COPY GameBrain/GameBrain.csproj GameBrain/
COPY MenuSystem/MenuSystem.csproj MenuSystem/
COPY WebApp/WebApp.csproj WebApp/

RUN dotnet restore WebApp/WebApp.csproj

COPY . .

WORKDIR /src/WebApp
RUN dotnet publish -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim-amd64 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "WebApp.dll"]
