# Карта проекта

Проект — Unity-пакет чит-панели. Игровой код в проекте отсутствует. `Assets/` содержит только пустую сцену и папку задач Unity Bridge.

## Структура

```
Project/
  Assets/
    Scenes/                  пустая сцена
    Editor/CoworkBridge/     задачи Unity Bridge (создаются автоматически)
  Docs/
    PROJECT_MAP.md           этот файл
  Packages/
    com.elmortem.cheatspanel/  пакет чит-панели
```

## Пакет com.elmortem.cheatspanel

```
Scripts/
  CheatsPanel.asmdef                сборка CheatPanel
  AssemblyInfo.cs                   InternalsVisibleTo для CheatPanel.InputSystem
  CheatsPanel.cs                    публичный API панели
  CheatsPanelBehaviour.cs           окно IMGUI и опрос ввода
  Input/
    ICheatsPanelInput.cs            интерфейс ввода
    CheatsPanelInput.cs             выбор и подмена провайдера ввода
    LegacyCheatsPanelInput.cs       провайдер на UnityEngine.Input
  InputSystem/
    CheatPanel.InputSystem.asmdef   сборка CheatPanel.InputSystem
    InputSystemCheatsPanelInput.cs  провайдер на UnityEngine.InputSystem
```

## Ключевые типы

| Тип | Доступ | Роль |
|---|---|---|
| `CheatsPanel` | public static | Регистрация читов (`Bind` / `Unbind`) и управление видимостью (`Show` / `Hide` / `Toggle`). Хранит группы читов. Создаёт объект панели при первом обращении. |
| `CheatsPanel.CheatEntry` | internal struct | Пара «имя чита — действие». |
| `CheatsPanelBehaviour` | internal MonoBehaviour | Рисует окно IMGUI. Опрашивает провайдер ввода в `Update`. Создаётся автоматически, в иерархии — объект `CheatsPanel`. |
| `ICheatsPanelInput` | public interface | Абстракция ввода: `IsToggleKeyDown`, `TouchCount`. |
| `CheatsPanelInput` | public static | Отдаёт текущий провайдер. Принимает провайдер проекта через `SetProvider`. |
| `LegacyCheatsPanelInput` | internal | Провайдер на старом `UnityEngine.Input`. |
| `InputSystemCheatsPanelInput` | internal | Провайдер на новом `UnityEngine.InputSystem`. Регистрирует себя в `RuntimeInitializeOnLoadMethod`. |

## Сборки и дефайны

| Сборка | Условия компиляции |
|---|---|
| `CheatPanel` | Компилируется всегда. Тело панели — под `#if CHEATS_ENABLE`. Класс `LegacyCheatsPanelInput` — под `#if ENABLE_LEGACY_INPUT_MANAGER`. |
| `CheatPanel.InputSystem` | `defineConstraints`: `CHEATS_ENABLE`, `ENABLE_INPUT_SYSTEM`, `CHEATS_INPUT_SYSTEM_PACKAGE`. Последний дефайн задан в `versionDefines` по пакету `com.unity.inputsystem`. Без пакета сборка не компилируется, и ссылка на `Unity.InputSystem` не разрешается. |

Дефайны `ENABLE_LEGACY_INPUT_MANAGER` и `ENABLE_INPUT_SYSTEM` задаёт Unity по настройке `Active Input Handling`. `CHEATS_ENABLE` добавляет разработчик в Scripting Define Symbols. Без `CHEATS_ENABLE` все вызовы `CheatsPanel` удаляет атрибут `[Conditional]`.

## Потоки данных

**Регистрация чита.** `CheatsPanel.Bind` → создание объекта панели при необходимости → запись в словарь групп `_groups` и порядок групп `_groupOrder`.

**Отрисовка.** `CheatsPanelBehaviour.OnGUI` → `CheatsPanel.CollectGroupNames` → для каждой группы `CheatsPanel.CollectCheats` → кнопка вызывает `CheatEntry.Action`. Исключение чита пишется в лог и не ломает панель.

**Ввод.** `CheatsPanelBehaviour.Update` → `CheatsPanelInput.Current` → `IsToggleKeyDown` или `TouchCount == 3` → `CheatsPanel.Toggle`.

**Выбор провайдера.** Приоритет: провайдер проекта из `SetProvider` → провайдер из `SetAutoProvider` (его ставит `InputSystemCheatsPanelInput` до загрузки первой сцены) → `LegacyCheatsPanelInput`. Если провайдера нет, `CheatsPanelBehaviour` ловит клавишу тильды через события IMGUI и переключает панель в следующем `Update`.

## Точки расширения

Проект со своим слоем ввода реализует `ICheatsPanelInput` и передаёт его в `CheatsPanelInput.SetProvider`. Значение `null` возвращает автоматический выбор.
