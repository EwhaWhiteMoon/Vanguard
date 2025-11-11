using System.Collections.Generic;
using UnityEngine;

public class AllyManager : MonoBehaviour
{
    [SerializeField] private UnitConfig[] allyUnits;
    private readonly List<Unit> allies = new();                // 데이터 
    private readonly List<UnitBehaviour> allyViews = new();    // 뷰 컬렉션

    public void Spawn(UnitConfig cfg, Vector3 pos)
    {
        var (data, view) = UnitFactory.Create(cfg, teamId: 0, pos: pos, rot: Quaternion.identity);
        allies.Add(data);
        allyViews.Add(view);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        // 데이터만 틱 (상태이상/재생/쿨감 등)
        for (int i = allies.Count - 1; i >= 0; --i)
        {
            var u = allies[i];
            u.Tick(dt);

            if (u.CurrentHealth <= 0f)
            {
                // 사망 처리: 뷰에게 애니메이션 시키고, 제거
                var view = allyViews[i];
                view.PlayDie();
                Destroy(view.gameObject, 2f);
                allies.RemoveAt(i);
                allyViews.RemoveAt(i);
            }
        }
    }
}