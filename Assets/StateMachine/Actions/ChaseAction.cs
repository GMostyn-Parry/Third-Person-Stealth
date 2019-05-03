using UnityEngine;
using UnityEngine.AI;

//An action that causes the agent to move to the player's position, or as close as it can get.
[CreateAssetMenu(menuName = "StateMachine/Actions/Chase")]
public class ChaseAction : Action
{
    public override void Act(StateController controller)
    {
        //Sample position, rather than using the target's position, so that the target can't just stand outside the NavMesh.
        NavMesh.SamplePosition(controller.target.transform.position, out NavMeshHit hit, controller.stats.samplePositionRange, NavMesh.AllAreas);
        controller.navMeshAgent.SetDestination(hit.position);
    }
}
