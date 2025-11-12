using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitTester : MonoBehaviour, ICombatManager
{
    public GameObject unit;
    private List<UnitData> allyList = new List<UnitData>();
    private List<UnitData> enemyList = new List<UnitData>();
    public List<GameObject> units { get; private set; } = new List<GameObject>();

    void Start()
    {
        // allyList = new List<UnitData>
        // {
        //     new UnitData(),
        //     new UnitData(unit_to_test),
        //     new UnitData(unit_to_test)
        // };
        // enemyList = new List<UnitData>
        // {
        //     new UnitData(unit_to_test),
        //     new UnitData(unit_to_test),
        //     new UnitData(unit_to_test)
        // };
        
        // for(int i = 0; i < allyList.Count; i++)
        // {
        //     GameObject u = Instantiate(unit, new Vector3(-2, i - 2, 0), Quaternion.identity);
        //     u.GetComponent<UnitGameObj>().Init(allyList[i], 0, this);
        //     units.Add(u);
        // }
        
        // for(int i = 0; i < enemyList.Count; i++)
        // {
        //     GameObject u = Instantiate(unit, new Vector3(2, i - 2, 0), Quaternion.identity);
        //     u.GetComponent<UnitGameObj>().Init(enemyList[i], 1, this);
        //     units.Add(u);
        // }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
