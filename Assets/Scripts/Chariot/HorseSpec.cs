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
}
