using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;

public class UnitTester : MonoBehaviour, ICombatManager
{
    public GameObject unit;
    public GameObject enemyUnit;
    private List<UnitData> allyList = null;
    private List<float> allyHPList = new List<float>();
    private List<UnitData> enemyList = new List<UnitData>();
    public List<GameObject> units { get; private set; } = new List<GameObject>();
    public List<RuntimeAnimatorController> animators = new List<RuntimeAnimatorController>();

    private Dictionary<int, List<UnitClass>> stageEnemyKindsMap;
    private Dictionary<int, UnitClass> stageBossKindsMap;

    public bool OnCombat = false;

    public void OnGameStateChange(GameState state)
    {
        if (state == GameState.Combat)
        {
            CombatStart();
        } // Combat이라면 살아남
    }
    private void Awake()
    {
        Debug.Log("매번 불림?");
        InitStageMap();

        // 유닛 초기화
        allyList = new List<UnitData>
        {
            new UnitData(UnitClass.Warrior.ToString(), UnitClass.Warrior, UnitGrade.Common),
            new UnitData(UnitClass.Archer.ToString(), UnitClass.Archer, UnitGrade.Common),
            new UnitData(UnitClass.Mage.ToString(), UnitClass.Mage, UnitGrade.Common)
        };
        for (int i = 0; i < allyList.Count; i++)
        {
            allyHPList.Add(-1);
        }

        GameManager.Instance.OnGameStateChange += OnGameStateChange;
        OnGameStateChange(GameManager.Instance.GameState); //지금 State에 맞게 한번 호출해줘야함.

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
        OnCombat = true;

        // stage 정보 기입 필요.
        enemyList = MakeRandomEnemy(1, false);

        for(int i = 0; i < allyList.Count && i < allyHPList.Count; i++)
        {
            GameObject u = Instantiate(unit, new Vector3(-2, i - 2, 0), Quaternion.identity);
            u.GetComponent<UnitObj>().Init(allyList[i], 0, this, allyHPList[i]);
            units.Add(u);
        }

        for(int i = 0; i < enemyList.Count; i++)
        {
            GameObject u = Instantiate(enemyUnit, new Vector3(2, i - 2, 0), Quaternion.identity);
            u.GetComponent<UnitObj>().Init(enemyList[i], 1, this);
            units.Add(u);
        }
    }

    private void Update()
    {
        if (!OnCombat) return;

        if(units.All(u => !u || u.GetComponent<UnitObj>().Team == 0 )) // 당연히!! 최적화해야 하지만 일단 작동함
        {
            Debug.Log("Combat End (Team 1 Eliminated : WIN)");
            OnCombatDone(true);
        }
        if(units.All(u => !u || u.GetComponent<UnitObj>().Team == 1 ))
        {
            Debug.Log("Combat End (Team 0 Eliminated : LOSE)");
            OnCombatDone(false);
        }
    }

    private void OnCombatDone(bool win)
    {
        OnCombat = false;

        allyList.Clear();
        allyHPList.Clear();

        foreach (GameObject u in units)
        {
            if (u != null && u.GetComponent<UnitObj>().Team == 0) // ally 인경우
            {
                UnitObj allyObj = u.GetComponent<UnitObj>();
                UnitData allyData = allyObj.unitData;

                if (allyObj.HP > 0) {
                    allyList.Add(new UnitData(allyData.Class.ToString(), allyData.Class, allyData.Grade));
                    allyHPList.Add(allyObj.HP);
                }
            }
        }

        foreach (GameObject u in units)
        {
            Destroy(u);
        }

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

        List<UnitClass> enemyKinds = GetEnemies(gameStage, isBoss);
        int enemyCount = isBoss ? 1 : rand.Next(1, gameStage*3);

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
}
