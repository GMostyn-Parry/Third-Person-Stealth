using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/Conditions/TimeElapsed")]
public class TimeElapsedCondition : Condition
{
    public override bool Evaluate(StateController controller)
    {
        return controller.TimeInState >= controller.stats.timeToSearch;
    }
}
