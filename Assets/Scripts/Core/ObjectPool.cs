using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 어떤 GameObject 프리팹이든 재사용할 수 있는 기본 오브젝트 풀.
/// Enemy, 배경, 이펙트 등 모두 이 클래스를 재사용 가능합니다.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [Header("풀링할 프리팹 (PooledObject 상속 필요)")]
    public PooledObject prefab;

    [Header("초기 생성 개수")]
    public int initialSize = 10;

    private readonly Queue<PooledObject> pool = new Queue<PooledObject>();
    private int activeCount = 0;

    /// <summary>
    /// 현재 활성화(씬에서 사용 중)인 오브젝트 수
    /// </summary>
    public int ActiveCount => activeCount;

    private void Awake()
    {
        Prewarm();
    }

    private void Prewarm()
    {
        for (int i = 0; i < initialSize; i++)
        {
            PooledObject obj = CreateNew();
            Return(obj);
        }
    }

    private PooledObject CreateNew()
    {
        PooledObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        obj.gameObject.SetActive(false);
        obj.SetPool(this);
        return obj;
    }

    /// <summary>
    /// position 위치에 활성화된 오브젝트 하나를 가져옵니다.
    /// </summary>
    public PooledObject Get(Vector3 position)
    {
        PooledObject obj = pool.Count > 0 ? pool.Dequeue() : CreateNew();

        obj.transform.position = position;
        obj.gameObject.SetActive(true);
        activeCount++;

        obj.OnSpawned();

        return obj;
    }

    /// <summary>
    /// 오브젝트를 풀로 되돌립니다.
    /// </summary>
    public void Return(PooledObject obj)
    {
        obj.OnReturned();

        if (obj.gameObject.activeSelf)
        {
            obj.gameObject.SetActive(false);
            activeCount = Mathf.Max(0, activeCount - 1);
        }

        pool.Enqueue(obj);
    }
}

