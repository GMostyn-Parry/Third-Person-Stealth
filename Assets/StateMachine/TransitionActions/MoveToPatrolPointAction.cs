using UnityEngine;

//A transition action that causes the agent to move to its current patrol point.
[CreateAssetMenu(menuName = "StateMachine/TransitionActions/MoveToPatrolPoint")]
public class MoveToPatrolPointAction : Action
{
    public override void Act(StateController controller)
    {
        controller.navMeshAgent.destination = controller.currentPoint.transform.position;
    }
}
