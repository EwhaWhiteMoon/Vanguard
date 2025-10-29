using UnityEngine;

public enum SkillCastMode { OnManaFull, AutoInterval }

[CreateAssetMenu(fileName = "SkillBase", menuName = "Scriptable Objects/SkillBase")]
public abstract class SkillBase : ScriptableObject
{
    [Header("Presentation")]
    public string DisplayName;

    [Header("Cast")]
    public SkillCastMode CastMode = SkillCastMode.OnManaFull;
    public float ManaCost = 100f;         // OnManaFull 일 때만 의미
    public float IntervalSeconds = 5f;    // AutoInterval 일 때만 의미

    public abstract void Execute(Unit caster);
}
