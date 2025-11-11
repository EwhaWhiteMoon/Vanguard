using System.Collections.Generic;
using UnityEngine;

public class UnitData
{
    public string Name;
    public Sprite sprite;
    public Stat stat;

    public UnitData(BaseUnit baseUnit)
    {
        this.Name = string.Copy(baseUnit.name);
        this.sprite = baseUnit.sprite;
        this.stat = new Stat(baseUnit.BaseStat);
    }
}