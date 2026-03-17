using UnityEngine;

public class WeaponSpec
{
    public string Name { get; set; }
    public float Range { get; set; }     // 활/창: 사거리·공격 거리
    public float BasePower { get; set; } // 검: 기본 위력(또는 활/창 추가 위력)
    public float Weight { get; set; }

    public WeaponSpec(string name, float range, float basePower, float weight)
    {
        Name = name;
        Range = range;
        BasePower = basePower;
        Weight = weight;
    }
}
