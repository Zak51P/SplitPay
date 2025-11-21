# SplitPay

Backend-приложение для разделения расходов (Splitwise-like) на .NET 8 c Clean Architecture: Domain → Application → Infrastructure → Api.

## Основные возможности
- Группы, участники, расходы.
- Методы деления: Equal, Exact, Percent (инварианты в домене).
- Swagger в Development.

## Быстрый старт (Docker)
```bash
docker compose up --build
```
API будет доступно на `http://localhost:8080/swagger`. ConnectionString прокидывается через `ConnectionStrings__Default`.

## Локальный старт без Docker
1. Установи PostgreSQL (или подними `docker run postgres:16 ...`), создай БД `splitpay`.
2. Пропиши строку подключения в `src/SplitPay.Api/appsettings.Development.json` (или через переменную `ConnectionStrings__Default`).
3. Миграции (нужен установленный `dotnet-ef`):
   ```bash
   dotnet ef migrations add InitialCreate -s src/SplitPay.Api/SplitPay.Api.csproj -p src/SplitPay.Infrastructure/SplitPay.Infrastructure.csproj -o Persistence/Migrations
   dotnet ef database update -s src/SplitPay.Api/SplitPay.Api.csproj -p src/SplitPay.Infrastructure/SplitPay.Infrastructure.csproj
   ```
4. Запуск API:
   ```bash
   dotnet run --project src/SplitPay.Api/SplitPay.Api.csproj
   ```
   Swagger: `https://localhost:7249/swagger` или `http://localhost:5248/swagger` (порт см. в логе).

## Основные эндпоинты (префикс `/api`)
- `GET /api/health`
- `POST /api/groups` — `{ "name": "Friends" }`
- `POST /api/groups/{groupId}/members` — `{ "displayName": "Alice" }`
- `POST /api/groups/{groupId}/expenses`
  ```json
  {
    "payerId": "guid",
    "amount": 120,
    "currency": "USD",
    "description": "Dinner",
    "method": "Equal", // или Exact/Percent
    "parts": [
      { "memberId": "guid", "share": null, "percent": null }
    ],
    "participantsCountForEqual": 0
  }
  ```
- `GET /api/groups/{groupId}` — группа с участниками и расходами.

## Тесты
```bash
dotnet test SplitPay.sln
```
Покрывают доменные правила Money/Expense и базовые хэндлеры.

## Архитектура
- **Domain**: сущности Group/Member/Expense/SplitPart, value object Money, инварианты сплитов.
- **Application**: команды/запросы, DTO, интерфейсы репозиториев.
- **Infrastructure**: EF Core + Npgsql, DbContext, fluent-конфигурации, репозитории, UnitOfWork.
- **Api**: Minimal API, DI из Infrastructure, Swagger.

## CI
В `.github/workflows/ci.yml` — restore/build/test на .NET 8. Добавь секреты/шаги, если нужен deploy.
