using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitTester : MonoBehaviour, ICombatManager
{
    public GameObject unit;
    public GameObject enemyUnit;
    private List<UnitData> allyList = new List<UnitData>();
    private List<UnitData> enemyList = new List<UnitData>();
    public List<GameObject> units { get; private set; } = new List<GameObject>();
    public List<RuntimeAnimatorController> animators = new List<RuntimeAnimatorController>();

    private List<UnitClass>enemykinds = new List<UnitClass>
    {
        UnitClass.Slime, UnitClass.Goblin, UnitClass.Wolf, UnitClass.GoblinArcher,
        UnitClass.OrcWarrior, UnitClass.OrcLeader, UnitClass.SkeletonSoldier, UnitClass.SkeletonArcher, UnitClass.SkeletonLeader
    };
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
        GameManager.Instance.OnGameStateChange += OnGameStateChange;
        OnGameStateChange(GameManager.Instance.GameState); //지금 State에 맞게 한번 호출해줘야함.

    }

    public void CombatStart()
    {
        OnCombat = true;
        allyList = new List<UnitData>
        {
            new UnitData("Warrior", UnitClass.Warrior, UnitGrade.Common),
            new UnitData("Archer", UnitClass.Archer, UnitGrade.Common),
            new UnitData("Mage", UnitClass.Mage, UnitGrade.Common)
        };
        enemyList = MakeRandomEnemy();

        for(int i = 0; i < allyList.Count; i++)
        {
            GameObject u = Instantiate(unit, new Vector3(-2, i - 2, 0), Quaternion.identity);
            u.GetComponent<UnitObj>().Init(allyList[i], 0, this);
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

    private List<UnitData> MakeRandomEnemy()
    {
        List<UnitData> enemyDataList = new List<UnitData>();

        System.Random rand = new System.Random();
        int enemyCount = rand.Next(1,5);

        for (int i = 0; i < enemyCount; i++)
        {
            int enemyKindIdx = rand.Next(0, enemykinds.Count);
            enemyDataList.Add(new UnitData(
                enemykinds[enemyKindIdx].ToString(), enemykinds[enemyKindIdx], UnitGrade.Common));
        }

        return enemyDataList;
    }
}
