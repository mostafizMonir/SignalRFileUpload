FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["FileUpload.Notification/FileUpload.Notification.csproj", "FileUpload.Notification/"]
COPY ["FileUpload.Contracts/FileUpload.Contracts.csproj", "FileUpload.Contracts/"]
RUN dotnet restore "FileUpload.Notification/FileUpload.Notification.csproj"
COPY . .
WORKDIR "/src/FileUpload.Notification"
RUN dotnet build "FileUpload.Notification.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileUpload.Notification.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileUpload.Notification.dll"] 