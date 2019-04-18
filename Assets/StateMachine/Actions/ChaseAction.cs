using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "StateMachine/Actions/Chase")]
public class ChaseAction : Action
{
    public override void Act(StateController controller)
    {
        //Sample position, rather than using the target's position, so that the target can't just stand outside the NavMesh.
        NavMeshHit hit;
        NavMesh.SamplePosition(controller.target.transform.position, out hit, controller.stats.samplePositionRange, NavMesh.AllAreas);
        controller.navMeshAgent.SetDestination(hit.position);

        //Set the position the target was last seen at.
        controller.desiredPosition = controller.target.transform.position;
    }
}
