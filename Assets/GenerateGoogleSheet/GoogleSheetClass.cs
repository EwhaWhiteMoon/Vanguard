using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>You must approach through `GoogleSheetManager.SO<GoogleSheetSO>()`</summary>
public class GoogleSheetSO : ScriptableObject
{
	public List<unit> unitList;
	public List<item> itemList;
	public List<synergy> synergyList;
	public List<unitTable> unitTableList;
}

[Serializable]
public class unit
{
	public string unitID;
	public string Job;
	public int Hp;
	public int Mp;
	public int Atk;
	public int Def;
	public float Speed;
	public float AtkSpeed;
	public float Crit;
	public float CritD;
	public int HpRegen;
	public int MpRegen;
	public int Aggro;
	public float Range;
	public string Skill;
	public string AniController;
}

[Serializable]
public class item
{
	public string itemID;
	public string Name;
	public string Job;
	public int Hp;
	public int Mp;
	public int Atk;
	public int Def;
	public float Speed;
	public int AtkSpeed;
	public float Crit;
	public float CritD;
	public float HpRegen;
	public int MpRegen;
	public string description;
}

[Serializable]
public class synergy
{
	public string synergyID;
	public string synergyName;
	public int requiredCount;
	public int Hp;
	public int Mp;
	public int Atk;
	public int Def;
	public int Speed;
	public int AtkSpeed;
	public float Crit;
	public float CritD;
	public float HpRegen;
	public int MpRegen;
}

[Serializable]
public class unitTable
{
	public string unitTableID;
	public string name;
	public int stage;
	public string count;
}

