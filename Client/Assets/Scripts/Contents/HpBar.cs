using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Transform hpBar;

    public void SetHpBar(float ratio)
    {
        ratio = Mathf.Clamp(ratio, 0, 1);
        hpBar.localScale = new Vector3(ratio, 1, 1);
    }
}