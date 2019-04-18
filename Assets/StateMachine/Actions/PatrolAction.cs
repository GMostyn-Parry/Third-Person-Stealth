using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Actions/Patrol")]
public class PatrolAction : Action
{
    public override void Act(StateController controller)
    {
        //Only choose a new destination after reaching the last destination.
        if(controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance)
        {
            controller.navMeshAgent.destination = controller.NextPatrolPosition();
        }
    }
}
