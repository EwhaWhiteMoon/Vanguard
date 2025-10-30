/*
 * 버프&디버프 추상클래스
*/
using UnityEngine;
public abstract class StatusEffect : ScriptableObject
{
    public string DisplayName;  // unit 이름 표시
    public float Duration = 3f;  //
    public bool IsStackable = true;

    public abstract void OnApply(Unit target, int stack);
    public abstract void OnExpire(Unit target, int stack);
}
