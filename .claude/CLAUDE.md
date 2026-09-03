# CLAUDE.md

## Что это за проект

`Calabonga.PagedListCore.Json` — небольшая NuGet-библиотека: конвертер `System.Text.Json`
для **десериализации** `IPagedList<T>` из пакета `Calabonga.PagedListCore`.
Публикуется в nuget.org: <https://www.nuget.org/packages/Calabonga.PagedListCore.Json>

- TFM: `netstandard2.1`, `LangVersion=latest` (нужен для file-scoped namespaces и др.
  современного синтаксиса — netstandard2.1 по умолчанию даёт C# 8.0), `Nullable` включён.
- Версия пакета задаётся в `<Version>` в `src/Calabonga.PagedListCore.Json.csproj` (сейчас `2.0.0`).
- `GeneratePackageOnBuild=True` — при обычной сборке уже собирается `.nupkg`.

## Структура

- `src/Calabonga.PagedListCore.Json.slnx` — решение (новый формат `.slnx`), один проект.
- `src/Calabonga.PagedListCore.Json.csproj` — проект библиотеки.
- `src/PageListConverter.cs` — единственный класс `PageListConverter<T> : JsonConverter<IPagedList<T>>`.
  - `Read(...)` разбирает объект с полями `pageIndex`, `pageSize`, `totalCount`, `items`
    и возвращает `PagedList<T>`.
  - `Write(...)` намеренно бросает `NotImplementedException` — сериализация не поддерживается.
- `src/README.md` — уходит в пакет как `PackageReadmeFile`.
- `README.md` (корень) — публичный changelog проекта.
- `.github/workflows/main.yml` — CI: restore → build → **test** → pack → push в nuget.org
  при push в `main` (runner `windows-latest`, .NET `10.0.x`).

## Команды

Сборка:

```bash
dotnet build src/Calabonga.PagedListCore.Json.slnx -c Release
```

Тесты (проект тестов пока отсутствует — см. `workflow.md`, его нужно создать в
`src/Calabonga.PagedListCore.Json.Tests`):

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
- Changelog в корневом `README.md` отстаёт от `<Version>` (в нём только `v1.0.0`) — обновляй при
  выпуске.
- Юнит-тестов нет; CI-шаг `test` их подразумевает — упавший `dotnet test` останавливает публикацию.
