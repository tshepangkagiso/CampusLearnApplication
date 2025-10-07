# -----------------------
# Stage 1: Base runtime
# -----------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080
#EXPOSE 8081

# Enable globalization & performance
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8 \
    COMPlus_ReadyToRun=1 \
    COMPlus_TC_QuickJitForLoops=1

# -----------------------
# Stage 2: Build
# -----------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only project files first for caching
COPY ["PrivateMessagingMicroservice/CampusLearn.PrivateMessaging.API/CampusLearn.PrivateMessaging.API.csproj", "PrivateMessagingMicroservice/CampusLearn.PrivateMessaging.API/"]
COPY ["Library/CampusLearn.Library/CampusLearn.Code.Library.csproj", "Library/CampusLearn.Library/"]
RUN dotnet restore "./PrivateMessagingMicroservice/CampusLearn.PrivateMessaging.API/CampusLearn.PrivateMessaging.API.csproj"

# Copy the rest of the source
COPY . .

# Build project
WORKDIR "/src/PrivateMessagingMicroservice/CampusLearn.PrivateMessaging.API"
RUN dotnet build "./CampusLearn.PrivateMessaging.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -----------------------
# Stage 3: Publish
# -----------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CampusLearn.PrivateMessaging.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:PublishTrimmed=true \
    /p:TrimMode=link 

# -----------------------
# Stage 4: Final runtime image
# -----------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CampusLearn.PrivateMessaging.API.dll"]
