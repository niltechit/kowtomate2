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
COPY ["kowtomate/Presentation/KowToMateAdmin/KowToMateAdmin.csproj", "kowtomate/Presentation/KowToMateAdmin/"]
COPY ["kowtomate/Libraries/CutOutWiz.Core/CutOutWiz.Core.csproj", "kowtomate/Libraries/CutOutWiz.Core/"]
COPY ["kowtomate/Libraries/CutOutWiz.Data/CutOutWiz.Data.csproj", "kowtomate/Libraries/CutOutWiz.Data/"]
COPY ["kowtomate/Libraries/CutOutWiz.Services/CutOutWiz.Services.csproj", "kowtomate/Libraries/CutOutWiz.Services/"]

# Restore dependencies
RUN dotnet restore "./kowtomate/Presentation/KowToMateAdmin/KowToMateAdmin.csproj"

# Copy all source files
COPY . .

# Build and publish separately with memory-optimized settings
WORKDIR "/src/kowtomate/Presentation/KowToMateAdmin"
RUN dotnet build "./KowToMateAdmin.csproj" -c $BUILD_CONFIGURATION -maxcpucount:1 --no-restore -o /app/build
RUN dotnet publish "./KowToMateAdmin.csproj" -c $BUILD_CONFIGURATION -maxcpucount:1 --no-build -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "KowToMateAdmin.dll"]
