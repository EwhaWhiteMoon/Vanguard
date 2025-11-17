# 아이템 시스템 사용 가이드

## 📋 전체 흐름 개요

```
1. 구글 시트에서 아이템 데이터 준비 (이미 완료)
   ↓
2. ItemDatabase 씬에 추가 (자동으로 구글 시트 데이터 로드)
   ↓
3. 아이템 획득 (ItemAcquisitionHelper.AcquireItem)
   ↓
4. 아이템 보너스 자동 적용 (ItemBonusManager)
   ↓
5. 획득 목록 기록 (ItemAcquiredListManager)
   ↓
6. 유닛에 아이템 스텟 적용 (UnitItemHelper)
   ↓
7. 아이템 보너스 UI 표시 (ItemBonusDisplayUI)
```

---

## 1단계: 구글 시트 데이터 준비 (이미 완료!)

구글 시트에 아이템 데이터가 이미 있다면 별도 작업이 필요 없습니다!

### 구글 시트 아이템 데이터 형식

구글 시트의 `item` 시트에 다음 컬럼들이 있어야 합니다:
- `itemID` (int) - 아이템 고유 ID
- `Name` (string) - 아이템 이름
- `Job` (string) - 적용 직업 ("All", "Warrior", "Archer", "Mage", "Assassin", "Tanker", "Healer" 또는 한글)
- `Hp`, `Mp`, `Atk`, `Def`, `Speed`, `AtkSpeed`, `Crit`, `CritD`, `HpRegen`, `MpRegen` - 스탯 값

### Job 문자열 지원 형식

- **전체**: "All", "전체"
- **전사**: "Warrior", "전사"
- **궁수**: "Archer", "궁수"
- **마법사**: "Mage", "마법사"
- **암살자**: "Assassin", "암살자"
- **탱커**: "Tanker", "탱커"
- **힐러**: "Healer", "힐러"

---

## 2단계: 씬 설정

### ItemDatabase 설정

1. **씬에 빈 GameObject 생성:**
   - 이름: "ItemDatabase"
   - `ItemDatabase` 컴포넌트 추가

2. **끝!** 구글 시트 데이터가 자동으로 로드됩니다.

   ```
   ItemDatabase 컴포넌트
   └─ 자동 로드: GoogleSheetSO.asset의 itemList에서 모든 아이템 자동 로드
   ```

### ItemBonusManager, ItemAcquiredListManager 설정

- **자동 생성됨!** (MonoSingleton이므로 씬에 없으면 자동 생성)
- 수동으로 추가하려면:
  - 빈 GameObject 생성
  - `ItemBonusManager` 또는 `ItemAcquiredListManager` 컴포넌트 추가

---

## 3단계: 아이템 획득

### 아이템 획득 방법

아이템을 획득할 때는 `ItemAcquisitionHelper.AcquireItem()`을 호출합니다.

```csharp
// 아이템 획득 (보상 화면, 상자 오픈 등에서 호출)
ItemAcquisitionHelper.AcquireItem("1"); // itemID를 문자열로

// int itemID를 사용하는 경우
int itemId = 2;
ItemAcquisitionHelper.AcquireItem(itemId.ToString());
```

### 동작 원리

```
ItemAcquisitionHelper.AcquireItem(itemId)
  ↓
ItemDatabase.GetItemById(itemId) → 구글 시트 데이터 조회
  ↓
JobParser.Parse(sheetItem.Job) → Job enum 변환
  ↓
item 데이터 → StatData 변환
  ↓
ItemBonusManager.AddItemBonus(job, bonus) → 스탯 보너스 누적
  ↓
ItemAcquiredListManager.AddItem(itemId) → 획득 목록 기록
```

### 획득한 아이템 확인

```csharp
// 특정 아이템을 몇 개 획득했는지 확인
int count = ItemAcquiredListManager.Instance.GetItemCount("1");

// 모든 획득한 아이템 목록 확인
foreach (var kvp in ItemAcquiredListManager.Instance.GetAllItemCounts())
{
    Debug.Log($"아이템 ID: {kvp.Key}, 개수: {kvp.Value}");
}
```

---

## 4단계: 유닛에 아이템 스텟 적용

### UnitObj와 연결하기

`UnitItemHelper`를 사용하여 유닛의 기본 Stat에 아이템 보너스를 쉽게 적용할 수 있습니다.

#### 방법 1: UnitTester에서 적용 (권장)

UnitTester에서 유닛을 생성할 때 UnitClass를 알고 있으므로, 여기서 아이템 보너스를 적용합니다.

```csharp
// UnitTester.cs의 CombatStart() 수정 예시
public void CombatStart()
{
    OnCombat = true;
    allyList = new List<UnitData>
    {
        new UnitData("Warrior", UnitClass.Warrior, UnitGrade.Common),
        new UnitData("Archer", UnitClass.Archer, UnitGrade.Common),
        new UnitData("Mage", UnitClass.Mage, UnitGrade.Common)
    };
    
    for(int i = 0; i < allyList.Count; i++)
    {
        GameObject u = Instantiate(unit, new Vector3(-2, i - 2, 0), Quaternion.identity);
        UnitObj unitObj = u.GetComponent<UnitObj>();
        unitObj.Init(allyList[i], 0, this);
        
        // 아이템 보너스 적용
        // UnitClass를 직접 전달 (UnitData.Class는 private이므로 생성 시 사용한 값 사용)
        UnitClass unitClass = GetUnitClassFromName(allyList[i].Name); // 또는 별도로 저장
        unitObj.stat = UnitItemHelper.ApplyItemBonusToStat(unitObj.stat, unitClass);
        
        units.Add(u);
    }
}

// 유닛 이름으로 UnitClass 추론 (또는 별도로 저장)
private UnitClass GetUnitClassFromName(string name)
{
    // 예시: 이름으로 추론
    if (name.Contains("Warrior")) return UnitClass.Warrior;
    if (name.Contains("Archer")) return UnitClass.Archer;
    if (name.Contains("Mage")) return UnitClass.Mage;
    // ... 등등
    return UnitClass.Warrior;
}
```

#### 방법 2: UnitObj에 UnitClass 저장 후 사용

UnitObj에 UnitClass를 저장하는 필드를 추가하면 더 편리합니다.

```csharp
// UnitObj.cs에 추가
public UnitClass unitClass; // UnitClass 저장

// UnitTester.cs에서
for(int i = 0; i < allyList.Count; i++)
{
    GameObject u = Instantiate(unit, new Vector3(-2, i - 2, 0), Quaternion.identity);
    UnitObj unitObj = u.GetComponent<UnitObj>();
    unitObj.unitClass = UnitClass.Warrior; // UnitClass 저장
    unitObj.Init(allyList[i], 0, this);
    
    // 아이템 보너스 적용
    unitObj.stat = UnitItemHelper.ApplyItemBonusToStat(unitObj.stat, unitObj.unitClass);
    
    units.Add(u);
}
```

### UnitItemHelper 사용법

```csharp
// UnitClass를 Job으로 변환
UnitClass unitClass = UnitClass.Warrior;
Job job = UnitItemHelper.UnitClassToJob(unitClass);

// 기본 Stat에 아이템 보너스 적용
Stat baseStat = unitData.BaseStat;
Stat finalStat = UnitItemHelper.ApplyItemBonusToStat(baseStat, job);

// 또는 UnitClass를 직접 사용
Stat finalStat = UnitItemHelper.ApplyItemBonusToStat(baseStat, unitClass);
```

### 중요 포인트

- **Job.All 보너스는 자동 포함됨**
  - `GetItemBonus(Job.Warrior)` 호출 시:
    - `Job.All` 보너스 + `Job.Warrior` 보너스 = 총합 반환

- **보너스는 누적됨**
  - 아이템을 여러 개 획득하면 보너스가 계속 누적됩니다.
  - 예: 아이템1 (Atk +10) + 아이템2 (Atk +5) = 총 Atk +15

---

## 5단계: UI 설정

### 보너스 표시 UI (ItemBonusDisplayUI)

현재 누적된 아이템 보너스를 실시간으로 표시하는 UI입니다.

#### 설정 방법

1. **UI 생성:**
   ```
   Canvas
   └─ ItemBonusPanel (Panel)
      └─ BonusText (TextMeshProUGUI) - 보너스 정보 표시
   ```

2. **ItemBonusDisplayUI 컴포넌트 추가:**
   - ItemBonusPanel에 `ItemBonusDisplayUI` 컴포넌트 추가
   - 인스펙터에서:
     - Bonus Text: BonusText 연결
     - Display Job: 표시할 직업 선택
     - Auto Update: 자동 업데이트 여부
     - Update Interval: 업데이트 주기 (초)

#### 사용 예시

```csharp
// 특정 직업의 보너스 표시
ItemBonusDisplayUI displayUI = GetComponent<ItemBonusDisplayUI>();
displayUI.SetDisplayJob(Job.Warrior);
displayUI.UpdateDisplay(); // 수동 업데이트
```

---

## 📝 실제 사용 예시 시나리오

### 시나리오 1: 유닛 스폰 시 스탯 적용

```csharp
public class UnitSpawner : MonoBehaviour
{
    public void SpawnUnit(UnitClass unitClass, Vector3 position)
    {
        GameObject unitObj = Instantiate(unitPrefab, position, Quaternion.identity);
        UnitObj unit = unitObj.GetComponent<UnitObj>();
        
        // 유닛의 기본 스탯 로드
        Stat baseStat = LoadUnitBaseStat(unitClass);
        
        // 아이템 보너스 적용
        Stat finalStat = UnitItemHelper.ApplyItemBonusToStat(baseStat, unitClass);
        unit.stat = finalStat;
        
        unit.Initialize();
    }
}
```

### 시나리오 2: 새 게임 시작 시 초기화

```csharp
public class GameManager : MonoBehaviour
{
    void StartNewGame()
    {
        // 아이템 보너스 초기화
        ItemBonusManager.Instance.ResetBonuses();
        
        // 획득 아이템 목록 초기화
        ItemAcquiredListManager.Instance.ResetAll();
    }
}
```

---

## ⚠️ 주의사항

1. **ItemDatabase 설정 필수**
   - 씬에 `ItemDatabase` 컴포넌트가 있어야 합니다.
   - 구글 시트 데이터는 자동으로 로드되지만, `GoogleSheetManager`가 씬에 있어야 합니다.

2. **ItemId 중복 금지**
   - 같은 ItemId를 가진 아이템이 여러 개 있으면 경고가 출력됩니다.
   - 구글 시트에서 중복된 itemID가 있으면 첫 번째 것만 사용됩니다.

3. **보너스는 누적됨**
   - 아이템을 여러 번 지급하면 보너스가 계속 누적됩니다.
   - 새 라운드 시작 시 초기화하려면 `ItemBonusManager.Instance.ResetBonuses()` 호출

4. **Job.All vs 특정 직업**
   - `Job.All` 아이템은 모든 직업에게 적용됩니다.
   - 특정 직업 아이템은 해당 직업에게만 적용됩니다.
   - 둘 다 있으면 합산되어 적용됩니다.

5. **구글 시트 데이터 업데이트**
   - 구글 시트를 업데이트한 후 `GoogleSheetManager`에서 `FetchGoogleSheet` 실행
   - 게임 재시작 시 자동으로 최신 데이터가 로드됩니다.

---

## 🔧 문제 해결

### Q: 유닛 스탯이 올라가지 않아요
- `UnitItemHelper.ApplyItemBonusToStat()`를 호출하고 있는지 확인
- `ItemBonusManager`에 보너스가 등록되어 있는지 확인
- 유닛의 `UnitClass`가 올바른지 확인

### Q: 보너스가 계속 누적되어요
- 새 라운드/스테이지 시작 시 `ItemBonusManager.Instance.ResetBonuses()` 호출

---

## 📚 관련 클래스 요약

| 클래스 | 역할 |
|--------|------|
| `ItemDatabase` | GoogleSheetSO.asset의 itemList를 직접 참조하여 아이템 조회 |
| `ItemBonusManager` | 아이템 보너스 누적 및 조회 |
| `ItemAcquiredListManager` | 플레이어가 획득한 아이템 목록 관리 (itemID별 개수) |
| `ItemAcquisitionHelper` | 아이템 획득 시 보너스 적용 및 목록 기록 처리 |
| `UnitItemHelper` | 유닛 시스템과 아이템 시스템 연결 헬퍼 (UnitClass↔Job, StatData↔Stat 변환) |
| `ItemBonusDisplayUI` | 현재 누적된 아이템 보너스 표시 UI |
| `JobParser` | 구글 시트의 Job 문자열을 Job enum으로 변환하는 유틸리티 |
| `Job` | 직업 enum |
| `StatData` | 스탯 데이터 구조체 |
| `GoogleSheetSO` | 구글 시트 데이터를 담는 SO |
| `GoogleSheetManager` | 구글 시트 데이터 로드 및 관리 |
| `item` | 구글 시트의 item 데이터 클래스 (GoogleSheetClass.cs에서 생성) |

---

## 🚀 빠른 시작 체크리스트

1. ✅ 구글 시트에 아이템 데이터가 있는지 확인
2. ✅ 씬에 `GoogleSheetManager`가 있는지 확인 (구글 시트 데이터 로드용)
3. ✅ 씬에 `ItemDatabase` 컴포넌트 추가
4. ✅ Play 모드 실행 후 Console에서 `[ItemDatabase] GoogleSheetSO.asset에서 X개의 아이템을 로드했습니다.` 확인
5. ✅ 유닛 스폰 시 `UnitItemHelper.ApplyItemBonusToStat()` 호출하여 아이템 보너스 적용

---

## 💡 사용 예시

### 구글 시트 데이터 예시

```
itemID | Name      | Job     | Hp | Atk | Def | ...
-------|-----------|---------|----|-----|-----|----
1      | 전사의 검  | Warrior | 50 | 10  | 5   | ...
2      | 마법사의 지팡이 | Mage | 30 | 15  | 3   | ...
3      | 전체 버프 | All     | 100| 0   | 0   | ...
```

### 코드에서 사용

```csharp
// 아이템 획득
ItemAcquisitionHelper.AcquireItem("1");

// 획득한 아이템 개수 확인
int count = ItemAcquiredListManager.Instance.GetItemCount("1");

// 아이템 조회
item sheetItem = ItemDatabase.Instance.GetItemById("1");
if (sheetItem != null)
{
    Debug.Log($"아이템 이름: {sheetItem.Name}");
    Debug.Log($"스탯: Hp={sheetItem.Hp}, Atk={sheetItem.Atk}");
}

// 유닛에 아이템 보너스 적용
Stat baseStat = unitData.BaseStat;
Stat finalStat = UnitItemHelper.ApplyItemBonusToStat(baseStat, UnitClass.Warrior);
unitObj.stat = finalStat;

// 아이템 보너스 조회
StatData itemBonus = ItemBonusManager.Instance.GetItemBonus(Job.Warrior);
```

---

이제 아이템 시스템을 사용할 준비가 되었습니다! 🎮

**핵심**: GoogleSheetSO.asset의 item 데이터를 직접 사용하므로, 별도로 아이템 SO를 만들 필요가 없습니다!
