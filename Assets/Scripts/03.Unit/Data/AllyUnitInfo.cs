public class AllyUnitInfo
{
    public UnitData unitData;
    public float unitHP;

    public AllyUnitInfo(UnitData unitData, float HP = -1)
    {
        this.unitData = unitData;
        this.unitHP = HP;
    }
}