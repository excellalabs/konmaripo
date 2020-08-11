#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build
WORKDIR /src
COPY ["Konmaripo.sln", "."]
COPY ["Konmaripo.Web/Konmaripo.Web.csproj", "./Konmaripo.Web/"]
COPY ["Konmaripo.Web.Tests.Unit/Konmaripo.Web.Tests.Unit.csproj", "./Konmaripo.Web.Tests.Unit/"]
RUN dotnet restore "Konmaripo.sln"
COPY . .

WORKDIR "/src"
RUN dotnet test "Konmaripo.sln"

WORKDIR "/src/Konmaripo.Web"
RUN dotnet build "Konmaripo.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Konmaripo.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV LD_DEBUG=all
ENTRYPOINT ["dotnet", "Konmaripo.Web.dll"]