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

	[NonSerialized] public Dictionary<string, unit> unitDict;
	[NonSerialized] public Dictionary<string, item> itemDict;
	[NonSerialized] public Dictionary<string, synergy> synergyDict;
	[NonSerialized] public Dictionary<string, unitTable> unitTableDict;
	public void BuildDictionaries()
	{
		unitDict = new Dictionary<string, unit>();
		if (unitList != null)
		{
			foreach (var temp in unitList)
			{
				if (!string.IsNullOrEmpty(temp.unitID))
					unitDict[temp.unitID] = temp;
			}
		}

		itemDict = new Dictionary<string, item>();
		if (itemList != null)
		{
			foreach (var temp in itemList)
			{
				if (!string.IsNullOrEmpty(temp.itemID))
					itemDict[temp.itemID] = temp;
			}
		}

		synergyDict = new Dictionary<string, synergy>();
		if (synergyList != null)
		{
			foreach (var temp in synergyList)
			{
				if (!string.IsNullOrEmpty(temp.synergyID))
					synergyDict[temp.synergyID] = temp;
			}
		}

		unitTableDict = new Dictionary<string, unitTable>();
		if (unitTableList != null)
		{
			foreach (var temp in unitTableList)
			{
				if (!string.IsNullOrEmpty(temp.unitTableID))
					unitTableDict[temp.unitTableID] = temp;
			}
		}

	}

	private void OnEnable()
	{
		BuildDictionaries();
	}
}

[Serializable]
public class unit
{
	public string unitID;
	public string Job;
	public int Hp;
	public int Atk;
	public int Def;
	public float Speed;
	public float AtkSpeed;
	public float Crit;
	public float CritD;
	public int HpRegen;
	public int MpRegen;
	public int Mp;
	public int Aggro;
	public float Range;
	public string Skill;
	public int Price;
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
	public int Price;
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

