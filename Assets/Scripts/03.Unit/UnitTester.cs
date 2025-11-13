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
        enemyList = new List<UnitData>
        {
            new UnitData("Troll", UnitClass.Warrior, UnitGrade.Common),
            new UnitData("Wolf", UnitClass.Archer, UnitGrade.Common),
            new UnitData("Zombie", UnitClass.Mage, UnitGrade.Common)
        };
        
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
            GameManager.Instance.GameState = GameState.StartMenu;
        }
        else
        {
            GameManager.Instance.GameState = GameState.GameOver;
        }
    }
}
