using UnityEngine;

/// <summary>
/// 창병 찌르기 시각 연출용 PooledObject.
/// 풀에서 꺼내면 창병 위치에서 적 방향으로 쭉 뻗었다 되돌아온 뒤 풀에 반환.
/// 데미지 판정은 LancerView에서 즉시 처리하므로 이 스크립트는 순수 비주얼.
/// </summary>
public class SpearThrust : PooledObject
{
    [Header("찌르기 연출")]
    [SerializeField] private float thrustDistance = 2.5f;
    [SerializeField] private float thrustDuration = 0.1f;
    [SerializeField] private float retractDuration = 0.15f;

    private Vector3 originPos;
    private Vector3 thrustTarget;
    private float timer;
    private Phase phase;

    private enum Phase { Idle, Thrust, Retract }

    /// <summary>
    /// 찌르기 연출을 시작합니다.
    /// </summary>
    /// <param name="from">창병 위치</param>
    /// <param name="toward">적 위치</param>
    public void Thrust(Vector3 from, Vector3 toward)
    {
        originPos = from;
        transform.position = from;

        Vector3 dir = (toward - from).normalized;
        thrustTarget = from + dir * thrustDistance;

        // 적 방향으로 회전 (스프라이트 세로 기본 → -90도 보정)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        timer = 0f;
        phase = Phase.Thrust;
    }

    private void Update()
    {
        if (phase == Phase.Idle) return;

        timer += Time.deltaTime;

        if (phase == Phase.Thrust)
        {
            float t = Mathf.Clamp01(timer / thrustDuration);
            transform.position = Vector3.Lerp(originPos, thrustTarget, t);

            if (t >= 1f)
            {
                timer = 0f;
                phase = Phase.Retract;
            }
        }
        else if (phase == Phase.Retract)
        {
            float t = Mathf.Clamp01(timer / retractDuration);
            transform.position = Vector3.Lerp(thrustTarget, originPos, t);

            if (t >= 1f)
            {
                phase = Phase.Idle;
                ReturnToPool();
            }
        }
    }

    public override void OnReturned()
    {
        phase = Phase.Idle;
        timer = 0f;
    }
}
