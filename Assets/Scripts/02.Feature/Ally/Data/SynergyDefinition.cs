/*
    * SynergyDefinition.cs
    * 
    * 유닛 시너지 정의를 위한 스크립트블 오브젝트
*/

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SynergyDefinition", menuName = "Scriptable Objects/SynergyDefinition")]
public class SynergyDefinition : ScriptableObject
{
    [Serializable]
    public struct Tier
    {
        public int CountRequired; // n 명 이상일 때
        public StatModifier[] Modifiers; // 부여되는 스탯 보너스
    }

    public UnitClass Class;
    public Tier[] Tiers;
}
