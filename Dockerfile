# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY StoreManagementService/*.csproj StoreManagementService/
RUN dotnet restore StoreManagementService/StoreManagementService.csproj

# Copiar el resto del código y compilar
COPY . .
RUN dotnet publish StoreManagementService/StoreManagementService.csproj -c Release -o /app

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app

# Copiar los binarios publicados desde la etapa anterior
COPY --from=build /app ./

# Exponer el puerto 8446
EXPOSE 8446

# Configurar la variable de entorno para que Kestrel use ese puerto
ENV ASPNETCORE_URLS=http://+:8446

# Comando de inicio
ENTRYPOINT ["dotnet", "StoreManagementService.dll"]
