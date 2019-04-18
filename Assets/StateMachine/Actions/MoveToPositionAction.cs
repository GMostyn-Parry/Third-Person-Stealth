using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Actions/MoveToPosition")]
public class MoveToPositionAction : Action
{
    public override void Act(StateController controller)
    {
        controller.navMeshAgent.destination = controller.desiredPosition;
    }
}
