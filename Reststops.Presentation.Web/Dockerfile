#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY Reststops.Presentation.Web/Reststops.Presentation.Web.csproj Reststops.Presentation.Web/
COPY Reststops.Infrastructure.Data/Reststops.Infrastructure.csproj Reststops.Infrastructure.Data/
COPY Reststops.Core/Reststops.Core.csproj Reststops.Core/
RUN dotnet restore "Reststops.Presentation.Web/Reststops.Presentation.Web.csproj"
COPY . .
WORKDIR "/src/Reststops.Presentation.Web"
RUN dotnet build "Reststops.Presentation.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Reststops.Presentation.Web.csproj" -c Release -o /app/publish -f netstandard2.1

FROM nginx:alpine
COPY --from=publish /app/publish/wwwroot /usr/share/nginx/html/
COPY Reststops.Presentation.Web/nginx.conf /etc/nginx/nginx.conf
