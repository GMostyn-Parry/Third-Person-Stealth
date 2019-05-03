using UnityEngine;
using UnityEngine.AI;

//An action to search for a recently lost target.
[CreateAssetMenu(menuName = "StateMachine/Actions/Search")]
public class SearchAction : Action
{
    public override void Act(StateController controller)
    {
        //Only set a new destination after reaching the last destination.
        if(controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance)
        {
            //Select a random position to move to on the NavMesh within the search radius, centred on where the target was last seen plus their last known velocity.
            NavMesh.SamplePosition((Vector3)controller.blackboard["LastTargetPosition"] + (Vector3)controller.blackboard["LastTargetVelocity"] + Random.insideUnitSphere * controller.stats.searchRadius, out NavMeshHit hit, controller.stats.samplePositionRange, NavMesh.AllAreas);

            controller.navMeshAgent.destination = hit.position;
        }
    }
}
