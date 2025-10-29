/*
 * 버프지속관리
 */
using UnityEngine;
using System.Collections.Generic;

public class StatusRunner : MonoBehaviour
{
    private class Running
    {
        public StatusEffect Effect;
        public float TimeLeft;
        public int Stack;
    }

    private readonly List<Running> _running = new();  //현재 적용 중인 상태효과 목록
    private Unit _unit;

    private void Awake() => _unit = GetComponent<Unit>();  // 이 컴포넌트가 붙은 유닛 참조

    public void Apply(StatusEffect effect, float? durationOverride = null)  // 버프 적용 및 갱신
    {
        // 스택 규칙
        var run = _running.Find(r => r.Effect == effect);
        if (run == null)
        {
            run = new Running { Effect = effect, TimeLeft = durationOverride ?? effect.Duration, Stack = 1 };
            _running.Add(run);
            effect.OnApply(_unit, run.Stack);
        }
        else
        {
            if (effect.IsStackable) run.Stack++;
            run.TimeLeft = durationOverride ?? effect.Duration; // 갱신
            effect.OnApply(_unit, run.Stack); // 스택 갱신 반영(선택)
        }
    }

    private void Update()
    {
        for (int i = _running.Count - 1; i >= 0; i--)
        {
            var r = _running[i];
            r.TimeLeft -= Time.deltaTime;
            if (r.TimeLeft <= 0f)
            {
                r.Effect.OnExpire(_unit, r.Stack);
                _running.RemoveAt(i);
            }
        }
    }
}
