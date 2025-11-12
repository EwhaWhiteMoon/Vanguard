/*
 * 런타임 유닛 본체
 */
using System;
using System.Collections.Generic;

[Serializable]
public class UnitData
{
    public string Name;
    public Stat BaseStat;
    public Stat CurrentStat;
    public UnitGrade Grade;
    public UnitClass Class;
    

    public UnitData(string name, UnitClass unitClass, UnitGrade unitGrade)
    {
        this.Name = name;
        this.Class = unitClass;
        this.Grade = unitGrade;
    }
}
