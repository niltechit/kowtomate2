# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["kowtomate/Presentation/KowToMateAdmin/KowToMateAdmin.csproj", "kowtomate/Presentation/KowToMateAdmin/"]
COPY ["kowtomate/Libraries/CutOutWiz.Core/CutOutWiz.Core.csproj", "kowtomate/Libraries/CutOutWiz.Core/"]
COPY ["kowtomate/Libraries/CutOutWiz.Data/CutOutWiz.Data.csproj", "kowtomate/Libraries/CutOutWiz.Data/"]
COPY ["kowtomate/Libraries/CutOutWiz.Services/CutOutWiz.Services.csproj", "kowtomate/Libraries/CutOutWiz.Services/"]
RUN dotnet restore "./kowtomate/Presentation/KowToMateAdmin/KowToMateAdmin.csproj"
COPY . .
WORKDIR "/src/kowtomate/Presentation/KowToMateAdmin"
RUN dotnet build "./KowToMateAdmin.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KowToMateAdmin.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KowToMateAdmin.dll"]
