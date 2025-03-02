# CarStore Backend

`CarStore` — это backend веб-приложения для магазина автомобилей. Проект предоставляет REST API для управления пользователями и автомобилями с поддержкой CRUD-операций, аутентификации, авторизации и кеширования. Разработан на C# с использованием ASP.NET Core, Entity Framework Core и развертывается через Docker.

---

## Описание проекта

`CarStore` — серверная часть интернет-магазина автомобилей. Основные сущности:
- **User**: Пользователь системы (наследуется от `IdentityUser` для аутентификации).
- **Car**: Автомобиль с характеристиками (марка, модель, год, цена).

**Отношение**: Один пользователь может владеть многими автомобилями ("один ко многим").

### Основные функции
- **CRUD для пользователей и автомобилей**: Создание, чтение, обновление и удаление через REST API.
- **Аутентификация и авторизация**: Используются JWT-токены для входа, обновления токенов и доступа к защищенным endpoint’ам.
- **Кеширование**: Ускорение операций чтения (например, `GetAllCars`, `GetAllUsers`) с помощью `IMemoryCache`.
- **База данных**: PostgreSQL, запускается в Docker.
- **DTO**: Используются для передачи данных между слоями (например, `UserDto`, `CarDto`).

---

## Архитектура

Проект следует многослойной архитектуре:

- **CarStore (API Layer)**:
   - `Controllers`: `AuthController`, `UserController`, `CarController`.
   - `Contracts`: Модели запросов/ответов.
   - `Program.cs`: Настройка DI и middleware.
- **CarStore.Application (Application Layer)**:
   - `Services`: `UserService`, `CarService`, `JWTUtilsService`, `TokenService`.
- **CarStore.Core (Domain Layer)**:
   - `Models`: `User`, `Car`, `RefreshToken`.
   - `DTO`: `UserDto`, `CarDto`, `AuthResponse`.
   - `MappingProfile`: AutoMapper.
   - `Abstractions`: Интерфейсы.
- **CarStore.DataAccess (Data Layer)**:
   - `CarStoreDbContext`: EF Core.
   - `Repository`: Репозитории.
   - `Migrations`: Миграции базы данных.

---

## Технологии

- **Язык**: C# 10
- **Фреймворк**: ASP.NET Core 8
- **База данных**: PostgreSQL
- **ORM**: Entity Framework Core
- **Аутентификация**: ASP.NET Core Identity + JWT
- **Кеширование**: `IMemoryCache`
- **Маппинг**: AutoMapper
- **Контейнеризация**: Docker + Docker Compose
- **IDE**: Rider

---

## Установка и запуск

### Требования
- [.NET SDK](https://dotnet.microsoft.com/download) (версия 8 или выше).
- [Docker](https://www.docker.com/get-started) и [Docker Compose](https://docs.docker.com/compose/install/).
- IDE: Rider/VS Code.

### Шаги для запуска
1. Клонировать репозиторий
2. Создать файл appsettings.json/appsettings.Development.json в корне проекта (указать настройки для логирования, подключение к БД, JWT)
3. Запустить БД в Docker (docker-compose.yml) > поднять контейнер docker-compose up -d
4. Выполнить мигрaцию и обновить БД: dotnet ef migrations add InitialCreate --project CarStore dotnet ef database update --project CarStore
5. Запустить: dotnet run --project CarStore

