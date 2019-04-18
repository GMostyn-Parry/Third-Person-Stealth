using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Conditions/AtPosition")]
public class AtPositionCondition : Condition
{
    public override bool Evaluate(StateController controller)
    {
        return !controller.ShouldMoveToPosition || controller.navMeshAgent.remainingDistance < controller.navMeshAgent.stoppingDistance;
    }
}
