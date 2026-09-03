# CLAUDE.md

## Что это за проект

`Calabonga.PagedListCore.Json` — небольшая NuGet-библиотека: конвертер `System.Text.Json`
для **десериализации** `IPagedList<T>` из пакета `Calabonga.PagedListCore`.
Публикуется в nuget.org: <https://www.nuget.org/packages/Calabonga.PagedListCore.Json>

- TFM: `netstandard2.1`, `LangVersion=latest` (нужен для file-scoped namespaces и др.
  современного синтаксиса — netstandard2.1 по умолчанию даёт C# 8.0), `Nullable` включён.
- Версия пакета задаётся в `<Version>` в `src/Calabonga.PagedListCore.Json.csproj`.
- `GeneratePackageOnBuild=True` — при обычной сборке уже собирается `.nupkg`.

## Структура

- `src/Calabonga.PagedListCore.Json.slnx` — решение с **одним** проектом библиотеки;
  используется для `dotnet pack` (в пакет попадает только библиотека).
- `src/Calabonga.PagedListCore.Json.Tests.slnx` — решение с библиотекой + проектом тестов;
  используется для `dotnet build` / `dotnet test` в CI.
- `src/Calabonga.PagedListCore.Json.csproj` — проект библиотеки. Каталог проекта — `src/`, поэтому
  подпапка тестов исключается явными `<Compile Remove="Calabonga.PagedListCore.Json.Tests\**" />`.
- `src/PageListConverter.cs` — единственный класс `PageListConverter<T> : JsonConverter<IPagedList<T>>`.
  - `Read(...)` разбирает объект с полями `pageIndex`, `pageSize`, `totalCount`, `items`
    и возвращает `PagedList<T>`.
  - `Write(...)` намеренно бросает `NotImplementedException` — сериализация не поддерживается.
- `src/Calabonga.PagedListCore.Json.Tests/` — xUnit-проект (`net10.0`, `IsPackable=false`),
  `ProjectReference` на библиотеку. Тесты покрывают `PageListConverter<T>.Read/Write`.
- `src/README.md` — уходит в пакет как `PackageReadmeFile`.
- `README.md` (корень) — публичный changelog проекта.
- `.github/workflows/main.yml` — CI: restore → build → **test** → pack → push в nuget.org
  при push в `main` (runner `windows-latest`, .NET `10.0.x`). Упавший `dotnet test` останавливает
  публикацию пакета.

## Команды

Сборка:

```bash
dotnet build src/Calabonga.PagedListCore.Json.slnx -c Release
```

Тесты:

```bash
dotnet test src/Calabonga.PagedListCore.Json.Tests.slnx -c Release
```

## Правила

Обязательны к соблюдению, лежат отдельными файлами:

- `.claude/rules/code-styles.md` — стиль C# (file-scoped namespaces, nullable, `sealed` по
  умолчанию, порядок членов класса, `ConfigureAwait(false)` в библиотечном коде, `Async`-суффикс
  и `CancellationToken` у новых async-методов).
- `.claude/rules/workflow.md` — рабочий процесс (отдельная ветка `feature/`|`bugfix/`|`hotfix/`
  перед изменениями, коммиты `type: description`, атомарные коммиты, версия пакета и changelog в
  том же PR, что и функциональные изменения; тесты обязательны перед фиксацией).

## Замечания по текущему состоянию

- Имя типа — `PageListConverter` (без «d»), файл `PageListConverter.cs`; при упоминании в коде
  соблюдай это написание.
- Changelog в корневом `README.md` держи синхронным с `<Version>` — обновляй при выпуске.
- `PagedList<T>` использует 1-based индекс страницы и отклоняет значение < 1. Поэтому `Read(...)`
  при отсутствии `pageIndex` в JSON подставляет `1`; явный `pageIndex` < 1 считается некорректным
  входом и пробрасывает `ArgumentOutOfRangeException` (Fail Fast).
- Ключи в `Read(...)` регистрозависимы (`items`, а не `Items`) и не зависят от `PropertyNamingPolicy`.
