## Правила рабочего процесса

- Всегда создавай отдельную ветку Git перед внесением изменений. `main` — основная ветка, публикуется в NuGet через GitHub Actions при push.
- Допустимые префиксы веток: `feature/`, `bugfix/`, `hotfix/`.
- Формат коммитов: `type: description` (`feat`, `fix`, `refactor`, `test`, `docs`, `style`, `perf`, `build`, `chore`, `revert`).
- Атомарные коммиты — одно логическое изменение на коммит.
- Перед созданием нового класса проверь, нет ли файла с таким же именем в решении.
- Сборка:
  - `dotnet build src/Calabonga.PagedListCore.Json.slnx -c Release`
- Тестирование:
  - Если нет проекта с Unit-тестами, создай его в `src/Calabonga.PagedListCore.Json.Tests`.
  - Если нет Unit-тестов, добавь их. Если есть, убедись, что они проходят.
  - `dotnet test src/Calabonga.PagedListCore.Json.slnx -c Release` — после каждой реализации и обязательно перед фиксацией изменений. Если тесты падают, исправь их до фиксации. Если тесты падают после фиксации, исправь их в отдельной ветке и создай PR. 
- Версию пакета (`<Version>` в `Calabonga.PagedListCore.Json.csproj`) и changelog в `README.md` обновляй в том же PR, что и функциональные изменения.
- CI (`.github/workflows/main.yml`) на push в `main`: restore → build → **test** → pack → push в nuget.org. Тесты останавливают публикацию: упавший `dotnet test` останавливает пакет.
- Тестовый проект (`IsPackable=false`) в NuGet-пакет не входит — `dotnet pack` по решению собирает только пакет библиотеки.