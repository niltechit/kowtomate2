# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Presentation/KowToMateAdmin/KowToMateAdmin.csproj", "Presentation/KowToMateAdmin/"]
COPY ["Libraries/CutOutWiz.Core/CutOutWiz.Core.csproj", "Libraries/CutOutWiz.Core/"]
COPY ["Libraries/CutOutWiz.Data/CutOutWiz.Data.csproj", "Libraries/CutOutWiz.Data/"]
COPY ["Libraries/CutOutWiz.Services/CutOutWiz.Services.csproj", "Libraries/CutOutWiz.Services/"]

# Restore dependencies
RUN dotnet restore "./Presentation/KowToMateAdmin/KowToMateAdmin.csproj"

# Copy all source files
COPY . .

# Build and publish separately with memory-optimized settings
WORKDIR "/src/Presentation/KowToMateAdmin"
RUN dotnet build "./KowToMateAdmin.csproj" -c $BUILD_CONFIGURATION -maxcpucount:1 --no-restore -o /app/build
RUN dotnet publish "./KowToMateAdmin.csproj" -c $BUILD_CONFIGURATION -maxcpucount:1 --no-build -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "KowToMateAdmin.dll"]
