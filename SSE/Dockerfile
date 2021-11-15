#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SSE/SSE.csproj", "SSE/"]
RUN dotnet restore "SSE/SSE.csproj"
COPY . .
WORKDIR "/src/SSE"
RUN dotnet build "SSE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SSE.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SSE.dll"]