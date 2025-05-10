# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Create a non-root user and set up uploads directory
RUN groupadd -r appuser && useradd -r -g appuser appuser && \
    mkdir -p /app/uploads && \
    chown -R appuser:appuser /app/uploads && \
    chmod -R 755 /app/uploads

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["FileUpload.API/FileUpload.API.csproj", "FileUpload.API/"]
COPY ["FileUpload.Contracts/FileUpload.Contracts.csproj", "FileUpload.Contracts/"]
RUN dotnet restore "FileUpload.API/FileUpload.API.csproj"
COPY . .
WORKDIR "/src/FileUpload.API"
RUN dotnet build "FileUpload.API.csproj" -c Release -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
RUN dotnet publish "FileUpload.API.csproj" -c Release -o /app/publish

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER appuser
ENTRYPOINT ["dotnet", "FileUpload.API.dll"]