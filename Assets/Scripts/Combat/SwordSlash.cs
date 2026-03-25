using UnityEngine;

/// <summary>
/// 검병 내려치기 시각 연출용 PooledObject.
/// 스프라이트 피봇이 중앙이어도, 코드에서 하단 피봇 회전을 시뮬레이션.
/// | (세로) → ㅡ (가로) 로 내려치는 연출.
/// 데미지 판정은 SwordsmanView에서 즉시 처리하므로 이 스크립트는 순수 비주얼.
/// </summary>
public class SwordSlash : PooledObject
{
    [Header("내려치기 연출")]
    [SerializeField] private float slashDuration = 0.1f;
    [SerializeField] private float bladeLength = 1.0f;

    private Vector3 pivotPos;
    private float startAngle;
    private float endAngle;
    private float timer;
    private bool slashing;

    /// <summary>
    /// 내려치기 연출을 시작합니다.
    /// </summary>
    /// <param name="from">검병 위치 (피봇 = 칼 하단)</param>
    /// <param name="toward">적 위치</param>
    public void Slash(Vector3 from, Vector3 toward)
    {
        pivotPos = from;

        // 적이 오른쪽이면 0 → -90 (시계방향 내려치기)
        // 적이 왼쪽이면 0 → 90 (반시계방향 내려치기)
        bool right = toward.x >= from.x;
        startAngle = 0f;
        endAngle = right ? -90f : 90f;

        timer = 0f;
        slashing = true;

        ApplyTransform(startAngle);
    }

    private void Update()
    {
        if (!slashing) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / slashDuration);

        float angle = Mathf.Lerp(startAngle, endAngle, t);
        ApplyTransform(angle);

        if (t >= 1f)
        {
            slashing = false;
            ReturnToPool();
        }
    }

    /// <summary>
    /// 피봇(하단)을 pivotPos에 고정하고, 칼날 중심을 반 길이만큼 위로 오프셋.
    /// 회전 시 하단을 축으로 돌리는 것처럼 보임.
    /// </summary>
    private void ApplyTransform(float angleDeg)
    {
        float angleRad = angleDeg * Mathf.Deg2Rad;
        float halfLen = bladeLength * 0.5f;

        // 세로(0도)일 때 오프셋 = (0, halfLen), 회전하면 오프셋도 같이 회전
        float offsetX = -Mathf.Sin(angleRad) * halfLen;
        float offsetY = Mathf.Cos(angleRad) * halfLen;

        transform.position = pivotPos + new Vector3(offsetX, offsetY, 0f);
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
    }

    public override void OnReturned()
    {
        slashing = false;
        timer = 0f;
    }
}
