using UnityEngine;

//An action that moves the agent between connected patrol points.
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
