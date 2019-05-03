using UnityEngine;

//A transition action to record the player last known position, and the velocity they were travelling at.
[CreateAssetMenu(menuName = "StateMachine/TransitionActions/Record")]
public class RecordAction : Action
{
    public override void Act(StateController controller)
    {
        controller.blackboard["LastTargetPosition"] = controller.target.transform.position;
        controller.blackboard["LastTargetVelocity"] = ((CharacterController)controller.blackboard["TargetController"]).velocity;
    }
}