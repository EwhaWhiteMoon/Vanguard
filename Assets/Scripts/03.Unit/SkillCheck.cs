using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SkillAction", story: "[Unit] Use Skills to [Target]", category: "Action")]
public partial class SkillAction : Action
{
    [SerializeReference] public BlackboardVariable<UnitObj> Unit;
    [SerializeReference] public BlackboardVariable<UnitObj> Target;
    
    private Animator animator;
    
    private float atkDelay = 0.0f;

    protected override Status OnStart()
    {
        if(Target.Value == null || Unit.Value.MP < Unit.Value.stat.ManaMax) return Status.Success;

        Unit.Value.MP = 0;
        
        animator = Unit.Value.GetComponent<Animator>();
        
        atkDelay = 1 * Unit.Value.stat.AttackSpeed;
        return atkDelay <= 0.0f ? Status.Success : Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(Target.Value == null) return Status.Failure;
        
        atkDelay -= Time.deltaTime;
        if (atkDelay <= 1.0f)
        {
            animator.SetBool("isAttack", true);
        }
        if (atkDelay <= 0.0f)
        {
            Unit.Value.UseSkill(Target.Value);
            animator.SetBool("isAttack", false);
            return Status.Success;
        }

        return Status.Running;
    }
}