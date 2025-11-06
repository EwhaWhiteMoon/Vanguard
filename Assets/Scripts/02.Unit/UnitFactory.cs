using UnityEngine;

public static class UnitFactory
{
    public static (Unit data, UnitBehaviour view) Create(UnitConfig config, int teamId, Vector3 pos, Quaternion rot)
    {
        // 1) 데이터 생성
        config.TeamId = teamId; // 팩토리에서 팀 지정

        var data = new Unit(config);

        // 2) 프리팹 인스턴스
        var go = Object.Instantiate(config.Prefab, pos, rot);
        var behaviour = go.GetComponent<UnitBehaviour>();
        if (behaviour == null) behaviour = go.AddComponent<UnitBehaviour>();

        // 3) 팀 스킨 적용
        TeamVisualSet.TeamSkin? skin = null;
        if (config.TeamVisualSet && config.TeamVisualSet.TryGet(teamId, out var s)) skin = s;

        // 4) 초기화
        behaviour.Initialize(data, config, skin);

        return (data, behaviour);
    }
}