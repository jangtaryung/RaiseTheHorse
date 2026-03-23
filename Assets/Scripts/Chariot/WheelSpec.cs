using UnityEngine;

public class WheelSpec
{
    public string Name { get; set; }
    public float Accel { get; set; }           // 가속 성능
    public float CollisionDamage { get; set; } // 충돌 데미지 기본값
    public float Weight { get; set; }

    public WheelSpec(string name, float accel, float collisionDamage, float weight)
    {
        Name = name;
        Accel = accel;
        CollisionDamage = collisionDamage;
        Weight = weight;
    }

    // ===== 업그레이드 적용 메서드 =====

    public void ApplyAccelUpgrade(float delta)
    {
        Accel = Mathf.Max(0f, Accel + delta);
    }

    public void ApplyCollisionUpgrade(float delta)
    {
        CollisionDamage = Mathf.Max(0f, CollisionDamage + delta);
    }
}
