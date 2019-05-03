using UnityEngine;

//Condition that checks if the time in the state has exceeded the maximum amount of time to search.
[CreateAssetMenu(menuName = "StateMachine/Conditions/SearchTimeElapsed")]
public class SearchTimeElapsedCondition : Condition
{
    public override bool Evaluate(StateController controller)
    {
        return controller.TimeInState >= controller.stats.timeToSearch;
    }
}
