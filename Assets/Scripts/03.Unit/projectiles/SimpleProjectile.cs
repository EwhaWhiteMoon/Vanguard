
using System;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    public Rigidbody2D rigid;
    public GameObject Target;
    public float Speed = 1.0f;

    public Action onHit;

    private void Start()
    {
        this.rigid = GetComponent<Rigidbody2D>();
    }

    public void init(GameObject target)
    {
        this.Target = target;
        //한윤구 추가
        SoundManager.Instance.PlaySFX("ADCAttack");
    }

    private void Update()
    {
        if(!Target)
        {
            Destroy(gameObject);
            return;
        }

        var dist = Target.transform.position - transform.position;
        if (dist.magnitude < 0.1f)
        {
            onHit?.Invoke();
            Destroy(gameObject);
        }
        rigid.linearVelocity = dist.normalized * Speed;
    }
}