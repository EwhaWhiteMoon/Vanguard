using UnityEngine;

[CreateAssetMenu(menuName = "Game/Unit Config")]
public class UnitConfig : ScriptableObject
{
    [Header("Presentation")]
    public GameObject Prefab;  // 유닛이 사용할 프리팹 오브젝트

    [Header("Basic Info")]
    public string Name;  //유닛 이름
    public Sprite Portrait;  // Sprite 추가

    [Header("Team/Tags")]
    public int TeamId; // 0,1,2... 같은 간단한 팀 번호 표시

    [Header("Base Stats")]
    public StatBlock EntryStats = new(); // 유닛의 기본 스탯 세트, 인스펙터에서 기본치 입력

    [Header("Resource")]
    public bool UseResource = false;     // 마나/스태미너 사용 여부
    public string ResourceName = "Mana"; // 표기용

    [Header("Identity")]
    public UnitClass UnitClass;     //직업 및 클래스
    public UnitGrade Grade;         //unit 등급
}
