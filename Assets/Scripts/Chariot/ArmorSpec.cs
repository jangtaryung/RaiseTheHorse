using UnityEngine;

public class ArmorSpec
{
    public string Name { get; set; }
    public float Durability { get; set; } // 전차 내구도
    public float Defense { get; set; }    // 방어 성능(피해 감소량)
    public float Weight { get; set; }

    public ArmorSpec(string name, float durability, float defense, float weight)
    {
        Name = name;
        Durability = durability;
        Defense = defense;
        Weight = weight;
    }
}
