FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM node as builder
WORKDIR /src
COPY web .
RUN yarn config set strict-ssl false
RUN yarn
RUN npm i
RUN yarn run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/HongJun.Service/HongJun.Service.csproj", "src/HongJun.Service/"]
RUN dotnet restore "src/HongJun.Service/HongJun.Service.csproj"
COPY . .
WORKDIR "/src/src/HongJun.Service"
RUN dotnet build "HongJun.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HongJun.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=builder /src/dist ./wwwroot
ENTRYPOINT ["dotnet", "HongJun.Service.dll"]
