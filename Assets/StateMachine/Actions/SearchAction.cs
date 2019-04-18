using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "StateMachine/Actions/Search")]
public class SearchAction : Action
{
    public override void Act(StateController controller)
    {
        //Only set a new destination after reaching the last destination.
        if(controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance)
        {
            //Choose a random position to move to within the search radius.
            NavMeshHit hit;
            NavMesh.SamplePosition(controller.desiredPosition + Random.insideUnitSphere * controller.stats.searchRadius, out hit, controller.stats.samplePositionRange, NavMesh.AllAreas);

            controller.navMeshAgent.destination = hit.position;
        }
    }
}
