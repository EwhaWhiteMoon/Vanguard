using UnityEngine;

public static class UnitFactory
{
    // public static (Unit data, UnitBehaviour view) Create(UnitConfig config, int teamId, Vector3 pos, Quaternion rot)
    // {
    //     // 1) 데이터 생성
    //     config.TeamId = teamId; // 팩토리에서 팀 지정

    //     var data = new Unit(config);

    //     // 2) 프리팹 인스턴스
    //     var go = Object.Instantiate(config.Prefab, pos, rot);
    //     var behaviour = go.GetComponent<UnitBehaviour>();
    //     if (behaviour == null) behaviour = go.AddComponent<UnitBehaviour>();

    //     // 3) 위치 설정

    //     // 4) sprite / 스킨 설정

    //     // 5) 초기화
    //     behaviour.Initialize(data, config);

    //     return (data, behaviour);
    // }
}