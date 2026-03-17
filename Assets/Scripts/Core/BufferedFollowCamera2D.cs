using UnityEngine;

/// <summary>
/// 2D/사이드뷰용 버퍼(데드존) 카메라 추적.
/// 타겟이 데드존(rect) 안에 있으면 카메라 정지, 밖으로 나가면 데드존 경계까지만 따라옵니다.
/// </summary>
[DisallowMultipleComponent]
public class BufferedFollowCamera2D : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Buffer / Dead Zone (월드 단위)")]
    [Min(0f)] public float bufferX = 2.5f;
    [Min(0f)] public float bufferY = 1.5f;

    [Header("Offset (월드 단위)")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Smoothing")]
    [Tooltip("0이면 즉시 이동, 클수록 더 부드럽게 따라옵니다.")]
    [Min(0f)] public float smoothTime = 0.12f;

    private Vector3 velocity;

    private void Reset()
    {
        offset = new Vector3(0f, 0f, -10f);
        smoothTime = 0.12f;
        bufferX = 2.5f;
        bufferY = 1.5f;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 camPos = transform.position;
        Vector3 desired = camPos;

        Vector3 targetPos = target.position + offset;

        // 현재 카메라 중심 기준으로 데드존 계산 (월드 단위)
        float minX = camPos.x - bufferX;
        float maxX = camPos.x + bufferX;
        float minY = camPos.y - bufferY;
        float maxY = camPos.y + bufferY;

        if (targetPos.x < minX) desired.x = targetPos.x + bufferX;
        else if (targetPos.x > maxX) desired.x = targetPos.x - bufferX;

        if (targetPos.y < minY) desired.y = targetPos.y + bufferY;
        else if (targetPos.y > maxY) desired.y = targetPos.y - bufferY;

        // z는 offset 기준으로 고정(대부분 -10)
        desired.z = targetPos.z;

        if (smoothTime <= 0f)
        {
            transform.position = desired;
            velocity = Vector3.zero;
            return;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }
}

