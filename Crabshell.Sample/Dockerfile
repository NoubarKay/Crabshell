FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Crabshell.Sample/Crabshell.Sample.csproj", "Crabshell.Sample/"]
COPY ["Crabshell.Core/Crabshell.Core.csproj", "Crabshell.Core/"]
COPY ["Crabshell.Data/Crabshell.Data.csproj", "Crabshell.Data/"]
COPY ["Crabshell.SourceGenerator/Crabshell.SourceGenerator.csproj", "Crabshell.SourceGenerator/"]
COPY ["Crabshell.Admin/Crabshell.Admin.csproj", "Crabshell.Admin/"]
RUN dotnet restore "Crabshell.Sample/Crabshell.Sample.csproj"
COPY . .
WORKDIR "/src/Crabshell.Sample"
RUN dotnet build "./Crabshell.Sample.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Crabshell.Sample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Crabshell.Sample.dll"]
