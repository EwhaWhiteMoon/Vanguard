## 🎮 Roguelike Auto-Battle Game

로그라이크 진행 방식 + 자동 전투가 결합된 전략 게임

<p align="center"> <img src="https://img.shields.io/badge/Unity-6.0-black?logo=unity" /> <img src="https://img.shields.io/badge/Genre-Roguelike%20%2B%20AutoBattle-blue" /> <img src="https://img.shields.io/badge/Platform-PC-lightgrey" /> <img src="https://img.shields.io/badge/Status-Development-green" /> </p> <p align="center"> 가볍게 즐길 수 있는 자동 전투 기반 로그라이크 게임 프로젝트 </p>


# 🗂️ Table of Contents
- [✨ Features](#-features)

- [🎲 Gameplay Overview](#-gameplay-overview)

- [⚙️ Installation & Run](#️-installation--run)

- [🧩 Project Structure](#-project-structure)

- [🗺 MiniMap System](#-minimap-system)

- [⚔️ Combat System](#combatmanager)

- [📡 Google Sheet SO Auto-Generation](#-google-sheet-so-auto-generation)

# ✨ Features
**✔ 자동 전투 기반 전투 시스템**

- 유저는 유닛을 수집/배치하고 전투는 자동으로 진행

- 턴 기반이 아닌 실시간 오토배틀 방식

**✔ 절차적 맵 + 미니맵 UI**

- RoomType 기반 맵 구성

- 플레이어 현재 위치를 표시하는 하이라이트 기능

- UI Image를 동적 생성하여 minimap 표시

**✔ Google Sheets → ScriptableObject 자동 변환**

- Google Sheets 값을 JSON 형태로 가져와 SO 자동 저장

- `[ContextMenu("FetchGoogleSheet")]` 로 간편 업데이트

**✔ 안정된 이벤트 기반 전투 종료 처리**

- `Update()` 폴링 방식 제거

- 유닛이 죽는 순간에만 전투 종료 여부 검사

- 방 이동 간 “전투 즉시 종료 버그” 해결

# 🎲 Gameplay Overview

- 층별 전투 진행 → 보스 처치 → 다음 층 이동

- 상점/보상에서 아이템 구매 및 유닛 신규 획득

- 메인 화면에서 유닛 스탯 강화

- 남은 HP가 다음 전투에도 이어지는 구조로 전략성 부여

- 로그라이크 특성에 따라 매 플레이마다 새로운 구성 제공

# ⚙️ Installation & Run

> ⚠️ 이 게임은 Build 파일이 아닌 프로젝트 실행 방식입니다.
반드시 Unity에서 직접 열고 Main 씬을 Play 해야 합니다.

> 빌드 파일을 만들어 보려고 했으나, 알수없는 버그를 고치지 못하여 불가피하게 Unity에서 직접 Main 씬을 Play 해야 합니다.

1. 프로젝트 다운로드
    ``` bash
    git clone https://github.com/your-repo/roguelike-autobattle.git
    ```

2. Unity Hub에서 프로젝트 열기

    Unity 버전: **`Unity 6`**

3. 메인 씬 실행
    ``` bash
    Assets/Scenes/mainScene.unity
    ```

4. ▶ 버튼 클릭 후 게임 시작
# 🧩 Project Structure
``` bash
Assets/
 ├── Scripts/
 │    ├── Combat/              # CombatManager, 전투 흐름 제어
 │    ├── Unit/                # UnitObj, UnitData, 스탯 처리
 │    ├── Minimap/             # 미니맵 시스템
 │    ├── GoogleSheet/         # SO 생성 자동화 기능
 │    └── Managers/            # MapManager, GameManager 등
 │
 ├── ScriptableObjects/        # 유닛/아이템 데이터 SO
 ├── Prefabs/                  # 유닛, UI, FX 등 프리팹
 ├── Sprites/                  # 스프라이트 에셋
 └── Scenes/
       └── Main.unity          # 게임 실행 메인 씬
```
# 🗺 MiniMap System

미니맵은 맵 크기에 따라 동적으로 생성되며, UI Panel 아래에 타일이 배치됩니다.

- 플레이어 위치 업데이트

- RoomType에 따라 스프라이트 표시

- `RefreshMiniMap()` 시 미니맵 전체 재구성
    ``` csharp
    tileGO.transform.SetParent(container, false);
    img.rectTransform.anchoredPosition = new Vector2(x * tileSize, y * tileSize);
    ```

# ⚔️ Combat System
## 🔥 기존 문제

전투 종료 판정을 `Update()`에서 검사하면:

- 방 이동 직후 유닛이 아직 없을 때 즉시 전투 종료 발생

- `All()`이 빈 리스트에서도 true를 반환하는 문제 발생

## 🔧 해결: 이벤트 기반 전투 종료 방식
## UnitObj

``` csharp
public event Action<UnitObj> OnDied;

void Die()
{
    OnDied?.Invoke(this);
}
```

## CombatManager
``` csharp
private void HandleUnitDied(UnitObj unit)
{
    if (unit.Team == 0) aliveTeam0--;
    else aliveTeam1--;

    if (aliveTeam1 <= 0 && aliveTeam0 > 0)
        OnCombatDone(true);   // WIN
    else if (aliveTeam0 <= 0 && aliveTeam1 > 0)
        OnCombatDone(false);  // LOSE
}
```

## 특징

- 불필요한 Update 호출 제거

- 유닛이 죽을 때만 로직 수행

- 방 이동 시 오작동 없음

- 퍼포먼스 개선

# 📡 Google Sheet SO Auto-Generation

데이터 관리 간소화를 위해 Google Sheets → SO 변환 기능을 구현했습니다.

## Editor 메뉴에서 자동 실행
``` csharp
#if UNITY_EDITOR
[ContextMenu("FetchGoogleSheet")]
async void FetchGoogleSheet()
{
    // Google Sheets → JSON → ScriptableObject 변환
}
#endif
```

장점

- 팀원 간 데이터 동기화 용이

- 별도 CSV 수동 변환 불필요

- SO 기반으로 런타임 로딩 안정적