using UnityEngine;

[DisallowMultipleComponent]
public class UnitBehaviour : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Renderer[] renderers; // 스킨 적용 대상
    [SerializeField] private Animator animator;

    // 순수 데이터 참조
    public Unit Data { get; private set; }
    public UnitConfig Config { get; private set; }

    public void Initialize(Unit data, UnitConfig config, TeamVisualSet.TeamSkin? skin = null)
    {
        Data = data;
        Config = config;

        if (skin.HasValue)
        {
            ApplySkin(skin.Value);
        }

        // 필요 시, 데이터 이벤트 → 이펙트 연결 (피격/사망 등)
        // Data.OnDamaged += OnDamaged; ...
    }

    private void ApplySkin(TeamVisualSet.TeamSkin skin)
    {
        if (skin.OverrideMaterial != null && renderers != null)
        {
            foreach (var r in renderers)
            {
                var mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; ++i) mats[i] = skin.OverrideMaterial;
                r.sharedMaterials = mats;
            }
        }
        if (renderers != null)
        {
            foreach (var r in renderers)
            {
                if (r.material.HasProperty("_Color")) r.material.color *= skin.Tint;
            }
        }
    }

    // 애니메이션 트리거만 담당 (일단 적어둠 추후 변경 있을 수 있음)
    public void PlayAttack() => animator?.SetTrigger("Attack");
    public void PlayHit()    => animator?.SetTrigger("Hit");
    public void PlayDie()    => animator?.SetTrigger("Die");
}