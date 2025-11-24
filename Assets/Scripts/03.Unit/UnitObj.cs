using System.Drawing;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class UnitObj : MonoBehaviour
{
    public UnitData unitData { get; private set; }
    private SafeAnimatorLoader animatorLoader;
    public Stat stat;
    public int Team;

    // 한윤구 추가
    public UnitUIController uiInstance; // 생성된 UI를 관리할 변수
    //---------------------------------------

    private float _hp;

    public float HP
    {
        get => _hp;
        set
        {
            _hp = value;
            if(uiInstance)
                uiInstance.UpdateUI();
            //if(HPText)
            //    HPText.text = _hp.ToString("N0");

        }
    }
    public TextMeshPro HPText;
    public ICombatManager combatManager;

    public SimpleProjectile projectilePrefab;

    public Size objSize;

    void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        animatorLoader = GetComponent<SafeAnimatorLoader>();
        if (animatorLoader == null)
        {
            Debug.LogError("애니 로더가 없음");
        }
    }

    public void Init(UnitData data, int team, ICombatManager combatManager, float HP = -1)
    {
        this.unitData = data;
        Team = team;
        this.combatManager = combatManager;

        // 스탯 설정해야함.
        this.stat = new Stat(this.unitData.BaseStat);
        this.HP = HP == -1 ? this.unitData.BaseStat.MaxHealth : HP;

        // 애니메이터 적용
        animatorLoader.InitAnimator(data);
    }

    public void Attack(UnitObj target)
    {
        if (stat.Range <= 1.0f)
        {
            target.TakeDamage(stat.Attack * (Random.Range(0f, 1f) < stat.CritChance ? stat.CritMultiplier : 1f));
        }
        else
        {
            var Bullet = Instantiate(projectilePrefab, transform.position, transform.rotation);
            Bullet.init(target.gameObject);
            Bullet.onHit += () =>
            {
                target.TakeDamage(stat.Attack * (Random.Range(0f, 1f) < stat.CritChance ? stat.CritMultiplier : 1f));
            };
        }
    }

    public void TakeDamage(float damage)
    {
        //한윤구 추가
        SoundManager.Instance.PlaySFX("Hit");

        HP -= (damage - stat.Defense) * (1 - stat.DamageReducePct);
        if (HP <= 0)
        {
            EffectManager.Instance.PlayEffect("Death", transform.position);
            //한윤구 추가
            SoundManager.Instance.PlaySFX("Death");

            Destroy(this.gameObject);
        }
        EffectManager.Instance.PlayEffect("Hit", transform.position); // 이게 여기 들어가는 게 맞을까 생각 중. "피격"애니메이션이니까 괜찮지 않을까요?
    }
}