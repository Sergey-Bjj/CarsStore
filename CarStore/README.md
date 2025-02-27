CarStore Backend
CarStore — backend веб-приложения для магазина автомобилей. 
Проект REST API для управления пользователями и автомобилями 
с поддержкой CRUD-операций, аутентификации, авторизации и кеширования. 
Проект разработан на C# с использованием ASP.NET Core, EntityFramework и развертывается с помощью Docker.

Описание проекта
CarStore — это серверная часть интернет-магазина автомобилей. 
Основные сущности:
User: Пользователь системы (наследуется от IdentityUser для аутентификации).
Car: Автомобиль с характеристиками (марка, модель, год, цена).
Отношение: Один пользователь может владеть многими автомобилями (отношение "один ко многим").

Основные функции:
CRUD для пользователей и автомобилей: Создание, чтение, обновление и удаление через REST API.
Аутентификация и авторизация: Используется JWT-токены для входа, обновления токенов и доступа к защищенным endpoint’ам.
Кеширование: Для ускорения операций чтения (например, GetAllCars, GetAllUsers) используется IMemoryCache.
База данных: PostgreSQL запускается в Docker.
DTO: Используются для передачи данных между слоями.

Архитектура
Проект следует многослойной архитектуре:
CarStore/
├── CarStore (API Layer)
│   ├── Controllers (AuthController, UserController, CarController), Contracts, Program.cs (DI и Middleware)
├── CarStore.Application (Application Layer)
│   ├── Services (UserService, CarStoreService, JWTUtilsService, TokenService)
├── CarStore.Core (Domain Layer)
│   ├── Models (User, Car, RefreshToken), MappingProfile (AutoMapper),DTO (UserDto, CarDto, AuthResponse), Abstractions (interface)
├── CarStore.DataAccess (Data Layer
│   ├── CarStoreDbContext (EF Core), Repository, Migrations

Технологии
Язык: C# 10
Фреймворк: ASP.NET Core 8
База данных: PostgreSQL
ORM: Entity Framework Core
Аутентификация: ASP.NET Core Identity + JWT
Кеширование: IMemoryCache
Маппинг: AutoMapper
Контейнеризация: Docker + Docker Compose
IDE: Rider

Установка и запуск
Установить .NET SDK (версия 8 или выше)
Установить Docker и Docker Compose
Установить IDE (Rider)

Шаги для запуска:
1. Клонировать репозиторий
2. Установить конфигурации проекта (appsettings.json)
3. Запустить БД в Docker (docker-compose.yml) > поднять контейнер docker-compose up -d
4. Выполнить мигрaцию и обновить БД: 
   dotnet ef migrations add InitialCreate --project CarStore
   dotnet ef database update --project CarStore
5. Запустить:
   dotnet run --project CarStore