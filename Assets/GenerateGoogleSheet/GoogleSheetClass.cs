using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>You must approach through `GoogleSheetManager.SO<GoogleSheetSO>()`</summary>
public class GoogleSheetSO : ScriptableObject
{
	public List<sheet1> sheet1List;
}

[Serializable]
public class sheet1
{
	public string ID;
	public string Name;
	public string SpriteID;
	public int Hp;
	public int Mp;
	public int Atk;
	public int Speed;
	public int Def;
	public int AtkSpeed;
	public int Crit;
	public string SkillId;
}

