# File Dump

Простое хранилище файлов с REST API для загрузки, скачивания и управления файлами.

## Возможности

- 📁 Загрузка файлов через HTTP API
- 📥 Скачивание файлов по идентификатору
- 🔐 Аутентификация на основе JWT
- 🗂️ Хранение метаданных в PostgreSQL
- ☁️ Поддержка облачного хранилища MinIO
- 📊 Логирование операций

## Требования

- .NET 6.0 SDK или новее
- PostgreSQL 12+
- MinIO (опционально, можно использовать файловую систему)

## Установка и настройка

1. Клонируйте репозиторий:
```bash
git clone <repository-url>
cd file-dump
```

2. Настройте подключение к базе данных в `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Postgres": "Server=localhost;Port=5432;Database=filedump;User Id=postgres;Password=your_password;"
  }
}
```

3. Настройте параметры JWT (рекомендуется изменить секретный ключ):
```json
{
  "JwtOptions": {
    "SecretKey": "your_secure_secret_key_here",
    "ExpiresHours": "12"
  }
}
```

4. Настройте подключение к MinIO (если используется):
```json
{
  "MinioOptions": {
    "Endpoint": "minio.example.com:9000",
    "AccessKey": "your_access_key",
    "SecretKey": "your_secret_key",
    "BucketName": "file-dump"
  }
}
```

5. Примените миграции базы данных:
```bash
dotnet ef database update
```

6. Запустите приложение:
```bash
dotnet run
```

## Использование API

### Аутентификация

Получение JWT токена:
```bash
curl -X POST "https://api.example.com/auth/login" \
     -H "Content-Type: application/json" \
     -d '{"username":"your_username","password":"your_password"}'
```

### Загрузка файла

```bash
curl -X POST "https://api.example.com/files/upload" \
     -H "Authorization: Bearer YOUR_JWT_TOKEN" \
     -F "file=@/path/to/your/file.txt"
```

### Скачивание файла

```bash
curl -X GET "https://api.example.com/files/download/{fileId}" \
     -H "Authorization: Bearer YOUR_JWT_TOKEN" \
     --output downloaded_file.txt
```

### Получение информации о файле

```bash
curl -X GET "https://api.example.com/files/{fileId}" \
     -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Удаление файла

```bash
curl -X DELETE "https://api.example.com/files/{fileId}" \
     -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Конфигурация

Полный пример файла конфигурации `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Postgres": "Server=localhost;Port=5432;Database=filedump;User Id=postgres;Password=password;"
  },
  "JwtOptions": {
    "SecretKey": "C2rR58wrMAgPSH3dbSEAMvvLGcVCpZXKdycZG4fr",
    "ExpiresHours": "12"
  },
  "MinioOptions": {
    "Endpoint": "",
    "AccessKey": "",
    "SecretKey": "",
    "BucketName": "file-dump"
  }
}
```

## Разработка

### Сборка проекта

```bash
dotnet build
```

### Запуск тестов

```bash
dotnet test
```

### Создание миграций

```bash
dotnet ef migrations add MigrationName
```

## Безопасность

- Все запросы требуют JWT аутентификации (кроме эндпоинта логина)
- Пароли хранятся в хэшированном виде
- Рекомендуется использовать HTTPS в production

## Лицензия

Этот проект лицензирован под MIT License - см. файл LICENSE для деталей.
