BankApi — инструкции по запуску

Кратко
- API доступно в Swagger по адресу: http://localhost:8080/swagger
- Запуск делается через Docker Compose (предпочтительный способ).

Требования
- Docker / Docker Compose
- (Опционально) Visual Studio 2022/2026 или dotnet 10 SDK для запуска локально

Запуск в Docker (рекомендуется)
1. В корне репозитория:
   docker compose up -d --build
2. Проверить логи:
   docker compose logs -f bankapi
3. Открыть Swagger:
   http://localhost:8080/swagger

Примечания по БД
- В docker-compose сервис db (Postgres) запущен без проброса порта на хост — доступ к БД есть только другим контейнерам (bankapi подключается к db по имени сервиса).
- Если хотите подключаться к Postgres с хоста (настольный pgAdmin или приложение вне Docker), откройте docker-compose.yml и у секции db добавьте:
	ports:
	  - "5432:5432"
  затем перезапустите docker compose up -d --build. После этого строка подключения для локального приложения будет Host=localhost;Port=5432;Username=postgres;Password=postgrespw

Пересоздание/обновление БД
- При старте приложение автоматически применяет миграции (db.Database.Migrate()).
- Переинициализация (удаление всех данных) — удалит volume и создаст БД заново:
	docker compose down -v
	docker compose up -d --build

Если нужен откат (возврат старых данных)
- Если у вас не сохранены бэкапы — данные восстановить нельзя.

Запуск локально (без Docker)
1. Установите/запустите PostgreSQL и создайте базу/пользователя, либо пробросьте порт контейнера на localhost.
2. В BankApi/appsettings.json задайте корректную строку подключения (пример):
   "ConnectionStrings": {
	 "PostgresConnection": "Host=localhost;Port=5432;Database=bankdb;Username=postgres;Password=postgrespw"
   }
3. Запустите проект из Visual Studio (F5) или dotnet run в каталоге проекта BankApi.
4. Swagger: http://localhost:5000/swagger или по адресу, который покажет приложение.

Полезные команды
- Посмотреть логи всех сервисов: docker compose logs --tail 200
- Логи конкретного сервиса: docker compose logs bankapi --tail 200
- Перезапустить сервис: docker compose restart bankapi
- Полный перезапуск с очисткой volume: docker compose down -v && docker compose up -d --build

Изменения в проекте при настройке Docker
- Program.cs: Swagger включён всегда и приложение выполняет миграции при старте.
- docker-compose.yml: настроен сервис bankapi и db; по умолчанию проброс 5432 наружу удалён (для безопасности).
