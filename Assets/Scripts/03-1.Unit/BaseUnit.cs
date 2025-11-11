using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/BaseUnit")]
public class BaseUnit : ScriptableObject
{
    public string ID;
    public Sprite sprite;

    public Stat BaseStat;
}