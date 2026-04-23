# Вимоги框架 автоматизованого тестування .NET NUnit Playwright

## 1. Архітектура та структура

### 1.1 Структура проєкту
```
PlaywrightTests/
├── bin/
├── obj/
├── Config/
│   ├── Environments/
│   │   ├── dev.json
│   │   ├── staging.json
│   │   └── production.json
│   └── PlaywrightSettings.json
├── Core/
│   ├── Managers/
│   │   ├── BrowserManager.cs
│   │   ├── ConfigManager.cs
│   │   └── LoggerManager.cs
│   ├── Pages/
│   │   └── BasePage.cs
│   ├── Steps/
│   │   └── BaseSteps.cs
│   └── Helpers/
│       ├── WaitHelpers.cs
│       └── DataHelpers.cs
├── Tests/
│   ├── UI/
│   │   └── [Тестові класи]
│   └── API/
│       └── [API тестові класи]
├── Logs/
├── Reports/
│   └── allure-results/
├── appsettings.json
└── PlaywrightTests.csproj
```

### 1.2 Архітектурні паттерни
- **Page Object Model (POM)** - для UI тестів
- **Step Definition Pattern** - для організації логіки тестів
- **Base Page/Step классы** - для переиспользования функциональности
- **Dependency Injection** - через Microsoft.Extensions.DependencyInjection
- **Configuration Management** - для управління середовищами

---

## 2. Залежності (NuGet Packages)

### 2.1 Обов'язкові
```xml
<!-- Тестування -->
<PackageReference Include="NUnit" Version="4.3.2" />
<PackageReference Include="NUnit3TestAdapter" Version="5.0.0" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.0" />

<!-- Playwright -->
<PackageReference Include="Microsoft.Playwright.NUnit" Version="1.48.0" />

<!-- Конфігурація -->
<PackageReference Include="Microsoft.Extensions.Configuration" Version="10.0.5" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="10.0.5" />
<PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="10.0.5" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.5" />
```

### 2.2 Логування
```xml
<!-- Serilog -->
<PackageReference Include="Serilog" Version="4.2.1" />
<PackageReference Include="Serilog.Sinks.Console" Version="6.2.1" />
<PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
<PackageReference Include="Serilog.Sinks.Async" Version="2.3.0" />
<PackageReference Include="Serilog.Enrichers.Environment" Version="3.0.1" />
<PackageReference Include="Serilog.Enrichers.Thread" Version="3.2.0" />
<PackageReference Include="Serilog.Formatting.Compact" Version="3.1.0" />
```

### 2.3 Звітування (Allure)
```xml
<PackageReference Include="Allure.NUnit" Version="2.13.45" />
<PackageReference Include="Allure.Net.Commons" Version="2.13.45" />
```

### 2.4 Додаткові утиліти
```xml
<!-- JSON -->
<PackageReference Include="System.Text.Json" Version="10.0.5" />

<!-- Async/Await -->
<PackageReference Include="Microsoft.Bcl.AsyncInterfaces" Version="10.0.0" />

<!-- Code coverage -->
<PackageReference Include="coverlet.collector" Version="6.0.4" />
```

---

## 3. Конфігурація

### 3.1 appsettings.json
```json
{
  "Playwright": {
    "BrowserType": "chromium",
    "HeadlessMode": true,
    "Timeout": 30000,
    "SlowMoDelay": 0,
    "FullPage": false,
    "ViewportWidth": 1920,
    "ViewportHeight": 1080,
    "LaunchArgs": ["--disable-blink-features=AutomationControlled"]
  },
  "Environments": {
    "Default": "dev",
    "Urls": {
      "dev": "https://dev.example.com",
      "staging": "https://staging.example.com",
      "production": "https://example.com"
    }
  },
  "Logging": {
    "MinimumLevel": "Information",
    "WriteTo": ["Console", "File"],
    "FilePath": "Logs/test-{Date:yyyy-MM-dd}.log",
    "OutputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"
  },
  "Allure": {
    "Enabled": true,
    "ResultsDirectory": "allure-results",
    "CleanBeforeRun": true
  },
  "TestData": {
    "DefaultTimeout": 5,
    "RetryAttempts": 3,
    "Screenshots": true,
    "Videos": false
  }
}
```

### 3.2 Environments JSON (dev.json, staging.json, production.json)
```json
{
  "Environment": "dev",
  "BaseUrl": "https://dev.example.com",
  "ApiUrl": "https://api-dev.example.com",
  "Credentials": {
    "Username": "test_user",
    "Password": "${TEST_PASSWORD}"
  },
  "Features": {
    "DebugMode": true,
    "SlowMode": false,
    "ScreenshotsOnFailure": true
  }
}
```

---

## 4. Вимоги до ядра框架

### 4.1 BrowserManager
- Ініціалізація Playwright контексту
- Управління браузером та сторінками
- Обробка скріншотів та відео
- Контекстне управління для паралельного виконання

### 4.2 ConfigManager
- Завантаження конфігурації з appsettings.json
- Управління середовищами (dev, staging, prod)
- Підтримка environment variables для чутливих даних
- Кешування конфігурації

### 4.3 LoggerManager
- Ініціалізація Serilog
- Різні рівні логування (Debug, Info, Warning, Error)
- Логування до файлу та консолі
- Форматування логів з контекстом
- Async логування для відсутності блокування

### 4.4 BasePage
- Базові методи для взаємодії з елементами
- Очікування елементів (Wait, WaitForVisible, WaitForClickable)
- Обробка помилок з auto-retry
- Логування дій на сторінці
- Screenshot на помилку

### 4.5 BaseSteps
- BDD-style step методи
- Управління test data
- Common actions (Login, Navigation, Form filling)
- Error handling та retry logic

---

## 5. Вимоги до логування

### 5.1 Структура логів
- **Debug**: Детальна інформація про виконання
- **Info**: Основні дії тесту (Навігація, Логін, Клік)
- **Warning**: Нестандартна поведінка
- **Error**: Помилки та падіння тестів

### 5.2 Контекст логування
```
[2026-04-16 10:30:45.123 +00:00] [INF] Test: LoginTest.ValidLoginScenario | Browser: Chromium | Step: Click Login Button
[2026-04-16 10:30:46.456 +00:00] [ERR] Navigation timeout after 30s to https://example.com
```

### 5.3 Логи файлів
- Розташування: `Logs/test-{Date:yyyy-MM-dd}.log`
- Ротація: За датою (окремий файл на день)
- Рівень: ConfigManager контролює
- Асинхронне логування для продуктивності

---

## 6. Вимоги до Allure звітування

### 6.1 Структура звіту
- **Defects** - Помилки за категоріями
- **Suites** - Групування тестів
- **Tests** - Результати всіх тестів
- **Trends** - Історія виконання
- **Duration** - Тривалість тестів

### 6.2 Allure атрибути
```csharp
[AllureFeature("Authentication")]
[AllureSuite("Login Tests")]
[AllureSeverity(SeverityLevel.Blocker)]
[AllureTag("UI")]
[AllureLink("https://jira.example.com/browse/TEST-123")]
```

### 6.3 Allure результати
- Розташування: `allure-results/`
- Очистка перед запуском: `true`
- Скріншоти прикріпляються до звіту
- Параметри тесту включаються в звіт

### 6.4 Команди Allure
```bash
# Запуск тестів
dotnet test

# Генерування звіту
allure generate allure-results -o allure-report -c

# Відкриття звіту
allure open allure-report
```

---

## 7. Вимоги до тестів

### 7.1 Організація
- Один тест - один сценарій
- Принцип AAA (Arrange, Act, Assert)
- Использование POM для UI взаимодействия
- Независимость тестов (no dependencies between tests)

### 7.2 Naming Convention
```csharp
[Test]
[AllureFeature("Feature Name")]
public async Task TestName_Scenario_ExpectedResult()
{
    // Arrange
    // Act
    // Assert
}
```

### 7.3 Атрибути та Metadata
```csharp
[Category("Smoke")]
[Category("UI")]
[Retry(3)]  // автоматичний retry
[Timeout(60000)]  // таймаут 60 сек
```

### 7.4 Assertion Library
- Використання NUnit Assertions або Fluent Assertions
- Описові повідомлення про помилку
- Логування перед assertion

---

## 8. Вимоги до обробки помилок та retry

### 8.1 Retry механізм
- Автоматичний retry для flaky тестів (max 3 спроби)
- Логування кожної спроби
- Screenshot на кожної помилці

### 8.2 Обробка timeout
- Встановлення глобального таймаута (30 сек)
- Логування timeout інформації
- Graceful shutdown браузера

### 8.3 Screenshot на помилку
- Автоматичні скріншоти при падінні
- Назва файла: `{TestName}_{Timestamp}_{Status}.png`
- Прикріплення до Allure звіту

---

## 9. Вимоги до CI/CD інтеграції

### 9.1 Команди запуска

```bash
# Запуск всіх тестів
dotnet test

# Запуск за категорією
dotnet test --filter "Category=Smoke"

# Запуск з параметрами
dotnet test --configuration Release --no-build

# Паралельне виконання
dotnet test --parallel
```

### 9.2 Вихід результатів
- NUnit3 XML результати
- Allure JSON результати
- Code coverage звіти
- Логи всіх тестів

### 9.3 Артефакти для CI/CD
- `TestResults/` - NUnit результати
- `allure-results/` - Allure результати
- `Logs/` - Логи виконання
- `Screenshots/` - Скріншоти на помилку
- `coverage-results/` - Code coverage звіти

---

## 10. Вимоги до документації та стандартів

### 10.1 Code Style
- C# Naming Conventions
- SOLID принципи
- DRY (Don't Repeat Yourself)
- Clean Code практики

### 10.2 Documentation
- Комментарії для складної логіки
- README для запуска
- Contributing guidelines
- API документація для框架методів

### 10.3 Version Control
- Git flow для управління гілками
- Meaningful commit messages
- Pull requests з описом
- Code review перед merge

---

## 11. Вимоги до тестового середовища

### 11.1 Системні вимоги
- .NET 10.0 SDK (мінімум)
- Windows/Linux/macOS
- 4GB RAM (мінімум для паралельного запуска)
- 2GB вільного місця на диску

### 11.2 Залежності
- Playwright browsers (автоматично завантажуються)
- Java 8+ для Allure (опціонально)
- Git для version control

### 11.3 Браузери
- Chromium (за замовчуванням)
- Firefox (опціонально)
- WebKit (опціонально)

---

## 12. Вимоги до безпеки та конфіденційності даних

### 12.1 Конфіденційні дані
- Credentials в environment variables
- Не комітити `.env` файли
- Маскування passwords в логах
- Secure хранення test data

### 12.2 Логування
- Не логірувати passwords та tokens
- Логування URL без параметрів авторизації
- Очистка чутливих даних з екранів

---

## 13. Метрики та KPI

### 13.1 Успішність тестів
- Pass Rate > 95%
- Failed tests повинні мати логи та скріншоти
- Flaky tests должны быть виявлені та зафіксовані

### 13.2 Тривалість
- Unit tests: < 1 min
- Smoke tests: < 5 min
- Full test suite: < 30 min

### 13.3 Покриття кода
- Target: > 70% покриття
- Critical paths: > 90% покриття
- Core library: 100% покриття

---

## 14. План розгортання

### Phase 1: Foundation (Week 1-2)
- [ ] Налаштування базового проєкту
- [ ] Конфігурація в appsettings.json
- [ ] BrowserManager та ConfigManager

### Phase 2: Logging & Core (Week 2-3)
- [ ] Serilog інтеграція
- [ ] LoggerManager
- [ ] BasePage та BaseSteps

### Phase 3: Allure Integration (Week 3-4)
- [ ] Allure NUnit інтеграція
- [ ] Жmeplating звітів
- [ ] Screenshot attachment

### Phase 4: Tests & CI/CD (Week 4-5)
- [ ] Написання тестів
- [ ] CI/CD pipeline
- [ ] Parallel execution

---

## 15. Успішність критерії

✅ Фреймворк готовий коли:
- [ ] Всі залежності встановлені та версіоновані
- [ ] Конфіг управління працює для dev/staging/prod
- [ ] Логування логує до файлу та консолі
- [ ] Allure звіти генеруються коректно
- [ ] Мінімум 5 UI тестів написані та проходять
- [ ] CI/CD pipeline налаштований
- [ ] Документація повна
- [ ] Code review пройдено

---
