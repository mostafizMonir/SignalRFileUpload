FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["FileUpload.Worker/FileUpload.Worker.csproj", "FileUpload.Worker/"]
COPY ["FileUpload.Contracts/FileUpload.Contracts.csproj", "FileUpload.Contracts/"]
RUN dotnet restore "FileUpload.Worker/FileUpload.Worker.csproj"
COPY . .
WORKDIR "/src/FileUpload.Worker"
RUN dotnet build "FileUpload.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileUpload.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileUpload.Worker.dll"] 