using UnityEngine;

public class HorseSpec
{
    public string Name { get; set; }
    public float PullCapacity { get; set; }   // 견인력(수용 가능한 무게)
    public float BaseMoveSpeed { get; set; }  // 기본 이동 속도
    public float Weight { get; set; }         // 말 자체 무게

    public HorseSpec(string name, float pullCapacity, float baseMoveSpeed, float weight)
    {
        Name = name;
        PullCapacity = pullCapacity;
        BaseMoveSpeed = baseMoveSpeed;
        Weight = weight;
    }

    // ===== 업그레이드 적용 메서드 =====

    public void ApplySpeedUpgrade(float delta)
    {
        BaseMoveSpeed = Mathf.Max(0.1f, BaseMoveSpeed + delta);
    }

    public void ApplyPullUpgrade(float delta)
    {
        PullCapacity = Mathf.Max(1f, PullCapacity + delta);
    }
}
