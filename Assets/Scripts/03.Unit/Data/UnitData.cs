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
    public Sprite sprite;
    private UnitGrade Grade;
    private UnitClass Class;

    [Header("방법: 파일 경로 사용 시")]
    public string filePath = "Assets/GenerateGoogleSheet/GoogleSheetJson.json";
    private GoogleSheetSO _so;

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
        this.sprite = Resources.Load<Sprite>($"Sprites/Units/{name}");
        InitializeBaseStat(unitClass, stat);
    }

    private void InitializeBaseStat(UnitClass unitClass = UnitClass.Warrior, Stat stat = null)
    {
        // 이거 따로 클래스로 빼야하긴 함.
        _so = GoogleSheetLoader.LoadFromFile(filePath);

        if (_so == null)
        {
            Debug.LogError("GoogleSheetSO 로드 실패");
            return;
        }

        // unitClass에 해당하는 유닛 정보 찾기
        // TODO : 이게 cell의 번호가 아니라 실제 unitID를 참조하게 하는 방법 없을까요? O(1)시간 안에 접근할 수 있게 미리 Dict를 만들어 둔다던가?
        if (_so.unitList.Count > 0 && _so.unitList.Count > (int)unitClass)
        {
            unit unitInfo = _so.unitList[(int)unitClass];

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
}
