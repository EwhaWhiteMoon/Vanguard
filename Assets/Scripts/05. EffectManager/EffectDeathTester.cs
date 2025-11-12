using UnityEngine;

public class EffectTester : MonoBehaviour
{
    IEffectManager E;
    void Start()
    {
        E = EffectManager.Instance;
        E.PlayEffect("Hit", Vector3.zero);
    }

    public void Test()
    {
        E.PlayEffect("Hit", Vector3.zero);
    }
}
