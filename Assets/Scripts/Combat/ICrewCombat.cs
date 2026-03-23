using UnityEngine;

/// <summary>
/// 병종 공격 인터페이스. 지휘관(ChariotCombat/EnemyChariot)이 "공격해"만 명령하고,
/// 각 병종(궁병/창병/검병)이 자기 방식대로 공격을 실행합니다.
/// 플레이어→적, 적→플레이어 양방향 지원.
/// </summary>
public interface ICrewCombat
{
    bool IsReady();
    float GetEffectiveRange();

    /// <summary>플레이어 전차가 적을 공격할 때</summary>
    void ExecuteAttack(Vector3 targetPos, int targetId, EnemyManager enemyManager);

    /// <summary>적 전차가 플레이어를 공격할 때</summary>
    void ExecuteAttack(Vector3 targetPos, Chariot targetChariot);
}
