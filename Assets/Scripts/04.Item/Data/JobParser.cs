using UnityEngine;

/// <summary>
/// 구글 시트의 Job 문자열을 Job enum으로 변환하는 유틸리티 클래스입니다.
/// 
/// 지원 형식:
/// - "All", "전체" → Job.All
/// - "Warrior", "전사" → Job.Warrior
/// - "Archer", "궁수" → Job.Archer
/// - "Mage", "마법사" → Job.Mage
/// - "Assassin", "암살자" → Job.Assassin
/// - "Tanker", "탱커" → Job.Tanker
/// - "Healer", "힐러" → Job.Healer
/// </summary>
public static class JobParser
{
    /// <summary>
    /// 구글 시트의 Job 문자열을 Job enum으로 변환합니다.
    /// </summary>
    /// <param name="jobString">구글 시트의 Job 컬럼 값</param>
    /// <returns>변환된 Job enum, 알 수 없는 값이면 Job.All 반환</returns>
    public static Job Parse(string jobString)
    {
        if (string.IsNullOrEmpty(jobString))
            return Job.All;

        string lower = jobString.ToLower().Trim();

        switch (lower)
        {
            case "all":
            case "전체":
                return Job.All;

            case "warrior":
            case "전사":
                return Job.Warrior;

            case "archer":
            case "궁수":
                return Job.Archer;

            case "mage":
            case "마법사":
                return Job.Mage;

            case "assassin":
            case "암살자":
                return Job.Assassin;

            case "tanker":
            case "탱커":
                return Job.Tanker;

            case "healer":
            case "힐러":
                return Job.Healer;

            default:
                Debug.LogWarning($"[JobParser] 알 수 없는 Job 문자열: '{jobString}'. All로 설정합니다.");
                return Job.All;
        }
    }
}

