#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# NOTE: We use the "bionic" tag (Ubuntu 18.04) as the default "buster-slim" (Debian) is not supported by libgit2sharp as of August 2020.
FROM mcr.microsoft.com/dotnet/runtime:6.0-focal AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# NOTE: We use the "focal" tag (Ubuntu 20.04) as the default "buster-slim" (Debian) is not supported by libgit2sharp as of August 2020.
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
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

# Install tzdata for timezone purposes
RUN export DEBIAN_FRONTEND=noninteractive
RUN apt-get update
RUN apt-get install tzdata -y

# Do the rest of the app stuff
WORKDIR /app
COPY --from=publish /app/publish .
ENV LD_DEBUG=all
ENTRYPOINT ["dotnet", "Konmaripo.Web.dll"]