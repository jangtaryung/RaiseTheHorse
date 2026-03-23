using UnityEngine;

/// <summary>
/// 인스펙터에서 값은 보이지만 수정은 불가능하게 만드는 어트리뷰트.
/// [ReadOnly] public float someValue;
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute { }
