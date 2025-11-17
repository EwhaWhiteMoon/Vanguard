using UnityEngine;

public class UnitAniController : MonoBehaviour
{
    private Animator _anim;

    // 파라미터 해시 캐싱
    private static readonly int HashIsAttack = Animator.StringToHash("isAttack");
    private static readonly int HashIsWalk   = Animator.StringToHash("isWalk");
    private static readonly int HashIsDie    = Animator.StringToHash("isDie");

    private enum AnimState
    {
        Idle,
        Walk,
        Attack,
        Die
    }

    private AnimState _currentState = AnimState.Idle;
    private bool _isDead = false;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void ResetAllTriggers()
    {
        // 필요하면 전부 리셋
        _anim.ResetTrigger(HashIsAttack);
        _anim.ResetTrigger(HashIsWalk);
        _anim.ResetTrigger(HashIsDie);
    }

    public void PlayIdle()
    {
        if (_isDead) return;

        if (_currentState == AnimState.Idle) return;
        _currentState = AnimState.Idle;
        ResetAllTriggers();
        // Idle은 보통 기본 상태라서 별도 트리거 없이 전환 조건으로 해결
    }

    public void PlayWalk()
    {
        if (_isDead) return;

        if (_currentState == AnimState.Walk) return;
        _currentState = AnimState.Walk;

        ResetAllTriggers();
        _anim.SetTrigger(HashIsWalk);
    }

    public void PlayAttack()
    {
        if (_isDead) return;

        if (_currentState == AnimState.Attack) return;
        _currentState = AnimState.Attack;

        ResetAllTriggers();
        _anim.SetTrigger(HashIsAttack);
    }

    public void PlayDie()
    {
        if (_isDead) return;        // 이미 죽었으면 무시
        _isDead = true;             // 이후 다른 애니메이션 요청은 모두 무시
        _currentState = AnimState.Die;

        ResetAllTriggers();
        _anim.SetTrigger(HashIsDie);
    }
}
