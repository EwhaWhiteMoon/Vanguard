using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UnitNavigate", story: "[Unit] Navigates to [Target]", category: "Action", id: "af0ad2c43c39edae90f35f5b692116c1")]
public partial class UnitNavigateAction : Action
{
    [SerializeReference] public BlackboardVariable<UnitObj> Unit;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    protected override Status OnStart()
    {
        if(Target.Value == null) return Status.Failure;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(Target.Value == null) return Status.Failure;


        float dist = Vector3.Distance(Unit.Value.gameObject.transform.position, Target.Value.transform.position);
        if (dist <= Unit.Value.stat.Range)
        {
            Unit.Value.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            return Status.Success;
        }
        
        Unit.Value.gameObject.GetComponent<Rigidbody2D>().linearVelocity =  
            ((Target.Value.transform.position - Unit.Value.gameObject.transform.position).normalized * Unit.Value.stat.MoveSpeed);
        
        return Status.Running;
    }
}

