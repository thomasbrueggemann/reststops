#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY Reststops.Presentation.API/Reststops.Presentation.API.csproj Reststops.Presentation.API/
COPY Reststops.Infrastructure.Data/Reststops.Infrastructure.csproj Reststops.Infrastructure.Data/
COPY Reststops.Core/Reststops.Core.csproj Reststops.Core/
RUN dotnet restore "Reststops.Presentation.API/Reststops.Presentation.API.csproj"
COPY . .
WORKDIR "/src/Reststops.Presentation.API"
RUN dotnet build "Reststops.Presentation.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Reststops.Presentation.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reststops.Presentation.API.dll"]
