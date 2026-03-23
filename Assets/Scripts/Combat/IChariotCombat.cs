using UnityEngine;

/// <summary>
/// 전차 전투 공통 인터페이스.
/// 디버그 시각화 등에서 아군/적 전차를 동일하게 다룰 수 있도록 합니다.
/// </summary>
public interface IChariotCombat
{
    Transform transform { get; }
    void GetZoneBoundaries(out float swordsmanMax, out float lancerMax, out float archerMax);
}
