FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
RUN curl -sL https://deb.nodesource.com/setup_13.x |  bash -
RUN apt-get update
RUN apt-get install -y nodejs
COPY BenihanaWebReact.csproj ./
RUN dotnet restore "./BenihanaWebReact.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "BenihanaWebReact.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BenihanaWebReact.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BenihanaWebReact.dll"]
