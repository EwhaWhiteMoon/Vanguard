using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager : MonoBehaviour, ICombatManager
{
    public GameObject unit;
    public GameObject enemyUnit;
    private List<UnitData> enemyList = new List<UnitData>();
    public List<GameObject> units { get; private set; } = new List<GameObject>();
    public List<RuntimeAnimatorController> animators = new List<RuntimeAnimatorController>();

    private Dictionary<int, List<UnitClass>> stageEnemyKindsMap;
    private Dictionary<int, UnitClass> stageBossKindsMap;

    public bool OnCombat = false;

    private MapManager mapManager;
    private int combatCnt;
    private bool isBossCleared;

    private int aliveTeam0;
    private int aliveTeam1;
    private bool combatEnded;

    public void OnGameStateChange(GameState state)
    {
        if (state == GameState.Combat)
        {
            Debug.Log($"[OnGameStateChange - CombatCheck] units.Count = {units.Count}, " +
          $"team0 = {units.Count(u => u && u.GetComponent<UnitObj>().Team == 0)}, " +
          $"team1 = {units.Count(u => u && u.GetComponent<UnitObj>().Team == 1)}");

            if (mapManager.getCurrentRoomType() == RoomType.BossRoom && GameManager.Instance.IsBossCleared)
            {
                GameManager.Instance.GameState = GameState.AfterCombat;
            }
            else
            {
                CombatStart();
            }
        } // Combat이라면 살아남
    }
    private void Awake()
    {
        InitStageMap();

        if(mapManager == null)
        {
            mapManager = FindFirstObjectByType<MapManager>();
        }

        GameManager.Instance.OnGameStateChange += OnGameStateChange;
        OnGameStateChange(GameManager.Instance.GameState); //지금 State에 맞게 한번 호출해줘야함.

    }

    public void ResetRoaster()
    {
        combatCnt = 0;
        isBossCleared = false;

        PlayerUnitRoster.Instance.AddUnit(new UnitData(UnitClass.Warrior.ToString(), UnitClass.Warrior, UnitGrade.Common));
        PlayerUnitRoster.Instance.AddUnit(new UnitData(UnitClass.Archer.ToString(), UnitClass.Archer, UnitGrade.Common));
        PlayerUnitRoster.Instance.AddUnit(new UnitData(UnitClass.Tanker.ToString(), UnitClass.Tanker, UnitGrade.Common));
    }

    void InitStageMap()
    {
        // 스테이지별 유닛 정보 생성
        stageEnemyKindsMap = new Dictionary<int, List<UnitClass>>
        {
            { 1, new List<UnitClass>
                {
                    UnitClass.Slime, UnitClass.Goblin, UnitClass.Wolf, UnitClass.GoblinArcher,
                }
            },
            { 2, new List<UnitClass>
                {
                    UnitClass.TrollWarrior, UnitClass.SkeletonSoldier, UnitClass.SkeletonArcher,
                }
            },
            { 3, new List<UnitClass>
                {
                    UnitClass.Slime, UnitClass.Goblin, UnitClass.Wolf, UnitClass.GoblinArcher,
                    UnitClass.TrollWarrior, UnitClass.SkeletonSoldier, UnitClass.SkeletonArcher
                }
            }
        };

        // 스테이지별 보스 정보 생성
        stageBossKindsMap = new Dictionary<int, UnitClass>
        {
            {1, UnitClass.TrollLeader},
            {2, UnitClass.SkeletonLeader},
            {3, UnitClass.Trassgo}
        };
    }

    public void CombatStart()
    {
        combatCnt++;
        aliveTeam0 = 0;
        aliveTeam1 = 0;
        combatEnded = false;
        OnCombat = true;

        bool isBoss = mapManager.getCurrentRoomType() == RoomType.BossRoom;
        // stage 정보 기입 필요.
        enemyList = MakeRandomEnemy(mapManager.floor, isBoss);

        // 직업별로 몇 번째 줄(Y)을 사용했는지 저장
        Dictionary<UnitClass, int> classRowIndex = new Dictionary<UnitClass, int>();

        var myUnits = PlayerUnitRoster.Instance.OwnedUnits;
        for (int i = 0; i < myUnits.Count; i++)
        {
            var data = myUnits[i].unitData;
            var hp   = myUnits[i].unitHP;

            UnitClass cls = data.Class;

            // 클래스별 행(index) 초기화
            if (!classRowIndex.ContainsKey(cls))
                classRowIndex[cls] = 0;

            // X, Y 계산
            float x = GetClassXPos(cls);
            float y = GetClassYPos(cls, classRowIndex[cls]);

            classRowIndex[cls]++;

            // 유닛 생성
            GameObject u = Instantiate(unit, new Vector3(x, y, 0), Quaternion.identity);
            u.GetComponent<UnitObj>().Init(data, 0, this, hp);
            units.Add(u);

            aliveTeam0++;
            u.GetComponent<UnitObj>().onDied -= HandleUnitDied;
            u.GetComponent<UnitObj>().onDied += HandleUnitDied;
        }

        for(int i = 0; i < enemyList.Count; i++)
        {
            GameObject u = Instantiate(enemyUnit, new Vector3(2, i - 2, 0), Quaternion.identity);
            if (isBoss) {
                u.GetComponent<UnitObj>().Init(enemyList[i], 1, this, -1, isBoss);
            }
            else
            {
                u.GetComponent<UnitObj>().Init(enemyList[i], 1, this);
            }
            units.Add(u);

            aliveTeam1++;
            u.GetComponent<UnitObj>().onDied -= HandleUnitDied;
            u.GetComponent<UnitObj>().onDied += HandleUnitDied;
        }
    }

    private void HandleUnitDied(UnitObj unit)
    {
        if (combatEnded) return;
        if (!OnCombat) return;

        if (unit.Team == 0)
            aliveTeam0--;
        else if (unit.Team == 1)
            aliveTeam1--;

        // 디버깅용
        Debug.Log($"[HandleUnitDied] Team0={aliveTeam0}, Team1={aliveTeam1}");

        if (aliveTeam1 <= 0 && aliveTeam0 > 0)
        {
            combatEnded = true;
            Debug.Log("Combat End (Team 1 Eliminated : WIN)");
            OnCombatDone(true); // 기존 메서드
        }
        else if (aliveTeam0 <= 0 && aliveTeam1 >= 0)
        {
            combatEnded = true;
            Debug.Log("Combat End (Team 0 Eliminated : LOSE)");
            OnCombatDone(false);
        }
    }

    private void OnCombatDone(bool win)
    {
        OnCombat = false;
        PlayerUnitRoster.Instance.ClearUnits();

        foreach (GameObject u in units)
        {
            if (u != null && u.GetComponent<UnitObj>().Team == 0) // ally 인경우
            {
                UnitObj allyObj = u.GetComponent<UnitObj>();
                UnitData allyData = allyObj.unitData;

                if (allyObj.HP > 0) {
                    PlayerUnitRoster.Instance.AddUnit(new UnitData(
                        allyData.Class.ToString(), allyData.Class, allyData.Grade), allyObj.HP);
                }
            }
        }

        foreach (GameObject u in units)
        {
            Destroy(u);
        }

        units.Clear();
        Debug.Log("Cleared all units from combat.");

        if (win)
        {
            GameManager.Instance.GameState = GameState.AfterCombat;
        }
        else
        {
            GameManager.Instance.GameState = GameState.GameOver;
        }
    }

    private List<UnitData> MakeRandomEnemy(int gameStage, bool isBoss)
    {
        // 게임 스테이지 1~3, enum으로 변경 필요. 나중에 맵 스테이지가 모두 합쳐지면 변경예정
        List<UnitData> enemyDataList = new List<UnitData>();
        System.Random rand = new System.Random();
        int enemyMaxCount = combatCnt > 2 ? gameStage * 5 : 3;

        List<UnitClass> enemyKinds = GetEnemies(gameStage, isBoss);
        int enemyCount = isBoss ? 1 : rand.Next(1, enemyMaxCount);

        for (int i = 0; i < enemyCount; i++)
        {
            int enemyKindIdx = rand.Next(0, enemyKinds.Count);
            enemyDataList.Add(new UnitData(
                enemyKinds[enemyKindIdx].ToString(), enemyKinds[enemyKindIdx], UnitGrade.Common));
        }

        return enemyDataList;
    }

    private List<UnitClass> GetEnemies(int gameStage, bool isBoss)
    {
        if (isBoss)
            return new List<UnitClass>{stageBossKindsMap[gameStage]};
        else
            return stageEnemyKindsMap[gameStage];
    }

    private float GetClassXPos(UnitClass cls)
    {
        switch (cls)
        {
            case UnitClass.Warrior:  return -2f;
            case UnitClass.Archer:   return -2.5f;
            case UnitClass.Mage:     return  -3f;
            case UnitClass.Assassin: return  -1.5f;
            case UnitClass.Tanker:   return  -1f;
            case UnitClass.Healer:   return  -3.5f;
            default: return 0f;
        }
    }

    private float GetClassYPos(UnitClass cls, int index)
    {
        return -1.5f * index; // (세로 간격 * 위치(순번))
    }
}
