# 유닛 시스템 연동 가이드 (아이템 & 시너지)

본 문서는 아이템 및 시너지 시스템을 유닛 시스템에 통합하기 위한 기술 가이드입니다.
유닛 담당자는 아래 내용을 참고하여 **유닛 획득, 배치, 스탯 적용** 로직을 구현해야 합니다.

---

## 1. 시스템 개요

- **보상/상점 시스템**: 유닛과 아이템을 플레이어에게 지급합니다.
- **PlayerUnitRoster**: 플레이어가 획득한 유닛 데이터를 저장하는 싱글톤 매니저입니다.
- **UnitItemHelper**: 유닛 생성 시 아이템 및 시너지 보너스를 스탯에 적용하는 헬퍼 클래스입니다.

---

## 2. 구현 요구사항

유닛 담당자는 다음 두 가지 작업을 수행해야 합니다.

### 2.1. 유닛 획득 및 배치 (전투 시작 시)

전투 시작 시 `PlayerUnitRoster`에 저장된 유닛 목록을 가져와 전장에 배치해야 합니다.

```csharp
// 전투 매니저 (예: CombatManager.cs) 예시

void StartCombat() 
{
    // 1. 플레이어 보유 유닛 목록 가져오기
    var myUnits = PlayerUnitRoster.Instance.OwnedUnits;

    if (myUnits.Count == 0)
    {
        // 예외 처리: 보유 유닛이 없을 경우 (테스트용 유닛 배치 등)
        return;
    }

    // 2. 유닛 오브젝트 생성 및 초기화
    foreach (var sheetUnit in myUnits) 
    {
        // 유닛 프리팹 생성
        GameObject unitObj = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        
        // unit(구글 시트 데이터) -> UnitData(인게임 데이터) 변환 로직 필요
        UnitData runtimeData = ConvertToUnitData(sheetUnit); 

        // 3. 유닛 초기화 (UnitObj.Init 호출)
        // Init 메서드 내부에서 아이템/시너지 스탯이 자동 적용됩니다.
        unitObj.GetComponent<UnitObj>().Init(runtimeData, team: 0, this);
    }
}
```

### 2.2. 스탯 적용 로직 확인 (UnitObj.cs)

`UnitObj.Init` 메서드 내부에 아래 코드가 포함되어 있는지 확인합니다. (이미 구현되어 있으므로, 삭제되지 않도록 주의)

```csharp
// Assets/Scripts/03.Unit/UnitObj.cs

public void Init(UnitData data, int team, ICombatManager combatManager, float HP = -1)
{
    this.unitData = data;
    Team = team;
    this.combatManager = combatManager;
    
    // [핵심] 아이템 및 시너지 보너스를 적용하여 최종 스탯 계산
    this.stat = UnitItemHelper.ApplyAllBonusesToStat(this.unitData.BaseStat, this.unitData.GetUnitClass());
    
    this.HP = HP == -1 ? this.stat.MaxHealth : HP;
    
    animatorLoader.InitAnimator(data);
}
```

---

## 3. 데이터 흐름 요약

1. **획득**: 보상(`RewardPanel`) 또는 상점(`ShopManager`)에서 유닛 획득 -> `PlayerUnitRoster`에 저장됨.
2. **배치**: 전투 시작 시 `PlayerUnitRoster.OwnedUnits`를 순회하여 유닛 생성.
3. **적용**: 유닛 생성(`Init`) 시 `UnitItemHelper`가 현재 보유 아이템에 따른 추가 스탯을 계산하여 적용.

---

## 4. 디버깅

유닛이 스폰될 때 콘솔 로그를 통해 스탯 적용 여부를 확인할 수 있습니다.

**로그 예시:**
```text
[UnitItemHelper] Warrior 최종 스탯 계산:
기본: Hp:100, Atk:10
아이템 보너스: Hp:10, Atk:5
시너지 보너스: Atk:10
최종: Hp:110, Atk:25
```

---

## 5. 참고 사항

- **유닛 데이터 변환**: `PlayerUnitRoster`는 `unit` (구글 시트 데이터 클래스) 타입을 저장합니다. 게임 로직에서 `UnitData` 클래스를 사용한다면 변환 로직이 필요할 수 있습니다.
- **초기화 순서**: `Managers` 오브젝트(아이템, 골드, 시너지 매니저 포함)가 씬에 존재해야 정상 동작합니다.
