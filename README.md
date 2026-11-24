# Mobile AR Solar System Interactive Map

[![Unity](https://img.shields.io/badge/Unity-2022.3%20LTS-000000?style=flat&logo=unity&logoColor=white)](https://unity.com)
[![C#](https://img.shields.io/badge/C%23-.NET%20Standard%202.1-239120?style=flat&logo=dotnet&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/)
[![ARFoundation](https://img.shields.io/badge/ARFoundation-ARCore%20Android-green?style=flat)](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@latest)
[![DOTween](https://img.shields.io/badge/Animation-DOTween%20Pro-red?style=flat)](http://dotween.demigiant.com/)

> **Note:** В данном репозитории представлен **архитектурный срез и ядро кодовой базы на C# (`Assets/Scripts`)** для технического ревью (Code Review). Тяжелые 3D-ассеты планет, текстуры высокого разрешения (4K/8K) и сторонние платные плагины исключены из публичной индексации Git.

---

## 📌 О проекте

Интерактивное образовательное мобильное приложение дополненной реальности (AR), разработанное в качестве **выпускной квалификационной работы (ВКР)** бакалавра по направлению «Прикладная информатика (в экономике)» (Тольяттинская академия управления, 2024).

Приложение проецирует интерактивную 3D-модель Солнечной системы на горизонтальные поверхности в реальном масштабе окружения, моделирует орбитальные траектории и осевое вращение планет и выводит образовательные карточки при взаимодействии.

---

## 🏗 Архитектура приложения (System Flow)

```mermaid
flowchart TD
    ARCamera["AR Camera (ARCore Video Feed)"] --> PlaneManager["ARPlaneManager (Horizontal Plane Tracking)"]
    PlaneManager --> Raycast["ARRaycastManager (Hit Testing on Planes)"]
    Raycast --> Placement["ARPlanePlacementController (Screen Tap Raycast)"]
    Placement --> Spawn["Spawn Solar System Root (DOTween Ease.OutBack)"]
    Spawn --> DisablePlanes["Disable Plane Visualizers (Draw Call Saving)"]
    
    Data["PlanetDataSO (Orbital Periods & Textual Info)"] --> Orbit["OrbitalMotionController (Zero-Allocation Update)"]
    Orbit --> Selection["Planet Tap / Interaction Trigger"]
    Selection --> Tween["Procedural Selection Pulse (DOTween)"]
    Selection --> UI["PlanetInfoView (uGUI Popup Modal Animation)"]
```

---

## 💡 Ключевые инженерные решения

### 1. Точная детекция поверхностей и оптимизация рендеринга ARCore
* Использование `ARRaycastManager` с фильтрацией по полигонам плоскостей (`PlaneWithinPolygon`).
* Сразу после успешного спавна Солнечной системы визуализаторы плоскостей ARCore отключаются программно — это экономит до 25% вызовов отрисовки (Draw Calls) на мобильных GPU.

### 2. Производительная симуляция без GC Alloc
* Орбитальные расчеты (`OrbitalMotionController`) выполняются на тригонометрических функциях с кэшированием структуры `Vector3`, полностью исключая выделение памяти (Garbage Collection) в цикле `Update()`.
* Анимации спавна, фокуса на планетах и карточек интерфейса реализованы через `DOTween` (с обязательным переиспользованием и очисткой твинов в `OnDestroy`).

### 3. Чистая архитектура данных (ScriptableObjects)
* Параметры всех небесных тел (периоды обращения, наклоны осей, радиусы, описание, цветовые схемы орбит) вынесены в `PlanetDataSO`, что позволяет легко расширять проект без правки программной логики.

---

## 📂 Структура кодовой базы

```text
Assets/Scripts/
├── AR/
│   └── ARPlanePlacementController.cs    # Детекция плоскостей, спавн и управление визуализацией ARCore
├── Configs/
│   └── PlanetDataSO.cs                  # ScriptableObject физических и текстовых параметров планет
├── Simulation/
│   └── OrbitalMotionController.cs       # Орбитальная механика, вращение и выбор планет без GC
└── UI/
    └── PlanetInfoView.cs                # Адаптивная карточка описания на uGUI Canvas с анимацией DOTween
```
