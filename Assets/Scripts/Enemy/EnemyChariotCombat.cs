using UnityEngine;

/// <summary>
/// 적 전차 전투 지휘관. ChariotCombat과 동일한 구조.
/// "누가 공격할지"만 결정하고, "어떻게 공격하느냐"는 각 View(ICrewCombat)에 위임.
/// 공격 권역: 검병(0~sMax) / 창병(sMax~lMax) / 궁병(lMax~aMax) 배타적 구간.
/// </summary>
public class EnemyChariotCombat : MonoBehaviour, IChariotCombat
{
    [Header("접근 정지 거리")]
    [SerializeField] private float stopDistance = 1.5f;

    [Header("승무원 참조")]
    [SerializeField] private ArcherView archerView;
    [SerializeField] private LancerView lancerView;
    [SerializeField] private SwordsmanView swordsmanView;

    private Chariot chariot;
    private Transform target;
    private Chariot targetChariot;

    // 캐싱된 권역 경계
    private float cachedSwordsmanMax;
    private float cachedLancerMax;
    private float cachedArcherMax;

    private void Awake()
    {
        var stats = GetComponent<ChariotStats>();
        if (stats != null)
            chariot = stats.GetChariot();
    }

    public void Init(Transform playerChariot, Chariot playerChariotModel)
    {
        target = playerChariot;
        targetChariot = playerChariotModel;

        if (chariot == null)
        {
            var stats = GetComponent<ChariotStats>();
            if (stats != null)
                chariot = stats.GetChariot();
        }

        var hitbox = GetComponent<ChariotHitbox>();
        if (hitbox != null && chariot != null)
            hitbox.SetChariot(chariot);

        RecalculateZones();
    }

    private void RecalculateZones()
    {
        cachedSwordsmanMax = swordsmanView != null ? swordsmanView.GetEffectiveRange() : 0f;
        cachedLancerMax = cachedSwordsmanMax + (lancerView != null ? lancerView.GetEffectiveRange() : 0f);
        cachedArcherMax = cachedLancerMax + (archerView != null ? archerView.GetEffectiveRange() : 0f);
    }

    public void SetPools(ObjectPool arrowPool, ObjectPool spearPool)
    {
        if (archerView != null) archerView.SetPool(arrowPool);
        if (lancerView != null) lancerView.SetPool(spearPool);
    }

    private void Update()
    {
        if (chariot == null || chariot.GetCurrentHP() <= 0f || target == null || targetChariot == null) return;

        // Battle 상태가 아니면 이동/공격 중단
        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.CurrentState != GameState.Battle)
            return;

        float dist = Mathf.Abs(target.position.x - transform.position.x);

        if (dist > stopDistance)
        {
            float dirX = target.position.x > transform.position.x ? 1f : -1f;
            transform.position += new Vector3(dirX * chariot.GetCurrentMoveSpeed() * Time.deltaTime, 0f, 0f);
        }

        if (dist <= cachedSwordsmanMax)
            TryAttack(swordsmanView);
        else if (dist <= cachedLancerMax)
            TryAttack(lancerView);
        else if (dist <= cachedArcherMax)
            TryAttack(archerView);
    }

    public void GetZoneBoundaries(out float swordsmanMax, out float lancerMax, out float archerMax)
    {
        if (cachedArcherMax > 0f)
        {
            swordsmanMax = cachedSwordsmanMax;
            lancerMax = cachedLancerMax;
            archerMax = cachedArcherMax;
            return;
        }

        swordsmanMax = swordsmanView != null ? swordsmanView.GetEffectiveRange() : 0f;
        lancerMax = swordsmanMax + (lancerView != null ? lancerView.GetEffectiveRange() : 0f);
        archerMax = lancerMax + (archerView != null ? archerView.GetEffectiveRange() : 0f);
    }

    private void TryAttack(ICrewCombat crew)
    {
        if (crew == null || !crew.IsReady()) return;
        crew.ExecuteAttack(target.position, targetChariot);
    }
}
