using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindTarget", story: "[Agent] Find [Target]", category: "Action", id: "8f7a6865bacaf48de2c87182839e13cd")]
public partial class FindTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<UnitGameObj> Agent;
    [SerializeReference] public BlackboardVariable<UnitGameObj> Target;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Target.Value = null;
        float cur = float.PositiveInfinity;

        if (Agent.Value.combatManager == null) return Status.Running;
        foreach (GameObject g in Agent.Value.combatManager.units)
        {
            if(g == null || g.GetComponent<UnitGameObj>().Team == Agent.Value.Team) continue;
            
            float dist = Vector3.Distance(Agent.Value.transform.position, g.transform.position);
            if (dist <= cur)
            {
                cur = dist;
                Target.Value = g.GetComponent<UnitGameObj>();
            }
        }
        
        return Target.Value != null ? Status.Success : Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

