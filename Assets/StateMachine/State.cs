using UnityEngine;

//Class to represent a state in the state machine; performing the state actions, and performing transitions if the condition is met.
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

    //Change state on the first transition that evaluates to the transition value.
    private void CheckTransitions(StateController controller)
    {
        bool hasTransitioned = false; //Whether a state transition occured, we only want to do this once.
        int i = 0;

        while(i < transitions.Length && !hasTransitioned)
        {
            if(transitions[i].condition.Evaluate(controller) == transitions[i].transitionOnTrue)
            {
                //Perform any actions that need to happen during this transiton.
                foreach(Action action in transitions[i].actionsOnTransition)
                {
                    action.Act(controller);
                }

                //Peform the change in state for the state machine.
                controller.ChangeState(transitions[i].nextState);
                hasTransitioned = true;
            }

            ++i;
        }
    }
}
