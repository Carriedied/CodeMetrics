# Используем SDK образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Устанавливаем рабочую директорию внутри контейнера сборки
WORKDIR /src

# Копируем файл решения и все файлы проектов (.csproj)
# Это позволяет эффективно использовать кэш слоев Docker при изменении кода
COPY ["APICodeMetrics.sln", "."]
COPY ["APICodeMetrics/APICodeMetrics.csproj", "APICodeMetrics/"]
COPY ["APICodeMetrics.Data/APICodeMetrics.Data.csproj", "APICodeMetrics.Data/"]

# Восстанавливаем зависимости (NuGet пакеты)
RUN dotnet restore "APICodeMetrics.sln" # Укажите путь к .sln файлу

# Копируем все исходные файлы проектов
COPY . .

# Собираем проект (Release конфигурация)
RUN dotnet build "APICodeMetrics/APICodeMetrics.csproj" -c Release -o /app/build

# --- Стадия публикации ---
FROM build AS publish
RUN dotnet publish "APICodeMetrics/APICodeMetrics.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Стадия выполнения ---
# Используем более легковесный runtime образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0
# Устанавливаем рабочую директорию внутри контейнера выполнения
WORKDIR /app
# Копируем опубликованные файлы из стадии публикации
COPY --from=publish /app/publish .

# Устанавливаем порт, который будет использовать ASP.NET Core
# Убедитесь, что это соответствует вашим настройкам (например, в launchSettings.json)
# Обычно ASP.NET Core слушает на $PORT или 80/443 внутри контейнера
ENV ASPNETCORE_URLS=http://+:80
# Если ваше приложение использует HTTPS, возможно, потребуется установить ASPNETCORE_HTTPS_PORT и ASPNETCORE_Kestrel__Certificates__Default__Path

# Команда, которая будет выполнена при запуске контейнера
ENTRYPOINT ["dotnet", "APICodeMetrics.dll"] # Укажите имя .dll вашего основного проекта (без .csproj)
