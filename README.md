## Установка и запуск

1. Клонируйте репозиторий:
    ```bash
    git clone <URL репозитория>
    cd AuctionApi
    ```

2. Настройте строку подключения к базе данных в `appsettings.json`:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AuctionDb;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

3. При запуске создаться Админ которые запишуться в базу данных:
     adminEmail = "admin@example.com";
     adminPassword = "Admin@1234";


4. Запустите приложение:
    ```bash
    dotnet run
    ```

5. API будет доступно по адресу `https://localhost:7130/api`.
