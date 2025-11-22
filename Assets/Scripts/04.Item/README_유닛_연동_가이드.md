# 🛠️ 유닛 시스템 연동 가이드 (아이템 & 시너지)

이 문서는 **유닛 담당자**가 아이템/시너지 시스템을 유닛에 적용하기 위해 **꼭 해야 할 작업**을 설명합니다.

---

## 🚀 핵심 요약 (이것만 하면 됩니다!)

1. **유닛 저장소 사용**: `PlayerUnitRoster.Instance.OwnedUnits`에서 획득한 유닛 목록을 가져오세요.
2. **스탯 자동 적용**: `UnitObj.Init()`을 호출하면 아이템/시너지 스탯이 **자동으로 적용**됩니다. (이미 구현됨)

---

## 1️⃣ 유닛 획득 및 배치 (전투 시작 시)

보상으로 얻은 유닛은 `PlayerUnitRoster`에 저장됩니다. 전투 시작 시 여기서 유닛을 꺼내 배치하세요.

### ✅ 코드 예시 (전투 매니저)

```csharp
void StartCombat() 
{
    // 1. 보유한 유닛 목록 가져오기
    var myUnits = PlayerUnitRoster.Instance.OwnedUnits;

    if (myUnits.Count == 0)
    {
        Debug.Log("보유한 유닛이 없습니다. 기본 유닛을 배치합니다.");
        // 기본 유닛 배치 로직...
        return;
    }

    // 2. 유닛 배치
    foreach (var sheetUnit in myUnits) 
    {
        // 유닛 프리팹 생성
        GameObject unitObj = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        
        // unit(시트 데이터) -> UnitData(인게임 데이터) 변환 (필요 시 구현)
        UnitData runtimeData = ConvertToUnitData(sheetUnit); 

        // 3. 초기화 (이때 아이템/시너지 스탯이 자동 적용됨!)
        unitObj.GetComponent<UnitObj>().Init(runtimeData, team: 0, this);
    }
}
```

---

## 2️⃣ 스탯 적용 확인 (이미 완료됨)

`UnitObj.cs`의 `Init` 함수는 이미 수정되어 있습니다. 유닛 담당자는 별도로 스탯 코드를 짤 필요가 없습니다.

**동작 원리:**
1. `Init()` 호출 시 `UnitItemHelper`가 동작합니다.
2. **아이템 보너스** + **시너지 보너스**를 계산합니다.
3. 유닛의 `stat`에 최종 값을 적용합니다.

```csharp
// UnitObj.cs 내부 (이미 구현됨)
public void Init(UnitData data, ...) 
{
    // 이 한 줄이 모든 보너스를 적용합니다.
    this.stat = UnitItemHelper.ApplyAllBonusesToStat(this.unitData.BaseStat, this.unitData.GetUnitClass());
}
```

---

## 🔍 디버깅 (제대로 적용됐는지 확인하기)

유닛이 스폰될 때 콘솔 로그를 확인하세요.

**정상 로그 예시:**
```text
[UnitItemHelper] Warrior 최종 스탯 계산:
기본: Hp:100, Atk:10
아이템 보너스: Hp:10, Atk:5 (아이템 효과)
시너지 보너스: Atk:10 (전사 시너지)
최종: Hp:110, Atk:25
```
위와 같은 로그가 뜨면 성공입니다!

---

## ⚠️ 주의사항

- **`PlayerUnitRoster` 사용 필수**: 유닛을 `List<UnitData>` 등으로 따로 관리하지 말고, `PlayerUnitRoster`를 사용해야 보상 시스템과 연동됩니다.
- **초기화 순서**: `Init()`을 호출하기 전에 아이템 매니저들이 씬에 존재해야 합니다. (이미 `Managers` 오브젝트에 정리되어 있습니다.)

---

## 📞 문의
연동 중 문제가 발생하면 **아이템 시스템 담당자**에게 문의해주세요.
