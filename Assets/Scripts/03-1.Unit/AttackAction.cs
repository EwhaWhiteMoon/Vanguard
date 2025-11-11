using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[Unit] attacks [Target]", category: "Action", id: "ac1498b55c8b05e3698307ac070c18a1")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<UnitGameObj> Unit;
    [SerializeReference] public BlackboardVariable<UnitGameObj> Target;
    
     private float atkDelay = 0.0f;

    protected override Status OnStart()
    {
        if(Target.Value == null) return Status.Failure;
        
        atkDelay = 1 / Unit.Value.stat.AttackSpeed;
        if (atkDelay <= 0.0f)
        {
            return Status.Success;
        }
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(Target.Value == null) return Status.Failure;
        
        atkDelay -= Time.deltaTime;
        if (atkDelay <= 0)
        {
            Unit.Value.Attack(Target.Value);
            return Status.Success;
        }

        return Status.Running;
    }
}

