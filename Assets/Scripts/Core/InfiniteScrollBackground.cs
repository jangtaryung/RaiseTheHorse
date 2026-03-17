using UnityEngine;

/// <summary>
/// 무한 스크롤 배경. SpriteRenderer 2~3장을 이어붙여 카메라가 이동하면 재배치합니다.
/// 패럴랙스(시차 효과) 지원: scrollSpeed로 카메라 대비 배경 이동 비율을 조절합니다.
///   - 1.0 = 카메라와 동일 속도 (전경)
///   - 0.5 = 절반 속도 (중경)
///   - 0.1 = 느리게 (원경/하늘)
///
/// 사용법:
///   1. 빈 GameObject 생성 → InfiniteScrollBackground 추가
///   2. 자식으로 동일한 배경 스프라이트 3개 배치 (나란히)
///   3. panels 배열에 3개 드래그
///   4. scrollSpeed 조절 (레이어별 다르게 설정하면 패럴랙스)
/// </summary>
public class InfiniteScrollBackground : MonoBehaviour
{
    [Header("배경 패널 (좌→우 순서로 나란히 배치된 스프라이트들)")]
    [SerializeField] private Transform[] panels;

    [Header("패럴랙스 속도 (1.0 = 카메라 동일, 0.0 = 고정)")]
    [Range(0f, 1f)]
    [SerializeField] private float scrollSpeed = 0.5f;

    private Camera cam;
    private float panelWidth;
    private float lastCamX;

    private void Start()
    {
        cam = Camera.main;

        if (panels == null || panels.Length < 2)
        {
            Debug.LogWarning("InfiniteScrollBackground: panels에 최소 2개 이상의 배경을 등록하세요.");
            enabled = false;
            return;
        }

        // 첫 번째 패널의 SpriteRenderer로 폭 계산
        var sr = panels[0].GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            panelWidth = sr.bounds.size.x;
        }
        else
        {
            Debug.LogWarning("InfiniteScrollBackground: panels[0]에 SpriteRenderer가 필요합니다.");
            enabled = false;
            return;
        }

        lastCamX = cam.transform.position.x;
    }

    private void LateUpdate()
    {
        if (cam == null) return;

        // 패럴랙스: 카메라 이동량의 일부만 배경에 반영
        float camX = cam.transform.position.x;
        float deltaX = camX - lastCamX;
        lastCamX = camX;

        // 배경 전체를 카메라 이동의 (1 - scrollSpeed) 비율만큼 반대로 움직임
        // scrollSpeed=1이면 배경이 카메라와 동일하게 이동 (= 월드에 고정)
        // scrollSpeed=0이면 배경이 카메라에 고정 (= 움직이지 않는 것처럼 보임)
        float parallaxOffset = deltaX * (1f - scrollSpeed);
        for (int i = 0; i < panels.Length; i++)
        {
            Vector3 pos = panels[i].position;
            pos.x -= parallaxOffset;
            panels[i].position = pos;
        }

        // 화면 밖으로 나간 패널을 반대쪽으로 재배치
        float halfView = cam.orthographicSize * cam.aspect;
        float camLeft = camX - halfView;
        float camRight = camX + halfView;

        for (int i = 0; i < panels.Length; i++)
        {
            float panelRight = panels[i].position.x + panelWidth * 0.5f;
            float panelLeft = panels[i].position.x - panelWidth * 0.5f;

            // 패널이 완전히 왼쪽으로 벗어나면 → 가장 오른쪽 패널 뒤로 이동
            if (panelRight < camLeft)
            {
                float maxX = GetRightmostX();
                Vector3 pos = panels[i].position;
                pos.x = maxX + panelWidth;
                panels[i].position = pos;
            }
            // 패널이 완전히 오른쪽으로 벗어나면 → 가장 왼쪽 패널 앞으로 이동
            else if (panelLeft > camRight)
            {
                float minX = GetLeftmostX();
                Vector3 pos = panels[i].position;
                pos.x = minX - panelWidth;
                panels[i].position = pos;
            }
        }
    }

    private float GetRightmostX()
    {
        float max = float.MinValue;
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].position.x > max)
                max = panels[i].position.x;
        }
        return max;
    }

    private float GetLeftmostX()
    {
        float min = float.MaxValue;
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].position.x < min)
                min = panels[i].position.x;
        }
        return min;
    }
}
