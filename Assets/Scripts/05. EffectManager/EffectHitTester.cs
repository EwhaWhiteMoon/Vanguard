using UnityEngine;

public class EffectDeathTester : MonoBehaviour
{
    IEffectManager E;
    void Start()
    {
        E = EffectManager.Instance;
        E.PlayEffect("Death", Vector3.zero);
    }

    public void Test()
    {
        E.PlayEffect("Death", Vector3.zero);
    }
}
