using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class UnitTester : MonoBehaviour, ICombatManager
{
    public GameObject unit;
    public GameObject enemyUnit;
    private List<UnitData> allyList = new List<UnitData>();
    private List<UnitData> enemyList = new List<UnitData>();
    public List<GameObject> units { get; private set; } = new List<GameObject>();

    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
    }
}
