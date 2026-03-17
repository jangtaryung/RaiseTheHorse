using UnityEngine;

/// <summary>
/// 풀에 들어갈 오브젝트가 상속해서 사용하는 베이스 클래스.
/// </summary>
public class PooledObject : MonoBehaviour
{
    private ObjectPool pool;

    public void SetPool(ObjectPool objectPool)
    {
        pool = objectPool;
    }

    /// <summary>
    /// 풀에서 꺼내졌을 때 호출됩니다.
    /// </summary>
    public virtual void OnSpawned()
    {
    }

    /// <summary>
    /// 풀로 되돌아갈 때 호출됩니다.
    /// </summary>
    public virtual void OnReturned()
    {
    }

    /// <summary>
    /// 이 오브젝트를 자신의 풀로 되돌립니다.
    /// </summary>
    public void ReturnToPool()
    {
        if (pool != null)
        {
            pool.Return(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

