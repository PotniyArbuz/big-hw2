# Plagiarism Checker

## Описание

Система состоит из трех микросервисов, каждый из которых выполняет свою задачу:

- **API Gateway** (порт 8080): Точка входа для клиентов, маршрутизирует запросы и координирует работу сервисов.
- **File Storing Service** (порт 8081): Хранит текстовые файлы в базе PostgreSQL и предоставляет их содержимое.
- **File Analysis Service** (порт 8082): Анализирует файлы, подсчитывает статистику и проверяет на плагиат с помощью SHA-256 хэшей.


## Архитектура

### Компоненты

1. **API Gateway**

   - **Назначение**: Управляет клиентскими запросами и координирует работу сервисов.
   - **Обязанности**:
     - Принимает файлы через `POST /upload`.
     - Пересылает файлы в `FileStoringService`.
     - Запрашивает анализ у `FileAnalysisService`.
     - Возвращает результаты клиенту.

2. **File Storing Service**

   - **Назначение**: Хранение и извлечение текстовых файлов (.txt).
   - **Обязанности**:
     - Сохраняет файлы в базе PostgreSQL.
     - Предоставляет содержимое и метаданные (время загрузки) через API.
   - **База данных**: `postgres-file-storing`

3. **File Analysis Service**

   - **Назначение**: Анализ файлов и проверка на плагиат.
   - **Обязанности**:
     - Получает содержимое файла из `FileStoringService`.
     - Подсчитывает параграфы, слова, символы.
     - Вычисляет SHA-256 хэш для проверки плагиата.
     - Сравнивает хэш с предыдущими файлами для выявления совпадений.
     - Сохраняет результаты анализа.
   - **База данных**: `postgres-file-analysis`

### Процесс работы

1. Клиент отправляет `POST /upload` с файлом .txt в `ApiGateway`.
2. `ApiGateway` пересылает файл в `FileStoringService` (`POST /files`), получая `fileId`.
3. `ApiGateway` вызывает `POST /analyze/{fileId}` в `FileAnalysisService`.
4. `FileAnalysisService` запрашивает файл через `GET /files/{fileId}` у `FileStoringService`.
5. `FileAnalysisService` выполняет анализ, проверяет на плагиат и возвращает результаты.
6. `ApiGateway` отправляет результаты клиенту.

## API

### API Gateway

- **POST /upload**
  - **Описание**: Загружает файл и возвращает результаты анализа.
  - **Запрос**: `multipart/form-data` с файлом (.txt).
  - **Ответ**:

    ```json
    {
      "fileId": "uuid",
      "paragraphs": "int",
      "words": "int",
      "characters": "int",
      "isPlagiarized": "boolean"
    }
    ```

### File Storing Service

- **POST /files**

  - **Описание**: Сохраняет файл и возвращает его ID.
  - **Запрос**: `multipart/form-data` с файлом (.txt).
  - **Ответ**:

    ```json
    {
      "fileId": "uuid"
    }
    ```

- **GET /files/{fileId}**

  - **Описание**: Возвращает содержимое файла и метаданные.
  - **Ответ**:

    ```json
    {
      "fileId": "uuid",
      "uploadTimestamp": "timestamp",
      "content": "string"
    }
    ```

### File Analysis Service

- **POST /analyze/{fileId}**
  - **Описание**: Анализирует файл и возвращает результаты.
  - **Ответ**:

    ```json
    {
      "fileId": "uuid",
      "paragraphs": "int",
      "words": "int",
      "characters": "int",
      "isPlagiarized": "boolean"
    }
    ```

## Развертывание

Система развертывается с помощью **Docker Compose**:

1. Клонировать репозиторий.
2. Зайти в папку с решением (PlagiarismChecker).
3. Запустить сервисы:
    ```bash
   docker-compose up --build
    ```

4. Доступ к сервисам:

   - **API Gateway**: `http://localhost:8080/swagger`
   - **File Storing Service**: `http://localhost:8081/swagger`
   - **File Analysis Service**: `http://localhost:8082/swagger`

5. Очистка баз данных и контейнеров:
   - **Остановить и удалить контейнеры**:
      ```bash
     docker-compose down
      ```
   - **Удалить Docker-тома**:
      ```bash
     docker volume rm plagiarismchecker_pgdata-file-analysis plagiarismchecker_pgdata-file-storing
      ```
