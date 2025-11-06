/*
 * 버프&디버프 추상클래스
*/
using UnityEngine;
public abstract class StatusEffect : ScriptableObject
{
    public string DisplayName;  // 상태효과의 이름 표시
    public float Duration = 3f;  // 상태효과 지속시간
    public bool IsStackable = true;  //중첩가능여부

    public abstract void OnApply(Unit target, int stack);  //유닛에 효과 적용될 때 호출됨
    public abstract void OnExpire(Unit target, int stack);  // 지속시간이 끝날 때 호출 효과 해제
}
