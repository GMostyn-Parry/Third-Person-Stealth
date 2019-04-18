using UnityEngine;

[CreateAssetMenu(menuName = "StateMachine/State")]
public class State : ScriptableObject
{
    public Action[] actions; //Actions performed in this state.
    public Transition[] transitions; //Transitions from this state.

    public void UpdateState(StateController controller)
    {
        PerformActions(controller);
        CheckTransitions(controller);
    }

    private void PerformActions(StateController controller)
    {
        foreach(Action action in actions)
        {
            action.Act(controller);
        }
    }

    private void CheckTransitions(StateController controller)
    {
        foreach(Transition transition in transitions)
        {
            if(transition.condition.Evaluate(controller) == transition.transitionOnTrue)
            {
                controller.ChangeState(transition.nextState);
            }
        }
    }
}
