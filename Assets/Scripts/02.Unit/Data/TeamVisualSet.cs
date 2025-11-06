using UnityEngine;

[CreateAssetMenu(fileName = "TeamVisualSet", menuName = "Game/Team Visual Set")]
public class TeamVisualSet : ScriptableObject
{
    [System.Serializable]
    public struct TeamSkin
    {
        public int TeamId;
        public Material OverrideMaterial;       // 통짜 교체용 머티리얼
        public Color Tint;                      // 머티리얼 컬러 곱
        
        // 아직 비쥬얼 적인거 정해지지 않아 주석처리
        // public RuntimeAnimatorController AnimatorOverride; // 팀별 애니메이터
        // public Sprite PortraitOverride;         // UI 초상화
        // public GameObject MuzzleVfx;            // 발사/히트 이펙트 등
    }

    public TeamSkin[] Skins;

    public bool TryGet(int teamId, out TeamSkin skin)
    {
        foreach (var s in Skins)
        {
            if (s.TeamId == teamId) { skin = s; return true; }
        }
        skin = default;
        return false;
    }
}
