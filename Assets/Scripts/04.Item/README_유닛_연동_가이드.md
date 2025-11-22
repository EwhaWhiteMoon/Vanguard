# 아이템/시너지 시스템 - 유닛 연동 가이드

## 📋 개요
이 문서는 **유닛 담당자**가 아이템/시너지 시스템을 유닛 배치에 연동하는 방법을 설명합니다.

---

## ✅ 현재 구현된 기능

### 1. 아이템 시스템
- `InventoryManager`: 획득한 아이템을 직업별로 관리
- `ItemBonusManager`: 직업별 아이템 스탯 보너스 누적
- `ItemEffectManager`: 아이템 획득 시 효과 처리

### 2. 시너지 시스템
- `SynergyManager`: 유니크 아이템 개수 기반 시너지 보너스 계산
- 자동으로 재계산됨 (아이템 획득 시)

### 3. 유닛-아이템 연동 헬퍼
- `UnitItemHelper`: 아이템/시너지 보너스를 유닛 스탯에 적용하는 헬퍼 클래스

### 4. 유닛 저장소
- `PlayerUnitRoster`: 보상으로 획득한 유닛을 저장 (`unit` 타입)

---

## 🔧 유닛 배치 시 필요한 작업

### 작업 1: UnitObj.Init()에서 아이템/시너지 보너스 적용

**현재 코드:**
```csharp
// Assets/Scripts/03.Unit/UnitObj.cs
public void Init(UnitData data, int team, ICombatManager combatManager, float HP = -1)
{
    this.unitData = data;
    Team = team;
    this.combatManager = combatManager;
    
    // ❌ 현재: 기본 스탯만 사용
    this.stat = new Stat(this.unitData.BaseStat);
    this.HP = HP == -1 ? this.unitData.BaseStat.MaxHealth : HP;
    
    animatorLoader.InitAnimator(data);
}
```

**수정 방법:**
```csharp
// Assets/Scripts/03.Unit/UnitObj.cs
public void Init(UnitData data, int team, ICombatManager combatManager, float HP = -1)
{
    this.unitData = data;
    Team = team;
    this.combatManager = combatManager;
    
    // ✅ 수정: 아이템 + 시너지 보너스 적용
    this.stat = UnitItemHelper.ApplyAllBonusesToStat(
        this.unitData.BaseStat, 
        this.unitData.GetUnitClass()
    );
    
    this.HP = HP == -1 ? this.stat.MaxHealth : HP;
    
    animatorLoader.InitAnimator(data);
}
```

**효과:**
- 유닛 스폰 시 자동으로 아이템/시너지 보너스가 적용됨
- 기본 스탯 + 아이템 보너스 + 시너지 보너스 = 최종 스탯

---

### 작업 2: PlayerUnitRoster의 유닛 사용 (선택사항)

**현재 상황:**
- `PlayerUnitRoster`에 보상으로 획득한 유닛이 저장됨 (`unit` 타입)
- 전투 시스템에서 하드코딩된 유닛을 사용 중

**활용 방법 (예시):**
```csharp
// UnitTester.cs 또는 전투 시스템
public void CombatStart()
{
    OnCombat = true;
    allyList = new List<UnitData>();
    
    // PlayerUnitRoster에서 획득한 유닛 가져오기
    var roster = PlayerUnitRoster.Instance;
    if (roster != null && roster.OwnedUnits.Count > 0)
    {
        foreach (var sheetUnit in roster.OwnedUnits)
        {
            // unit → UnitData 변환 필요
            UnitData data = ConvertUnitToUnitData(sheetUnit);
            allyList.Add(data);
        }
    }
    
    // 획득한 유닛이 없으면 기본 유닛 사용
    if (allyList.Count == 0)
    {
        allyList = new List<UnitData>
        {
            new UnitData("Warrior", UnitClass.Warrior, UnitGrade.Common),
            new UnitData("Archer", UnitClass.Archer, UnitGrade.Common),
            new UnitData("Mage", UnitClass.Mage, UnitGrade.Common)
        };
    }
    
    // ... 나머지 스폰 로직
}
```

**참고:**
- `unit` (구글 시트 데이터) → `UnitData` (런타임 데이터) 변환 로직이 필요할 수 있습니다.
- `UnitDatabase`를 활용하거나 새로운 변환 헬퍼를 만들어야 할 수 있습니다.

---

## 📚 주요 API 사용법

### UnitItemHelper 사용법

#### 1. 아이템 + 시너지 보너스 모두 적용
```csharp
Stat baseStat = unitData.BaseStat;
Job job = UnitItemHelper.UnitClassToJob(unitClass);
Stat finalStat = UnitItemHelper.ApplyAllBonusesToStat(baseStat, job);
```

#### 2. 아이템 보너스만 적용 (시너지 제외)
```csharp
Stat baseStat = unitData.BaseStat;
Job job = UnitItemHelper.UnitClassToJob(unitClass);
Stat finalStat = UnitItemHelper.ApplyItemBonusToStat(baseStat, job);
```

#### 3. UnitClass를 Job으로 변환
```csharp
Job job = UnitItemHelper.UnitClassToJob(UnitClass.Warrior);
// 결과: Job.Warrior
```

---

## 🔍 디버깅

### 로그 확인
아이템/시너지 보너스가 제대로 적용되는지 확인하려면 콘솔 로그를 확인하세요:

```
[ItemBonusManager] Warrior에게 보너스 추가: Hp:10, Atk:5, Crit:0.1(10%)
[SynergyManager] Warrior 시너지 계산 완료: 유니크 아이템 3개, 보너스 합계 = Atk:10, Crit:0.15(15%)
[UnitItemHelper] Warrior 최종 스탯 계산:
기본: Hp:100.0, Atk:10.0, ...
아이템 보너스: Hp:10, Atk:5, ...
시너지 보너스: Atk:10, ...
최종: Hp:110.0, Atk:25.0, ...
```

### 수동 확인
- `ItemBonusManager.Instance.DebugPrintAllBonuses()`: 현재 아이템 보너스 확인
- `SynergyManager.Instance.GetSynergyBonus(Job.Warrior)`: 특정 직업의 시너지 보너스 확인

---

## ⚠️ 주의사항

1. **초기화 순서**
   - `ItemBonusManager`, `SynergyManager`가 먼저 초기화되어야 합니다.
   - 유닛 스폰 전에 아이템이 획득되어 있어야 보너스가 적용됩니다.

2. **직업 매칭**
   - `UnitClass`와 `Job`은 같은 값들을 가지고 있습니다.
   - `UnitItemHelper.UnitClassToJob()`로 자동 변환됩니다.

3. **스탯 계산**
   - 기본 스탯 + 아이템 보너스 + 시너지 보너스 = 최종 스탯
   - 모든 보너스는 **더하기** 방식으로 적용됩니다.

---

## 📞 문의
문제가 발생하거나 추가 기능이 필요하면 아이템 시스템 담당자에게 문의하세요.

