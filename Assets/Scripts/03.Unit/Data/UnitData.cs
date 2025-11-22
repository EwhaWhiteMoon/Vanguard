/*
 * 런타임 유닛 본체
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Rendering.Universal.Internal;

[Serializable] public class SheetMeta { public string title; public int version; }

[Serializable]
public class UnitData
{
    public string Name;
    public Stat BaseStat;
    public string AnimatorControllerName;
    public UnitGrade Grade;
    public UnitClass Class;

    [Header("방법: 파일 경로 사용 시")]
    public string filePath = "Assets/GenerateGoogleSheet/GoogleSheetJson.json";

    public UnitGrade GetUnitGrade()
    {
        return Grade;
    }
    public void SetUnitGrade(UnitGrade grade)
    {
        Grade = grade;
    }

    public UnitData(string name, UnitClass unitClass, UnitGrade unitGrade, Stat stat = null)
    {
        this.Name = name;
        this.Class = unitClass;
        this.Grade = unitGrade;
        InitializeBaseStat(unitClass, unitGrade, stat);
        this.AnimatorControllerName = GetAnimatorControllerName(unitClass, unitGrade);
    }

    private void InitializeBaseStat(UnitClass unitClass = UnitClass.Warrior, UnitGrade unitGrade = UnitGrade.Common, Stat stat = null)
    {
        GoogleSheetSO _so = GoogleSheetManager.SO<GoogleSheetSO>();

        if (_so == null)
        {
            Debug.LogError("GoogleSheetSO 로드 실패");
            return;
        }

        // GoogleSheetDebugger.PrintGoogleSheetSO(_so);

        // unitClass에 해당하는 유닛 정보 찾기
        if (_so.unitDict.TryGetValue(GetUnitId(unitClass, unitGrade), out unit unitInfo))
        {
            if (stat == null)
            {
                this.BaseStat = new Stat
                {
                    MaxHealth = unitInfo.Hp,
                    Attack = unitInfo.Atk,
                    Defense = unitInfo.Def,
                    MoveSpeed = unitInfo.Speed,
                    AttackSpeed = unitInfo.AtkSpeed,
                    CritChance = unitInfo.Crit,
                    CritMultiplier = unitInfo.CritD,
                    ManaMax = unitInfo.Mp,
                    Range = unitInfo.Range,
                };
            }
            else
            {
                this.BaseStat = new Stat(stat);
            }
        }
    }

    private string GetAnimatorControllerName(UnitClass unitClass = UnitClass.Warrior, UnitGrade unitGrade = UnitGrade.Common)
    {
        GoogleSheetSO _so = GoogleSheetManager.SO<GoogleSheetSO>();

        if (_so == null)
        {
            Debug.LogError("GoogleSheetSO 로드 실패");
            return "";
        }

        string aniCtrName = "";
        if (_so.unitDict.TryGetValue(GetUnitId(unitClass, unitGrade), out unit unitInfo))
        {
            aniCtrName = unitInfo.AniController;
        }

        return aniCtrName;
    }
    private string GetUnitId(UnitClass unitClass, UnitGrade unitGrade)
    {
        return $"{(int)unitClass}{(int)unitGrade}";
    }

}
